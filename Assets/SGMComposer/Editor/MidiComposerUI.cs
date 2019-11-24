//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;
//using UnityEditorInternal;
//using static MidiComposerCore;

//[CustomEditor(typeof(MidiComposer))]
//public class MidiComposerUI : Editor
//{
//    ComposerCore composer = new ComposerCore();
//    TracksSet currentSet = null;

//    ReorderableList localTrackRL;
//    ReorderableList globalTracksRL;

//    ReorderableList presetsRL; // todo now
//    ReorderableList flowsRL;
    
//    List<bool> enabled = new List<bool>();

    
//    private void OnEnable()
//    {
//        localTrack = new ReorderableList(serializedObject,
//                serializedObject.FindProperty("localTracks"),
//                true, true, true, true);
//        localTrack.drawHeaderCallback = (Rect rect) => {
//            EditorGUI.LabelField(rect, "Tracks");
//        };
//        localTrack.drawElementCallback =
//         (Rect rect, int index, bool isActive, bool isFocused) =>
//         {
//             var element = localTrack.serializedProperty.GetArrayElementAtIndex(index);
//             rect.y += 2;

//             EditorGUI.LabelField(new Rect(rect.x, rect.y, 15, EditorGUIUtility.singleLineHeight), 
//                 new GUIContent("P"));
//             EditorGUI.PropertyField(
//                 new Rect(rect.x+15, rect.y, 30, EditorGUIUtility.singleLineHeight),
//                 element.FindPropertyRelative("isplaying"), GUIContent.none);

//             EditorGUI.LabelField(new Rect(rect.x + 45, rect.y, 15, EditorGUIUtility.singleLineHeight),
//                 new GUIContent("S"));
//             EditorGUI.PropertyField(
//                 new Rect(rect.x + 60, rect.y, 30, EditorGUIUtility.singleLineHeight),
//                 element.FindPropertyRelative("solo"), GUIContent.none);

//             EditorGUI.PropertyField(
//                 new Rect(rect.x + 200, rect.y, rect.width - 300, EditorGUIUtility.singleLineHeight),
//                 element.FindPropertyRelative("instrument"), GUIContent.none);
//             EditorGUI.PropertyField(
//                 new Rect(rect.x + 90, rect.y, 100, EditorGUIUtility.singleLineHeight),
//                 element.FindPropertyRelative("hash"), GUIContent.none);

//             EditorGUI.PropertyField(
//                 new Rect(rect.x + rect.width - 90, rect.y, 90, EditorGUIUtility.singleLineHeight),
//                 element.FindPropertyRelative("midi"), GUIContent.none);
//         };
//        localTrack.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
//            GenericMenu menu = new GenericMenu();
//            menu.AddItem(new GUIContent("Add Random..."), false, clickHandler, null);
//            menu.AddSeparator("");//add random stuff
//            menu.AddItem(new GUIContent("Melody/Synth"),false,clickHandler, new Tracks() as object);//add random stuff
//            menu.AddItem(new GUIContent("Melody/Piano"), false, clickHandler, new Tracks() as object);//add random stuff
//            menu.AddItem(new GUIContent("Melody/Brass"), false, clickHandler, new Tracks() as object);//add random stuff
//            menu.AddItem(new GUIContent("Melody/Guitar"), false, clickHandler, new Tracks() as object);//add random stuff
//            menu.AddItem(new GUIContent("Drum"), false, clickHandler, new Tracks() as object);//add random stuff
//            menu.AddItem(new GUIContent("Bass/BassGutar"), false, clickHandler, new Tracks() as object);//add random stuff
//            //menu.AddItem(new GUIContent("Mobs/" + Path.GetFileNameWithoutExtension(path)),
//            //false, clickHandler,
//            //new WaveCreationParams() { Type = MobWave.WaveType.Mobs, Path = path });
//            menu.ShowAsContext();
//        };

//        globalTrack = new ReorderableList(serializedObject,
//                serializedObject.FindProperty("globalTracks"),
//                true, true, true, true);
//        globalTrack.drawHeaderCallback = (Rect rect) => {
//            EditorGUI.LabelField(rect, "Tracks");
//        };
//        globalTrack.drawElementCallback =
//         (Rect rect, int index, bool isActive, bool isFocused) =>
//         {
//             var element = localTrack.serializedProperty.GetArrayElementAtIndex(index);
//             rect.y += 2;

