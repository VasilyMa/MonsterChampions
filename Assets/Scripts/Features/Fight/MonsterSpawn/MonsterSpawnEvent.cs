using UnityEngine;
using System.Collections.Generic;

namespace Client
{
    struct MonsterSpawnEvent
    {
        public Vector3 SpawnPoint;
        public Quaternion Direction;
        public bool isFriendly;
        public int Cost;
        public int Level;
        public float Damage;
        public float Health;
        public float MoveSpeed;
        public ElementalType Elemental;
        public MonstersID.Value MonsterID;
        public List<MonsterVisualAndAnimations> VisualAndAnimations;

        public Transform Holder;

        public void Invoke(Vector3 spawnPoint, Quaternion direction, bool isFriendly, int cost, int level, float damage, float health, float moveSpeed, ElementalType elemental, MonstersID.Value monsterID, List<MonsterVisualAndAnimations> visualAndAnimations, Transform holder = null)
        {
            SpawnPoint = spawnPoint;
            Direction = direction;
            this.isFriendly = isFriendly;
            Cost = cost;
            Level = level;
            Damage = damage;
            Health = health;
            MoveSpeed = moveSpeed;
            Elemental = elemental;
            MonsterID = monsterID;
            VisualAndAnimations = visualAndAnimations;

            Holder = holder;
        }
    }
}