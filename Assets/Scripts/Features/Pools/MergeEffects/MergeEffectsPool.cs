using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MergeEffectsPool", menuName = "Pools/MergeEffectsPool", order = 0)]
public class MergeEffectsPool : ScriptableObject
{
    // Info from ExplosionEvent.Size
    [Header("")]
    public GameObject[] MergeEffectPrefab;
}
