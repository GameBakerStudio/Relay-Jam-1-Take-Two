using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Media;

[CustomEditor(typeof(NoteRollPartitionAuthoring))]
public class CustomNoteRollEditor : Editor
{
    SerializedProperty noteInstruction;
    SerializedProperty _16thsPerMeasure;
    SerializedProperty _divisionsPerMeasure;
    SerializedProperty visibleDivisions;
    NoteRollPartitionAuthoring authoring;
    ReorderableList list;

    int sixteenthsPerMeasure = 16;
    int divisionsPerMeasure = 4;

    private void OnEnable()
    {
        noteInstruction = serializedObject.FindProperty("instructions");
        _16thsPerMeasure = serializedObject.FindProperty("_16thsPerMeasure");
        _divisionsPerMeasure = serializedObject.FindProperty("_divisionsPerMeasure");
        visibleDivisions = serializedObject.FindProperty("visibleDivisions");
        authoring = (NoteRollPartitionAuthoring)target;
   
        list = new ReorderableList(serializedObject, noteInstruction, true, true, true, true);

        list.drawElementCallback = DrawListItems;
        list.drawHeaderCallback = DrawHeader;
        list.onAddCallback = OnAdd;
        list.onRemoveCallback = OnDelete;
    }

    void DrawHeader(Rect rect)
    {
        string name = $"        Note (ASPN)        AD Preset             V Freq,Depth    B (ASPN), Time   Velocity";
        EditorGUI.LabelField(rect, name);
    }

    private void OnAdd(ReorderableList list)
    {
        // Unused at the moment
    }

    private void OnDelete(ReorderableList list)
    {
        // Unused at the moment
    }

    // Yes, this code is just a bit cursed. Sorry.
    private Vector2 list0Position;
    void DrawListItems(Rect rect, int index, bool isActive, bool focused)
    {
        if (index == 0)
        {
            list0Position = rect.position;
        }
        int noteGranularity = NoteDurationToInt(authoring.visibleDivisions);

        // This borks the list layout :(
        // rect.y = (rect.y + list0Position.y) / NoteDurationToInt(ENoteDuration.eighth);

        var styleSmall = new GUIStyle();
        styleSmall.fontSize = 10;

        int divisionMultiple = sixteenthsPerMeasure / divisionsPerMeasure;

        float noteColumnStart = rect.x + 40;
        float envelopeColumnStart = rect.x + 80;
        float vibratoColumnStart = rect.x + 200;
        float bendColumnStart = rect.x + 280;
        float velocityColumnStart = rect.x + 380;

        SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index); // The element in the list

        string measureNum = string.Empty;
        if(index == 0 || index % sixteenthsPerMeasure == 0)
        {
            measureNum = (index / 16).ToString();
        }

        //Use something like this if live tracking ever works?
        //EditorGUI.DrawRect(new Rect(rect.position, new Vector2(EditorGUIUtility.currentViewWidth, EditorGUIUtility.singleLineHeight * 1.2f)), Color.black);

