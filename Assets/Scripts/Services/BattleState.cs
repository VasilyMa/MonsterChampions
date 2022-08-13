using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    public class BattleState
    {
        public EcsWorld EcsWorld { get; set; }

        private static List<int> EnemyBaseEntity = new List<int>();
        private static int _nullEnemyBaseEntity = -1;

        public BattleState(EcsWorld EcsWorld)
        {
            this.EcsWorld = EcsWorld;
        }

        public static void AddEnemyBaseEntity(int entity)
        {
            EnemyBaseEntity.Add(entity);
            Debug.Log($"Добавили {entity}");
        }

        public static int GetEnemyBaseEntity()
        {
            if (isExistEnemyBase())
            {
                return EnemyBaseEntity[0];
            }
            else
            {
                return _nullEnemyBaseEntity;
            }
        }

        public static bool isExistEnemyBase()
        {
            return EnemyBaseEntity.Count > 0;
        }
    }
}
