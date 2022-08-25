using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Client;
[System.Serializable]
public class UnitData
{
    public int UnitID;
    public string NameUnit;
    public float Damage;
    public float Health;
    public float MoveSpeed;
    public List<MonsterVisualAndAnimations> VisualAndAnimations = new List<MonsterVisualAndAnimations>();
    public GameObject[] Prefabs;
    public ElementalType Elemental;
}
