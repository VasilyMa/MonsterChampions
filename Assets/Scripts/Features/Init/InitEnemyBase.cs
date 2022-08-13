using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace Client
{
    sealed class InitEnemyBase : IEcsInitSystem
    {
        readonly EcsWorldInject _world;

        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        readonly EcsPoolInject<BaseTag> _enemyBasePool = default;
        readonly EcsPoolInject<HealthComponent> _healthPool = default;

        public void Init (IEcsSystems systems)
        {
            var allEnemyBasesMB = GameObject.FindObjectsOfType<EnemyBaseTagMB>();

            foreach (var enemyBaseMB in allEnemyBasesMB)
            {
                int enemyBaseEntity = _world.Value.NewEntity();

                BattleState.AddEnemyBaseEntity(enemyBaseEntity);

                ref var viewComponent = ref _viewPool.Value.Add(enemyBaseEntity);
                viewComponent.EntityNumber = enemyBaseEntity;

                viewComponent.GameObject = enemyBaseMB.gameObject;
                viewComponent.Transform = viewComponent.GameObject.transform;
                viewComponent.EcsInfoMB = viewComponent.GameObject.GetComponent<EcsInfoMB>();
                viewComponent.EcsInfoMB?.Init(_world, enemyBaseEntity);

                ref var enemyBaseComponent = ref _enemyBasePool.Value.Add(enemyBaseEntity);

                ref var healthComponent = ref _healthPool.Value.Add(enemyBaseEntity);
                healthComponent.MaxValue = 10;
                healthComponent.CurrentValue = healthComponent.MaxValue;
            }
        }
    }
}