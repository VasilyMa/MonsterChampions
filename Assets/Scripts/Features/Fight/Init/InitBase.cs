using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace Client
{
    sealed class InitBase : IEcsInitSystem
    {
        readonly EcsWorldInject _world;

        readonly EcsSharedInject<GameState> _battleState;

        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        readonly EcsPoolInject<BaseTag> _enemyBasePool = default;
        readonly EcsPoolInject<HealthComponent> _healthPool = default;
        readonly EcsPoolInject<DisabledBaseTag> _disabledBasePool = default;
        readonly EcsPoolInject<FractionComponent> _fractionPool = default;
        readonly EcsPoolInject<Animable> _animablePool = default;
        readonly EcsPoolInject<MonsterSpawner> _monsterSpawnerPool = default;
        readonly EcsPoolInject<GoldAddingComponent> _goldAddingPool = default;
        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;

        public void Init (IEcsSystems systems)
        {
            var allBasesMB = GameObject.FindObjectsOfType<BaseTagMB>();

            foreach (var baseMB in allBasesMB)
            {
                int baseEntity = _world.Value.NewEntity();

                ref var viewComponent = ref _viewPool.Value.Add(baseEntity);
                viewComponent.EntityNumber = baseEntity;

                viewComponent.GameObject = baseMB.gameObject;
                viewComponent.Transform = viewComponent.GameObject.transform;
                viewComponent.EcsInfoMB = viewComponent.GameObject.GetComponent<EcsInfoMB>();
                viewComponent.EcsInfoMB?.Init(_world, baseEntity);

                ref var enemyBaseComponent = ref _enemyBasePool.Value.Add(baseEntity);

                ref var healthComponent = ref _healthPool.Value.Add(baseEntity);
                healthComponent.MaxValue = baseMB.BaseHealth;
                healthComponent.CurrentValue = healthComponent.MaxValue;

                ref var fractionComponent = ref _fractionPool.Value.Add(baseEntity);
                fractionComponent.isFriendly = baseMB.isFriendly;

                ref var animableComponent = ref _animablePool.Value.Add(baseEntity);
                animableComponent.Animator = viewComponent.GameObject.GetComponent<Animator>();

                ref var goldAddingComponent = ref _goldAddingPool.Value.Add(baseEntity);
                goldAddingComponent.Modifier = baseMB.GoldAddingModifier;

                if (fractionComponent.isFriendly)
                {
                    _battleState.Value.SetPlayerBaseEntity(baseEntity);
                    viewComponent.Model = viewComponent.GameObject.GetComponentInChildren<BaseModelMB>().gameObject;
                }
                else
                {
                    _disabledBasePool.Value.Add(baseEntity);
                    _battleState.Value.AddEnemyBaseEntity(baseEntity);
                }

                if (baseMB.MonstersSquads.Count <= 0) // to do rewrite thin in method
                {
                    continue;
                }

                ref var monsterSpawner = ref _monsterSpawnerPool.Value.Add(baseEntity);
                monsterSpawner.MonsterSpawnerInfo = baseMB;
                monsterSpawner.ActualSquad = 0;

            }
            ref var ourHealth = ref _healthPool.Value.Get(_battleState.Value.GetPlayerBaseEntity());
            ref var enemyHealth = ref _healthPool.Value.Get(_battleState.Value.GetEnemyBaseEntity());

            ref var interfaceComp = ref _interfacePool.Value.Get(_battleState.Value.InterfaceEntity);
            interfaceComp.Progress.UpdateHealth(ourHealth.CurrentValue, enemyHealth.CurrentValue);

            if (allBasesMB.Length > 0)
                _battleState.Value.ActivateNextEnemyBase();
        }
    }
}