using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace Client
{
    sealed class InitUnits : IEcsInitSystem
    {
        readonly EcsWorldInject _world;

        readonly EcsSharedInject<GameState> _battleState;

        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        readonly EcsPoolInject<Animable> _animablePool = default;
        readonly EcsPoolInject<PhysicsComponent> _physicsPool = default;
        readonly EcsPoolInject<UnitTag> _unitPool = default;
        readonly EcsPoolInject<Movable> _movablePool = default;
        readonly EcsPoolInject<Targetable> _targetablePool = default;
        readonly EcsPoolInject<HealthComponent> _healthPool = default;
        readonly EcsPoolInject<ElementalComponent> _elementalPool = default;
        readonly EcsPoolInject<LevelComponent> _levelPool = default;
        readonly EcsPoolInject<DamageComponent> _damagePool = default;
        readonly EcsPoolInject<FractionComponent> _fractionPool = default;

        public void Init (IEcsSystems systems)
        {
            var allUnitsMB = GameObject.FindObjectsOfType<UnitTagMB>();

            foreach (var unitsMB in allUnitsMB)
            {
                int unitEntity = _world.Value.NewEntity();

                ref var viewComponent = ref _viewPool.Value.Add(unitEntity);
                viewComponent.EntityNumber = unitEntity;

                viewComponent.GameObject = unitsMB.gameObject;
                viewComponent.Transform = viewComponent.GameObject.transform;
                viewComponent.Model = viewComponent.Transform.GetComponentInChildren<UnitModelMB>().gameObject;

                ref var fractionComponent = ref _fractionPool.Value.Add(unitEntity);
                fractionComponent.isFriendly = unitsMB.IsFriendly;

                ref var physicsComponent = ref _physicsPool.Value.Add(unitEntity);
                physicsComponent.Rigidbody = viewComponent.GameObject.GetComponent<Rigidbody>();

                ref var animableComponent = ref _animablePool.Value.Add(unitEntity);
                animableComponent.Animator = viewComponent.GameObject.GetComponent<Animator>();

                ref var unitComponent = ref _unitPool.Value.Add(unitEntity);

                ref var movableComponent = ref _movablePool.Value.Add(unitEntity);
                movableComponent.NavMeshAgent = viewComponent.GameObject.GetComponent<NavMeshAgent>();
                movableComponent.NavMeshAgent.speed = 10;

                viewComponent.EcsInfoMB = viewComponent.GameObject.GetComponent<EcsInfoMB>();
                viewComponent.EcsInfoMB?.Init(_world, unitEntity);

                ref var targetableComponent = ref _targetablePool.Value.Add(unitEntity);
                targetableComponent.TargetEntity = BattleState.NULL_ENTITY;
                targetableComponent.EntitysInDetectionZone = new List<int>();
                targetableComponent.EntitysInMeleeZone = new List<int>();
                targetableComponent.EntitysInRangeZone = new List<int>();

                ref var healthComponent = ref _healthPool.Value.Add(unitEntity);
                healthComponent.MaxValue = 100;
                healthComponent.CurrentValue = healthComponent.MaxValue;

                ref var elementalComponent = ref _elementalPool.Value.Add(unitEntity);
                elementalComponent.CurrentType = ElementalType.Fire;

                ref var levelComponent = ref _levelPool.Value.Add(unitEntity);
                ref var damageComponent = ref _damagePool.Value.Add(unitEntity);

                levelComponent.Value = 1;
                damageComponent.Value = 10;
            }
        }
    }
}