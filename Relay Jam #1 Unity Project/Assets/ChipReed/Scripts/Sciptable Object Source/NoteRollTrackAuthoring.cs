using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewNoteRollTrack", menuName = "ScriptableObjects/NoteRollTrack")]
[System.Serializable]
public class NoteRollTrackAuthoring : ScriptableObject
{
    [SerializeField] int timeSigUpper = 4;
    [SerializeField] int timeSigLower = 4;
    [SerializeField] int bpm = 120;

    [SerializeField] public List<NoteRollPartitionAuthoring> partitions = new List<NoteRollPartitionAuthoring>();

    public List<NoteRoll.LWNoteInstruction> MergePartitions()
    {
        List<NoteRoll.LWNoteInstruction> merged = new List<NoteRoll.LWNoteInstruction>();

        foreach (var partition in partitions)
        {
            foreach (var instruction in partition.instructions)
            {
                merged.Add(instruction);
            }
        }

        return merged;
    }
}
