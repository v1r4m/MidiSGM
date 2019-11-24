using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditorInternal;

using UnityEngine;
using static MidiComposerCore;



[Serializable]
public class StringStringListTuple
{
    [SerializeField] public string key;
    [SerializeField] public string val;
}
public static class StringStringListTupleListExtension
{
    public static Dictionary<string,List<string>> ToStringListDic(this List<StringStringListTuple> list)
    {
        Dictionary<string, List<string>> r = new Dictionary<string, List<string>>();
        char[] delimiters = new[] { ',' };
        foreach (var a in list)
            r.Add(a.key, new List<string>(a.val.Split(delimiters,StringSplitOptions.RemoveEmptyEntries)));
        return r;
    }
    public static List<StringStringListTuple> ToSSTuple(this Dictionary<string, List<string>>  list)
    {
        List<StringStringListTuple> r = new List<StringStringListTuple>();
        foreach (var a in list)
        {
            r.Add(new StringStringListTuple() { key = a.Key, val = String.Join(", ", a.Value.ToArray()) });
        }
            
        return r;
    }
}

public class DBOBJ : ScriptableObject
{
    [SerializeField] public List<Track> localTracks = new List<Track>(); // used in midicomposereditor
    [SerializeField] public List<Track> globalTracks = new List<Track>();
    [SerializeField] public string setName;
    [SerializeField] public List<StringStringListTuple> presets = new List<StringStringListTuple>();
    [SerializeField] public List<StringStringListTuple> flows = new List<StringStringListTuple>();
    

    static List<Track> activeTrack;


}

public class MidiComposerEditor : EditorWindow
{
    [MenuItem("Window/Midi SGM Composer Editor")]
    public static void ShowWindow()
    {
        MidiComposerEditor win = EditorWindow.GetWindow(typeof(MidiComposerEditor)) as MidiComposerEditor;
        
        win.Show();
    }

    static ComposerCore composer;
    
    static ReorderableList globalTracksRL;

    static ReorderableList presetsRL; // todo now
    static ReorderableList flowsRL;



    List<bool> enabled = new List<bool>();

    
    static DBOBJ mc;
    static SerializedObject so;
    const string savedir = "ComposerSav.sav";
    static System.Random rng = new System.Random();

