using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChipReedSettings", menuName = "ScriptableObjects/ChipReedSettings")]
[System.Serializable]
public class ChipReedSettings : ScriptableObject
{
    [SerializeField][Tooltip("Only works on Windows")] public bool previewBeeps;
    public static bool _previewBeeps = true;
}
