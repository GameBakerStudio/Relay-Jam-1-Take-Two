using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NoteRollTrackAuthoring))]
public class CustomTrackEditor : Editor
{
    NoteRollTrackAuthoring authoring;
    private void OnEnable()
    {
        authoring = (NoteRollTrackAuthoring)target;
    }

    public override void OnInspectorGUI()
    {
        var instructions = 0;

        foreach (NoteRollPartitionAuthoring partition in authoring.partitions)
        {
            instructions += partition.instructions.Count;
        }

        EditorGUILayout.LabelField($"Current instruction count: {instructions.ToString()}");
        EditorGUILayout.Separator();

        base.OnInspectorGUI();
    }
}
