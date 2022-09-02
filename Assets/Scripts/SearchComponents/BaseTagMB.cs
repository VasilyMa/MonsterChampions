using System.Collections.Generic;
using System;
using UnityEngine;

public class BaseTagMB : MonoBehaviour
{
    public bool isFriendly; // to do ay del this after create baseSpawnSystems

    [Range(1, 50)]
    public int BaseHealth;

    [Range(-5, 5)]
    public int GoldAddingModifier;

    public Transform SpawnPoint;
    public List<MonstersSquad> MonstersSquads = new List<MonstersSquad>();
}

[Serializable]
public class MonstersSquad
{
    public string Name = "Squad_";
    [Range(1, 4)]
    public int MonstersLevel;
    public List<MonsterStorage> Monsters = new List<MonsterStorage>();
}