using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AD Envelope", menuName = "ScriptableObjects/AD_Envelope")]
[System.Serializable]
public class AD_SO : ScriptableObject
{
    [SerializeField] [Range(0, 1)] public float levelStart;
    [SerializeField] [Range(0, 5)] public float timeA;
    [SerializeField] [Range(0, 1)] public float levelA;

    [SerializeField] [Range(0, 5)] public float timeD;
    [SerializeField] [Range(0, 1)] public float levelD;
}
