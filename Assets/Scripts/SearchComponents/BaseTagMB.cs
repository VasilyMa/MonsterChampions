using System.Collections.Generic;
using System;
using UnityEngine;

public class BaseTagMB : MonoBehaviour
{
    public bool isFriendly;

    [Range(1, 100)]
    public int BaseHealth = 10;

    [Range(-10, 10)]
    public int GoldAddingModifier;

    public Transform SpawnPoint;
    public List<MonstersSquad> MonstersSquads = new List<MonstersSquad>();
}

[Serializable]
public class MonstersSquad
{
    public string Name = "Squad_";
    [Range(1, 4)]
    public int MonstersLevel = 1;
    public List<MonsterStorage> Monsters = new List<MonsterStorage>();
}