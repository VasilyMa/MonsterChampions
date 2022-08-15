using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Client;
[System.Serializable]
public class UnitData
{
    public int MonsterID;
    public string NameUnit;
    public float Damage;
    public float Health;
    public float MoveSpeed;
    public GameObject[] Prefabs;
    public ElementalType Elemental;
}
