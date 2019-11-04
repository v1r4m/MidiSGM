using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(MidiComposer))]
public class MidiComposerUI : Editor
{
    ReorderableList tracksList;

    bool trackListFold = true;

    List<bool> enabled = new List<bool>();

    private void OnEnable()
    {
        tracksList = new ReorderableList(serializedObject,
                serializedObject.FindProperty("tracks"),
                true, true, true, true);
        tracksList.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Tracks");
        };
        tracksList.drawElementCallback =
         (Rect rect, int index, bool isActive, bool isFocused) =>
         {
             var element = tracksList.serializedProperty.GetArrayElementAtIndex(index);
             rect.y += 2;

             EditorGUI.LabelField(new Rect(rect.x, rect.y, 15, EditorGUIUtility.singleLineHeight), 
                 new GUIContent("P"));
             EditorGUI.PropertyField(
                 new Rect(rect.x+15, rect.y, 30, EditorGUIUtility.singleLineHeight),
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
        tracksList.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
            GenericMenu.var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add Random..."), false, clickHandler, null);
            menu.AddSeparator("");//add random stuff
            menu.AddItem(new GUIContent("Melody/Synth"),false,clickHandler, new Tracks() as object);//add random stuff
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
    }
    private void clickHandler(object target)
    {
        var data = (Tracks)target;
        var index = tracksList.serializedProperty.arraySize;
        tracksList.serializedProperty.arraySize++;
        tracksList.index = index;
        var element = tracksList.serializedProperty.GetArrayElementAtIndex(index);
        //todo: apply attributes
        element.FindPropertyRelative("Count").intValue = 10;
        serializedObject.ApplyModifiedProperties();
    }

    
    
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        serializedObject.Update();


        tracksList.DoLayoutList();
            /*
        if (trackListFold = EditorGUILayout.Foldout(trackListFold, "Tracks"))
        {
            
            for (int i = 0; i < tracksList.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                string fieldName = "no" + i;
                //tracksList.stringValue;
                EditorGUILayout.PrefixLabel(fieldName + " P");
                enabled[i] = EditorGUILayout.Toggle(enabled[i]);
                EditorGUILayout.EndHorizontal();
            }
        }
        string k = "";
        for (int i = 0; i < tracksList.arraySize; i++)
        {
            k += enabled[i] ? i + "," : "";
        }

        var presets_ = presets.Copy();
        presets_.Next(true); presets_.Next(true); presets_.Next(true);
        presets_.
        */
        serializedObject.ApplyModifiedProperties();

    }
}
