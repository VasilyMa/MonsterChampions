using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

namespace Client
{
    sealed class EnemySpawnerSystem : IEcsRunSystem
    {        
        readonly EcsWorldInject _world = default;

        readonly EcsFilterInject<Inc<UnitSpawner, ViewComponent>, Exc<DeadTag>> _unitSpawnerFilter = default;

        readonly EcsPoolInject<UnitSpawner> _unitSpawnerPool = default;

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

        public void Run (IEcsSystems systems) // to do ay add any word row for working 
        {
            foreach (var unitSpawnerEntity in _unitSpawnerFilter.Value)
            {
                ref var unitSpawnerComponent = ref _unitSpawnerPool.Value.Get(unitSpawnerEntity);
                ref var unitSpawnerViewComponent = ref _viewPool.Value.Get(unitSpawnerEntity);

                if (unitSpawnerComponent.TimerCurrentValue > 0)
                {
                    unitSpawnerComponent.TimerCurrentValue -= Time.deltaTime;
                    continue;
                }
                else
                {
                    unitSpawnerComponent.TimerCurrentValue = unitSpawnerComponent.TimerMaxValue;
                }

                var unitEntity = _world.Value.NewEntity();

                ref var viewComponent = ref _viewPool.Value.Add(unitEntity);
                viewComponent.EntityNumber = unitEntity;

                viewComponent.GameObject = GameObject.Instantiate(unitSpawnerComponent.MonsterStorage[0].Prefabs[0], unitSpawnerViewComponent.Transform.position, Quaternion.identity); // to do write versatile system for so more monsters in MonsterStorage
                viewComponent.Transform = viewComponent.GameObject.transform;
                viewComponent.Model = viewComponent.Transform.GetComponentInChildren<UnitModelMB>().gameObject;

                ref var fractionComponent = ref _fractionPool.Value.Add(unitEntity);
                fractionComponent.isFriendly = false;

                ref var physicsComponent = ref _physicsPool.Value.Add(unitEntity);
                physicsComponent.Rigidbody = viewComponent.GameObject.GetComponent<Rigidbody>();

                ref var animableComponent = ref _animablePool.Value.Add(unitEntity);
                animableComponent.Animator = viewComponent.GameObject.GetComponent<Animator>();

                ref var unitComponent = ref _unitPool.Value.Add(unitEntity);

                ref var movableComponent = ref _movablePool.Value.Add(unitEntity);
                movableComponent.NavMeshAgent = viewComponent.GameObject.GetComponent<NavMeshAgent>();
                movableComponent.NavMeshAgent.speed = unitSpawnerComponent.MonsterStorage[0].MoveSpeed;

                viewComponent.EcsInfoMB = viewComponent.GameObject.GetComponent<EcsInfoMB>();
                viewComponent.EcsInfoMB?.Init(_world, unitEntity);

                ref var targetableComponent = ref _targetablePool.Value.Add(unitEntity);
                targetableComponent.TargetEntity = BattleState.NULL_ENTITY;
                targetableComponent.EntitysInDetectionZone = new List<int>();
                targetableComponent.EntitysInMeleeZone = new List<int>();
                targetableComponent.EntitysInRangeZone = new List<int>();

                ref var healthComponent = ref _healthPool.Value.Add(unitEntity);
                healthComponent.MaxValue = unitSpawnerComponent.MonsterStorage[0].Health;
                healthComponent.CurrentValue = healthComponent.MaxValue;
                healthComponent.HealthBar = viewComponent.Transform.GetComponentInChildren<HealthBarMB>().gameObject;
                healthComponent.HealthBarMaxWidth = healthComponent.HealthBar.transform.localScale.x;
                healthComponent.HealthBar.SetActive(false);

                ref var elementalComponent = ref _elementalPool.Value.Add(unitEntity);
                elementalComponent.CurrentType = unitSpawnerComponent.MonsterStorage[0].Elemental;

                ref var levelComponent = ref _levelPool.Value.Add(unitEntity);
                ref var damageComponent = ref _damagePool.Value.Add(unitEntity);

                levelComponent.Value = 1;
                damageComponent.Value = unitSpawnerComponent.MonsterStorage[0].Damage;
            }
        }
    }
}