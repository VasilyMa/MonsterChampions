using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "GetMonster", menuName = "MonstersStorages/get")]
public class GetMonster : ScriptableObject
{
    public GameObject MainMonsterPrefab;

    [Header("MonsterElement == MonsterID")]
    public MonsterStorage[] monster;
}
