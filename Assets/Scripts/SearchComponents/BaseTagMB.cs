using System.Collections.Generic;
using UnityEngine;

public class BaseTagMB : MonoBehaviour
{
    public bool isFriendly; // to do ay del this after create baseSpawnSystems

    [Range(-5, 5)]
    public int GoldAddingModifier;

    public Transform SpawnPoint;

    [Range(1, 10)]
    public int TimeToSpawn = 5;

    [Range(1, 4)]
    public int MonsterLevel;
    public List<MonstersSquad> MonstersSquads = new List<MonstersSquad>();
    public List<MonsterStorage> monster = new List<MonsterStorage>();

    
}

[System.Serializable]
public class MonstersSquad
{
    [Range(1, 4)]
    public int MonstersLevel;
    public List<MonsterStorage> Monsters = new List<MonsterStorage>();
}