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

        readonly EcsSharedInject<BattleState> _battleState;

        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        readonly EcsPoolInject<BaseTag> _enemyBasePool = default;
        readonly EcsPoolInject<HealthComponent> _healthPool = default;
        readonly EcsPoolInject<DisabledBaseTag> _disabledBasePool = default;
        readonly EcsPoolInject<FractionComponent> _fractionPool = default;
        readonly EcsPoolInject<Animable> _animablePool = default;

        public void Init (IEcsSystems systems)
        {
            var allEnemyBasesMB = GameObject.FindObjectsOfType<BaseTagMB>();

            foreach (var enemyBaseMB in allEnemyBasesMB)
            {
                int baseEntity = _world.Value.NewEntity();

                ref var viewComponent = ref _viewPool.Value.Add(baseEntity);
                viewComponent.EntityNumber = baseEntity;

                viewComponent.GameObject = enemyBaseMB.gameObject;
                viewComponent.Transform = viewComponent.GameObject.transform;
                viewComponent.EcsInfoMB = viewComponent.GameObject.GetComponent<EcsInfoMB>();
                viewComponent.EcsInfoMB?.Init(_world, baseEntity);

                ref var enemyBaseComponent = ref _enemyBasePool.Value.Add(baseEntity);

                ref var healthComponent = ref _healthPool.Value.Add(baseEntity);
                healthComponent.MaxValue = 10;
                healthComponent.CurrentValue = healthComponent.MaxValue;

                ref var fractionComponent = ref _fractionPool.Value.Add(baseEntity);
                fractionComponent.isFriendly = enemyBaseMB.isFriendly;

                ref var animableComponent = ref _animablePool.Value.Add(baseEntity);
                animableComponent.Animator = viewComponent.GameObject.GetComponent<Animator>();

                if (fractionComponent.isFriendly)
                {
                    _battleState.Value.SetPlayerBaseEntity(baseEntity);
                }
                else
                {
                    _disabledBasePool.Value.Add(baseEntity);
                    _battleState.Value.AddEnemyBaseEntity(baseEntity);
                }
            }

            _battleState.Value.ActivateNextEnemyBase();
        }
    }
}