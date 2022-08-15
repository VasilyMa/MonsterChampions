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
        private const int NULL_ENTITY = -1;

        public BattleState(EcsWorld EcsWorld)
        {
            this.EcsWorld = EcsWorld;
        }

        public static void AddEnemyBaseEntity(int entity)
        {
            EnemyBaseEntity.Add(entity);
            Debug.Log($"Добавили базу в BattleState: {entity}");
        }

        public static int GetEnemyBaseEntity()
        {
            if (isExistEnemyBase())
            {
                return EnemyBaseEntity[0];
            }
            else
            {
                return NULL_ENTITY;
            }
        }

        public static bool isNullableBaseEntity(int baseEntity)
        {
            return baseEntity == NULL_ENTITY;
        }

        public static bool isNullableEntity(int entity)
        {
            return entity == NULL_ENTITY;
        }

        public static bool isExistEnemyBase()
        {
            return EnemyBaseEntity.Count > 0;
        }
    }
}
