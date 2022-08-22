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
        public int BoardEntity { get; set; }

        public PlayableDeck PlayableDeck = new PlayableDeck();
        private int _playerBaseEntity = -1;
        private List<int> _enemyBaseEntity = new List<int>();
        private int _currentActiveEnemyBaseInArray = -1;
        public const int NULL_ENTITY = -1;

        public BattleState(EcsWorld EcsWorld)
        {
            this.EcsWorld = EcsWorld;
        }

        public void SetPlayerBaseEntity(int baseEntity)
        {
            if (baseEntity > NULL_ENTITY)
            {
                _playerBaseEntity = baseEntity;
            }
            else
            {
                _playerBaseEntity = NULL_ENTITY;
                Debug.LogError("Write incorrect number for PlayerBaseEntity. Value was match to -1");
            }
        }

        public int GetPlayerBaseEntity()
        {
            if (_playerBaseEntity == NULL_ENTITY) Debug.LogError("Get nullable value (-1) for PlayerBaseEntity.");
            return _playerBaseEntity;
        }

        public void AddEnemyBaseEntity(int entity)
        {
            _enemyBaseEntity.Add(entity);
            Debug.Log($"Добавили базу в BattleState: {entity}");
        }

        public int GetEnemyBaseEntity()
        {
            if (_enemyBaseEntity.Count > 0)
            {
                return _enemyBaseEntity[0];
            }
            else
            {
                return NULL_ENTITY;
            }
        }

        public void ActivateNextEnemyBase()
        {
            _currentActiveEnemyBaseInArray++;
            EcsWorld.GetPool<ActivateEnemyBaseEvent>().Add(_enemyBaseEntity[_currentActiveEnemyBaseInArray]);
        }

        public static bool isNullableEntity(int entity)
        {
            return entity == NULL_ENTITY;
        }
    }
}