    void OnEnable()
    {
        if (composer == null)
        {
            composer = ComposerCore.Instance;
            if (File.Exists(savedir))
                composer.d = Data.Desrialize(File.ReadAllText(savedir));
        }
        Debug.Log("MidiCompoerEditor is Awake!");

        
        mc = ScriptableObject.CreateInstance<DBOBJ>();
        
        mc.globalTracks = composer.d.tracks;

        so = new SerializedObject(mc);

        so.Update();
        globalTracksRL = new ReorderableList(so,
                so.FindProperty("globalTracks"),
                true, true, true, true);
        globalTracksRL.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Tracks");
        };
        globalTracksRL.drawElementCallback =
         (Rect rect, int index, bool isActive, bool isFocused) =>
         {
             var element = globalTracksRL.serializedProperty.GetArrayElementAtIndex(index);
             rect.y += 2;

             EditorGUI.LabelField(new Rect(rect.x, rect.y, 15, EditorGUIUtility.singleLineHeight),
                 new GUIContent("P"));
             EditorGUI.PropertyField(
                 new Rect(rect.x + 15, rect.y, 30, EditorGUIUtility.singleLineHeight),
                 element.FindPropertyRelative("isplaying"), GUIContent.none);

             EditorGUI.LabelField(new Rect(rect.x + 45, rect.y, 15, EditorGUIUtility.singleLineHeight),
                 new GUIContent("S"));
             EditorGUI.PropertyField(
                 new Rect(rect.x + 60, rect.y, 30, EditorGUIUtility.singleLineHeight),
                 element.FindPropertyRelative("issolo"), GUIContent.none);

             EditorGUI.PropertyField(
                 new Rect(rect.x + 200, rect.y, rect.width - 300, EditorGUIUtility.singleLineHeight),
                 element.FindPropertyRelative("instrument"), GUIContent.none);
             EditorGUI.PropertyField(
                 new Rect(rect.x + 90, rect.y, 100, EditorGUIUtility.singleLineHeight),
                 element.FindPropertyRelative("name"), GUIContent.none);

             EditorGUI.PropertyField(
                 new Rect(rect.x + rect.width - 90, rect.y, 90, EditorGUIUtility.singleLineHeight),
                 element.FindPropertyRelative("midi"), GUIContent.none);
         };
        globalTracksRL.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
           
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add Blank..."), false, ClickHandler, new Track() as object);
            menu.AddSeparator("");//add random stuff
            menu.AddItem(new GUIContent("Random Drum"), false, ClickHandler, new Track() as object);//add random stuff
            //menu.AddItem(new GUIContent("Mobs/" + Path.GetFileNameWithoutExtension(path)),
            //false, clickHandler,
            //new WaveCreationParams() { Type = MobWave.WaveType.Mobs, Path = path });
            menu.ShowAsContext();
        };
        
        presetsRL = new ReorderableList(so,
                so.FindProperty("presets"),
                true, true, true, true);
        presetsRL.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "presets");
        };
        presetsRL.drawElementCallback =
         (Rect rect, int index, bool isActive, bool isFocused) =>
         {
             var element = presetsRL.serializedProperty.GetArrayElementAtIndex(index);
             rect.y += 2;

             EditorGUI.LabelField(new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight),
                 new GUIContent("preset " + (index + 1)));
             EditorGUI.PropertyField(
                 new Rect(rect.x + 60, rect.y, 140, EditorGUIUtility.singleLineHeight),
                 element.FindPropertyRelative("key"), GUIContent.none);

             EditorGUI.PropertyField(
                 new Rect(rect.x + 205, rect.y, rect.width - 230, EditorGUIUtility.singleLineHeight),
                 element.FindPropertyRelative("val"), GUIContent.none);
             if (GUI.Button(new Rect(rect.x + rect.width - 20, rect.y, 20, EditorGUIUtility.singleLineHeight), "P"))
             {
                 ComposerCore.Instance.p.preset_flow_name = mc.presets[index].key;
                 ComposerCore.Instance.p.mode = PlayingData.Mode.preset;
             }
         };
        presetsRL.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add New"), false, NewPreset, null);
            menu.ShowAsContext();
        };
        flowsRL = new ReorderableList(so,
                so.FindProperty("flows"),
                true, true, true, true);
        flowsRL.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "flows");
        };

        flowsRL.drawElementCallback =
         (Rect rect, int index, bool isActive, bool isFocused) =>
         {
             var element = flowsRL.serializedProperty.GetArrayElementAtIndex(index);
             rect.y += 2;

             EditorGUI.LabelField(new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight),
                 new GUIContent("flow " + (index + 1)));
             EditorGUI.PropertyField(
                 new Rect(rect.x + 60, rect.y, 140, EditorGUIUtility.singleLineHeight),
                 element.FindPropertyRelative("key"), GUIContent.none);

             EditorGUI.PropertyField(
                 new Rect(rect.x + 205, rect.y, rect.width - 230, EditorGUIUtility.singleLineHeight),
                 element.FindPropertyRelative("val"), GUIContent.none);

             if (GUI.Button(new Rect(rect.x + rect.width - 20, rect.y, 20, EditorGUIUtility.singleLineHeight), "P"))
             {
                 ComposerCore.Instance.p.preset_flow_name = mc.flows[index].key;
                 ComposerCore.Instance.p.mode = PlayingData.Mode.flow;
             }
         };

        flowsRL.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add New"), false, NewFlow, null);
            menu.ShowAsContext();
        };
    }
    void OnDisable()
    {
        File.WriteAllText(savedir, composer.d.Serialize());
    }
    private void ClickHandler(object target)
    {
        var data = (Track)target;
        composer.d.tracks.Add(data);
    }
    private void NewPreset(object target)
    {
        var v = composer.d.tracksets[selectedTrackset].presets;
        v.Add("Preset " + (v.Count + 1), new List<string>());
    }
    private void NewFlow(object target)
    {
        var v = composer.d.tracksets[selectedTrackset].flows;
        v.Add("Flow " + (v.Count + 1), new List<string>());
    }



    bool trackListFold = true;

    int selectedTrackset = 0;
    void OnGUI()
    {
        if (so == null) OnEnable();
        if (selectedTrackset == composer.d.tracksets.Count)
            composer.d.tracksets.Add(new TracksSet());

        mc.presets = composer.d.tracksets[selectedTrackset].presets.ToSSTuple();
        mc.flows = composer.d.tracksets[selectedTrackset].flows.ToSSTuple();
        so. Update();

        if (trackListFold = EditorGUILayout.Foldout(trackListFold, "All Tracks"))
        {
            globalTracksRL.DoLayoutList();
        } 
         
        string[] options = new string[composer.d.tracksets.Count + 1];
        for (int i = 0; i < composer.d.tracksets.Count; i++)
            options[i] = string.Format("Track {0}: {1}", i, composer.d.tracksets[i].name);
        options[composer.d.tracksets.Count] = "New Set";
         

        int lastSelected = selectedTrackset;   
        selectedTrackset = EditorGUILayout.Popup("Trackset", selectedTrackset, options);
        if (selectedTrackset == composer.d.tracksets.Count)
            composer.d.tracksets.Add(new TracksSet());
    

        EditorGUILayout.PropertyField(so.FindProperty("setName"));
    
        presetsRL.DoLayoutList();
        flowsRL.DoLayoutList();
        
        so.ApplyModifiedProperties();

        composer.d.tracksets[selectedTrackset].presets = mc.presets.ToStringListDic();
        composer.d.tracksets[selectedTrackset].flows = mc.flows.ToStringListDic();
    }
}
