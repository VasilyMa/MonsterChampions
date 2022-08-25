using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;
using UnityEngine;
using Client;

[CreateAssetMenu(fileName = "NewMonster", menuName = "MonstersStorages/Monster", order = 0)]
public class MonsterStorage : ScriptableObject
{
    public Sprite Sprite;
    public int Cost;
    public MonstersID.Value MonsterID;
    public float Damage;
    public float Health;
    public float MoveSpeed;
    public List<MonsterVisualAndAnimations> VisualAndAnimations = new List<MonsterVisualAndAnimations>();
    public GameObject[] Prefabs;
    public ElementalType Elemental;
}

[System.Serializable]
public class MonsterVisualAndAnimations
{
    public string Name = "View for different level";
    public Avatar Avatar;
    public RuntimeAnimatorController RuntimeAnimatorController;
    public GameObject Prefab;
}


