using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AD_SO))]
public class CustomEnvelopeEditor : Editor
{
    AD_SO envSrc; //Readonly
    SerializedProperty timeDecay;

    void OnEnable()
    {
        envSrc = (AD_SO)target;
        timeDecay = serializedObject.FindProperty("timeD");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.LabelField($"{serializedObject.targetObject.name}");

        if(envSrc.timeA >= envSrc.timeD)
        {
            envSrc.timeD = envSrc.timeA + .01f;
            EditorUtility.SetDirty(target);
        }

        EditorGUILayout.Separator();
        DrawDefaultInspector();
        EditorGUILayout.Separator();

        EditorGUILayout.LabelField($"Preview: {serializedObject.targetObject.name}");
        AnimationCurve envelopeCurve = new AnimationCurve();

        envelopeCurve.keys = new UnityEngine.Keyframe[] { 
            new UnityEngine.Keyframe { time = 0, value = envSrc.levelStart, weightedMode = WeightedMode.None} , 
            new UnityEngine.Keyframe { time = envSrc.timeA, value = envSrc.levelA, weightedMode = WeightedMode.None }, 
            new UnityEngine.Keyframe { time = envSrc.timeD, value = envSrc.levelD, weightedMode = WeightedMode.None }
        };

        EditorGUILayout.CurveField(envelopeCurve, Color.yellow, new Rect(0 , 0, 5, 1), GUILayout.Height(300));
        
        serializedObject.ApplyModifiedProperties();
    }
}
