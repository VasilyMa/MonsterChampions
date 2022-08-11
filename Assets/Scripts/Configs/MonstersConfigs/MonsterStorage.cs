using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Client;
[CreateAssetMenu(fileName = "NewMonster", menuName = "MonstersStorages/Monster", order = 0)]
public class MonsterStorage : ScriptableObject
{
    public int MonsterID;
    public string NameUnit;
    public float Damage;
    public float Health;
    public float MoveSpeed;
    public GameObject[] Prefabs;
    public Elemental.Type Elemental;

}
