using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Client;
[System.Serializable]
public class UnitData
{
    public MonstersID.Value MonsterID;
    public float Damage;
    public float Health;
    public float MoveSpeed;
    public int Cost;
    public Sprite Sprite;
    public List<MonsterVisualAndAnimations> VisualAndAnimations = new List<MonsterVisualAndAnimations>();
    public GameObject[] Prefabs;
    public ElementalType Elemental;
}
