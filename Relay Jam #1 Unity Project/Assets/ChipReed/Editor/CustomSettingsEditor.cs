using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ChipReedSettings))]
public class CustomSettingsEditor : Editor
{
    ChipReedSettings settings;
    SerializedProperty previewBeeps;
    private void OnEnable()
    {
        settings = (ChipReedSettings)target;
        previewBeeps = serializedObject.FindProperty("previewBeeps");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(previewBeeps);
        ChipReedSettings._previewBeeps = previewBeeps.boolValue;
        
        serializedObject.ApplyModifiedProperties();
    }
}
