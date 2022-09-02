using UnityEngine;

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
        public MonsterVisualAndAnimations VisualAndAnimations;
    }
}