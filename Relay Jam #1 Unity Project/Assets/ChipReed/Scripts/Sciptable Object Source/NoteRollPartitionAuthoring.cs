using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewNoteRollPartition", menuName = "ScriptableObjects/NoteRollPartition")]
[System.Serializable]
public class NoteRollPartitionAuthoring : ScriptableObject
{
    [SerializeField] public int _16thsPerMeasure = 16;
    [SerializeField] public int _divisionsPerMeasure = 4;
    [SerializeField] public List<NoteRoll.LWNoteInstruction> instructions = new List<NoteRoll.LWNoteInstruction>();
    [SerializeField] public ENoteDuration visibleDivisions = ENoteDuration.sixteenth;
}
