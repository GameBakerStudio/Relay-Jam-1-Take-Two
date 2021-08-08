using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MultiSourceManager))]
public class CustomMultiSourceEditor : Editor
{
    MultiSourceManager msm;
    private void OnEnable()
    {
        msm = (MultiSourceManager)target;
    }

    public override void OnInspectorGUI()
    {
        if(GUILayout.Button("Test Play"))
        {
            msm.Play();
        }
        if (msm.playing && GUILayout.Button("Test Pause"))
        {
            msm.Pause();
        }
        if (!msm.playing && GUILayout.Button("Test Resume"))
        {
            msm.Resume();
        }
        if (msm.playing && GUILayout.Button("Test Stop"))
        {
            msm.Stop();
        }

        base.OnInspectorGUI();

        if (msm.managedOscillators.Count < 1 || msm.managedOscillators[0]?.GetNoteRoll() == null) return;

        int firstInstructionCount = msm.managedOscillators[0].GetNoteRoll().GetInstructionCountNoMerge();
        bool flagForDiff = false;

        foreach (OscillatorSource oscillator in msm.managedOscillators)
        {
            if (oscillator?.GetNoteRoll() == null) continue;
            int localCount = oscillator.GetNoteRoll().GetInstructionCountNoMerge();
            if(localCount != firstInstructionCount) { flagForDiff = true; break; }
        }

        if(flagForDiff)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorStyles.label.wordWrap = true;
            EditorGUILayout.LabelField("Instruction counts in managed oscillators are not all the same. This is fine, but the max instruction count will be used for the track length");
        }
    }
}