        EditorGUI.LabelField(new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight), $"{measureNum}");
        
        if (ShouldShowElement(index, noteGranularity))
        {
            EditorGUI.LabelField(new Rect(rect.x + 20, rect.y + 5, 100, EditorGUIUtility.singleLineHeight), $"{(index % sixteenthsPerMeasure)/ noteGranularity + 1}", styleSmall);

            var newInput = EditorGUI.DelayedTextField(
                new Rect(noteColumnStart, rect.y, 40, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("ASPN").stringValue
            );

            if (newInput != authoring.instructions[index].ASPN)
            {
                int? newNoteValue = newInput.ASPN2Int();
                if (newNoteValue != null)
                {
                    AudioUtil.PlayBeepProcess((int)Wave.Freq(newNoteValue), 300);
                }
            }

            element.FindPropertyRelative("ASPN").stringValue = newInput;

            EditorGUI.PropertyField(
                new Rect(envelopeColumnStart, rect.y, 120, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("ADEnvelopeID"),
                GUIContent.none
            );

            EditorGUI.PropertyField(
                new Rect(vibratoColumnStart, rect.y, 40, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("vibratoFrequency"),
                GUIContent.none
            );

            EditorGUI.PropertyField(
                new Rect(vibratoColumnStart + 40, rect.y, 40, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("vibratoDepth"),
                GUIContent.none
            );

            EditorGUI.PropertyField(
                new Rect(bendColumnStart, rect.y, 60, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("bendTarget"),
                GUIContent.none
            );

            EditorGUI.PropertyField(
                new Rect(bendColumnStart + 60, rect.y, 40, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("bendTime"),
                GUIContent.none
            );

            EditorGUI.PropertyField(
                new Rect(velocityColumnStart, rect.y, 40, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("velocity"),
                GUIContent.none
            );
        }

        #region dividers

        //Draw horizontal dividers

        Rect r = rect;
        r.height = (index % sixteenthsPerMeasure + 1) == sixteenthsPerMeasure ? 3 : 1;
        r.y += EditorGUIUtility.singleLineHeight + 3;

        Color c = new Color(0.5f, 0.5f, 0.5f, 1);

        if (index != 0 && divisionMultiple != 0 && index % divisionMultiple + 1 == divisionMultiple)
        {
            c = Color.yellow;
        }
        if (index != 0 && sixteenthsPerMeasure != 0 && (index % sixteenthsPerMeasure + 1) == sixteenthsPerMeasure)
        {
            c = Color.green;
        }

        EditorGUI.DrawRect(r, c);

        //Draw vertical dividers

        r.x = envelopeColumnStart;
        r.y -= EditorGUIUtility.singleLineHeight + 5;
        r.height = EditorGUIUtility.singleLineHeight + 5;
        r.width = 1;
        EditorGUI.DrawRect(r, Color.red);

        r.x = noteColumnStart;
        EditorGUI.DrawRect(r, Color.red);

        r.x = vibratoColumnStart;
        EditorGUI.DrawRect(r, Color.red);

        r.x = bendColumnStart;
        EditorGUI.DrawRect(r, Color.red);

        r.x = velocityColumnStart;
        EditorGUI.DrawRect(r, Color.red);

        #endregion
    }

    int NoteDurationToInt(ENoteDuration noteDuration)
    {
        switch (noteDuration)
        {
            case ENoteDuration.sixteenth:
                return 1;
            case ENoteDuration.eighth:
                return 2;
            case ENoteDuration.quarter:
                return 4;
            case ENoteDuration.half:
                return 8;
            case ENoteDuration.whole:
                return 16;
            default: return 1;
        }
    }

    bool ShouldShowElement(int index, int noteGranularity)
    {
        var positionInMeasure = index % sixteenthsPerMeasure;
        if(positionInMeasure % noteGranularity == 0)
        {
            return true;
        }
        return false;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.LabelField($"Note Roll Partition: {serializedObject.targetObject.name}");

        EditorGUILayout.Separator();

        EditorGUILayout.PropertyField(visibleDivisions);

        var spm = EditorGUILayout.DelayedIntField("16ths Per Measure" , authoring._16thsPerMeasure);
        var dpm = EditorGUILayout.DelayedIntField("Divisions Per Measure", authoring._divisionsPerMeasure);

        if (spm <= 0) spm = 1;
        if (dpm <= 0) dpm = 1;

        authoring._16thsPerMeasure = spm;
        authoring._divisionsPerMeasure = dpm;

        sixteenthsPerMeasure = spm;
        divisionsPerMeasure = dpm;

        EditorGUILayout.Separator();

        var measures = EditorGUILayout.DelayedIntField("Measures", authoring.instructions.Count/sixteenthsPerMeasure);
        if (measures < 1) measures = 1;
        if (measures > 100) measures = 100;

        while (authoring.instructions.Count / sixteenthsPerMeasure < measures)
        {
            authoring.instructions.Add(new NoteRoll.LWNoteInstruction());
        }

        while(authoring.instructions.Count > measures * sixteenthsPerMeasure)
        {
            authoring.instructions.RemoveAt(authoring.instructions.Count-1);
        }

        var changed = EditorGUI.EndChangeCheck();

        if(changed) EditorUtility.SetDirty(target);

        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();

        if(GUILayout.Button("Export json"))
        {
            string json = JsonUtility.ToJson(authoring);
            TextAsset saveFile = new TextAsset(json);

            var asset = AssetDatabase.AssetPathToGUID($"Assets/ChipReed/Exports/{authoring.name}.json");
            TextAsset textAsset = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(asset), typeof(TextAsset)) as TextAsset;
            if (textAsset != null)
            {
                File.WriteAllText(AssetDatabase.GUIDToAssetPath(asset), json);
                EditorUtility.SetDirty(textAsset);
            }
            else
            {
                AssetDatabase.CreateAsset(saveFile, $"Assets/ChipReed/Exports/{authoring.name}.json");
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

}

