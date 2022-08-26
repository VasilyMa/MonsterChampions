using System.Collections.Generic;
using UnityEngine;

public class BaseTagMB : MonoBehaviour
{
    public bool isFriendly; // to do ay del this after create baseSpawnSystems

    [Range(1, 10)]
    public float TimeToSpawn = 5;

    [Range(1, 4)]
    public int MonsterLevel;
    public List<MonsterStorage> monster = new List<MonsterStorage>();
}
