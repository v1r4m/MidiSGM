using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEditorInternal;

using UnityEngine;

public class MidiComposerEditor : EditorWindow
{
    [MenuItem("Window/MIDI_SGM/Composer Editor")]
    public static void ShowWindow()
    {
        MidiComposerEditor win = EditorWindow.GetWindow(typeof(MidiComposerEditor)) as MidiComposerEditor;
        
        win.Show();
    }

    [SerializeField] public List<Tracks> localTracks;
    [SerializeField] public List<TracksSet> tracksets;
    [SerializeField] public List<string> presets;
    [SerializeField] public List<string> flows;
    [SerializeField] public List<Tracks> globalTracks;

    static ReorderableList localTrackRL;
    static ReorderableList globalTracksRL;
    static ReorderableList tracksetsRL;
    static ReorderableList presetsRL; // todo now
    static ReorderableList flowsRL;

    List<bool> enabled = new List<bool>();

    public void Awake()
    {
        Debug.Log("MidiCompoerEditor is Awake");
        localTrackRL = new ReorderableList(localTracks, typeof(Tracks));
        localTrackRL.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Tracks");
        };
        localTrackRL.drawElementCallback =
         (Rect rect, int index, bool isActive, bool isFocused) =>
         {
             var element = localTrackRL.serializedProperty.GetArrayElementAtIndex(index);
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
                 element.FindPropertyRelative("solo"), GUIContent.none);

             EditorGUI.PropertyField(
                 new Rect(rect.x + 200, rect.y, rect.width - 300, EditorGUIUtility.singleLineHeight),
                 element.FindPropertyRelative("instrument"), GUIContent.none);
             EditorGUI.PropertyField(
                 new Rect(rect.x + 90, rect.y, 100, EditorGUIUtility.singleLineHeight),
                 element.FindPropertyRelative("hash"), GUIContent.none);

             EditorGUI.PropertyField(
                 new Rect(rect.x + rect.width - 90, rect.y, 90, EditorGUIUtility.singleLineHeight),
                 element.FindPropertyRelative("midi"), GUIContent.none);
         };
        localTrackRL.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add Random..."), false, clickHandler, null);
            menu.AddSeparator("");//add random stuff
            menu.AddItem(new GUIContent("Melody/Synth"), false, clickHandler, new Tracks() as object);//add random stuff
            menu.AddItem(new GUIContent("Melody/Piano"), false, clickHandler, new Tracks() as object);//add random stuff
            menu.AddItem(new GUIContent("Melody/Brass"), false, clickHandler, new Tracks() as object);//add random stuff
            menu.AddItem(new GUIContent("Melody/Guitar"), false, clickHandler, new Tracks() as object);//add random stuff
            menu.AddItem(new GUIContent("Drum"), false, clickHandler, new Tracks() as object);//add random stuff
            menu.AddItem(new GUIContent("Bass/BassGutar"), false, clickHandler, new Tracks() as object);//add random stuff
            //menu.AddItem(new GUIContent("Mobs/" + Path.GetFileNameWithoutExtension(path)),
            //false, clickHandler,
            //new WaveCreationParams() { Type = MobWave.WaveType.Mobs, Path = path });
            menu.ShowAsContext();
        };

        globalTracksRL = new ReorderableList(globalTracks, typeof(Tracks));
        globalTracksRL.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Tracks");
        };
        globalTracksRL.drawElementCallback =
         (Rect rect, int index, bool isActive, bool isFocused) =>
         {
             var element = localTrackRL.serializedProperty.GetArrayElementAtIndex(index);
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
                 element.FindPropertyRelative("solo"), GUIContent.none);

             EditorGUI.PropertyField(
                 new Rect(rect.x + 200, rect.y, rect.width - 300, EditorGUIUtility.singleLineHeight),
                 element.FindPropertyRelative("instrument"), GUIContent.none);
             EditorGUI.PropertyField(
                 new Rect(rect.x + 90, rect.y, 100, EditorGUIUtility.singleLineHeight),
                 element.FindPropertyRelative("hash"), GUIContent.none);

             EditorGUI.PropertyField(
                 new Rect(rect.x + rect.width - 90, rect.y, 90, EditorGUIUtility.singleLineHeight),
                 element.FindPropertyRelative("midi"), GUIContent.none);
         };
        globalTracksRL.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add Random..."), false, clickHandler, null);
            menu.AddSeparator("");//add random stuff
            menu.AddItem(new GUIContent("Melody/Synth"), false, clickHandler, new Tracks() as object);//add random stuff
            menu.AddItem(new GUIContent("Melody/Piano"), false, clickHandler, new Tracks() as object);//add random stuff
            menu.AddItem(new GUIContent("Melody/Brass"), false, clickHandler, new Tracks() as object);//add random stuff
            menu.AddItem(new GUIContent("Melody/Guitar"), false, clickHandler, new Tracks() as object);//add random stuff
            menu.AddItem(new GUIContent("Drum"), false, clickHandler, new Tracks() as object);//add random stuff
            menu.AddItem(new GUIContent("Bass/BassGutar"), false, clickHandler, new Tracks() as object);//add random stuff
            //menu.AddItem(new GUIContent("Mobs/" + Path.GetFileNameWithoutExtension(path)),
            //false, clickHandler,
            //new WaveCreationParams() { Type = MobWave.WaveType.Mobs, Path = path });
            menu.ShowAsContext();
        };

        tracksetsRL = new ReorderableList(tracksets, typeof(TracksSet));
    }

    private void clickHandler(object target)
    {
        var data = (Tracks)target;
        var index = localTrackRL.count;
        localTracks.Add(data);
        globalTracks.Add(data);
    }


    bool trackListFold = true;


    void OnGUI()
    {
        if (trackListFold = EditorGUILayout.Foldout(trackListFold, "All Tracks"))
        {
            globalTracksRL.DoLayoutList();
        }
        tracksetsRL.DoLayoutList();

        if (trackListFold = EditorGUILayout.Foldout(trackListFold, "Tracks"))
        {
            localTrackRL.DoLayoutList();
        }
    }
}