//             EditorGUI.LabelField(new Rect(rect.x, rect.y, 15, EditorGUIUtility.singleLineHeight),
//                 new GUIContent("P"));
//             EditorGUI.PropertyField(
//                 new Rect(rect.x + 15, rect.y, 30, EditorGUIUtility.singleLineHeight),
//                 element.FindPropertyRelative("isplaying"), GUIContent.none);

//             EditorGUI.LabelField(new Rect(rect.x + 45, rect.y, 15, EditorGUIUtility.singleLineHeight),
//                 new GUIContent("S"));
//             EditorGUI.PropertyField(
//                 new Rect(rect.x + 60, rect.y, 30, EditorGUIUtility.singleLineHeight),
//                 element.FindPropertyRelative("solo"), GUIContent.none);

//             EditorGUI.PropertyField(
//                 new Rect(rect.x + 200, rect.y, rect.width - 300, EditorGUIUtility.singleLineHeight),
//                 element.FindPropertyRelative("instrument"), GUIContent.none);
//             EditorGUI.PropertyField(
//                 new Rect(rect.x + 90, rect.y, 100, EditorGUIUtility.singleLineHeight),
//                 element.FindPropertyRelative("hash"), GUIContent.none);

//             EditorGUI.PropertyField(
//                 new Rect(rect.x + rect.width - 90, rect.y, 90, EditorGUIUtility.singleLineHeight),
//                 element.FindPropertyRelative("midi"), GUIContent.none);
//         };
//        globalTrack.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
//            GenericMenu menu = new GenericMenu();
//            menu.AddItem(new GUIContent("Add Random..."), false, clickHandler, null);
//            menu.AddSeparator("");//add random stuff
//            menu.AddItem(new GUIContent("Melody/Synth"), false, clickHandler, new Tracks() as object);//add random stuff
//            menu.AddItem(new GUIContent("Melody/Piano"), false, clickHandler, new Tracks() as object);//add random stuff
//            menu.AddItem(new GUIContent("Melody/Brass"), false, clickHandler, new Tracks() as object);//add random stuff
//            menu.AddItem(new GUIContent("Melody/Guitar"), false, clickHandler, new Tracks() as object);//add random stuff
//            menu.AddItem(new GUIContent("Drum"), false, clickHandler, new Tracks() as object);//add random stuff
//            menu.AddItem(new GUIContent("Bass/BassGutar"), false, clickHandler, new Tracks() as object);//add random stuff
//            //menu.AddItem(new GUIContent("Mobs/" + Path.GetFileNameWithoutExtension(path)),
//            //false, clickHandler,
//            //new WaveCreationParams() { Type = MobWave.WaveType.Mobs, Path = path });
//            menu.ShowAsContext();
//        };
//    }
//    private void clickHandler(object target)
//    {
//        var data = (Tracks)target;
//        var index = localTrack.serializedProperty.arraySize;
//        localTrack.serializedProperty.arraySize++;
//        localTrack.index = index;
//        var element = localTrack.serializedProperty.GetArrayElementAtIndex(index);
//        //todo: apply attributes
//        element.FindPropertyRelative("Count").intValue = 10;
//        serializedObject.ApplyModifiedProperties();
//    }



//    bool trackListFold = false;
//    bool trackListFold2 = true;
//    int selectedTrackset = 0;
//    public override void OnInspectorGUI()
//    {
//        //base.OnInspectorGUI();

//        serializedObject.Update();


//        if (trackListFold2 = EditorGUILayout.Foldout(trackListFold2, "All Tracks"))
//        {
//            globalTracksRL.DoLayoutList();
//        }

//        string[] options = new string[composer.d.tracksets.Count + 1];
//        for (int i = 0; i < composer.d.tracksets.Count; i++)
//            options[i] = string.Format("Track {0}: {1}", i, composer.d.tracksets[i].name);
//        options[composer.d.tracksets.Count] = "New TrackSet";

//        int lastSelected = selectedTrackset;
//        selectedTrackset = EditorGUILayout.Popup("Trackset", selectedTrackset, options);
//        if (selectedTrackset == composer.d.tracksets.Count)
//            composer.d.tracksets.Add(new TracksSet());

//        if (lastSelected != selectedTrackset)
//        {
            
//        }


//        if (trackListFold = EditorGUILayout.Foldout(trackListFold, "Tracks"))
//        {
//            localTrackRL.DoLayoutList();
//        }

//        serializedObject.ApplyModifiedProperties();
//    }
//}
