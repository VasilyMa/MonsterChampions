using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

namespace Client
{
    sealed class MonsterSpawnerSystem : IEcsRunSystem
    {        
        readonly EcsWorldInject _world = default;

        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsFilterInject<Inc<MonsterSpawner, ViewComponent>, Exc<DeadTag>> _monsterSpawnerFilter = default;

        readonly EcsPoolInject<MonsterSpawner> _monsterSpawnerPool = default;

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
        readonly EcsPoolInject<DroppingGoldComponent> _droppingGoldPool = default;

        private int _standartGoldValue = 5;

        private int _spawnOnlyFirstMonster = 0;

        public void Run (IEcsSystems systems) // to do ay add any word row for working 
        {
            foreach (var monsterSpawnerEntity in _monsterSpawnerFilter.Value)
            {
                ref var monsterSpawnerComponent = ref _monsterSpawnerPool.Value.Get(monsterSpawnerEntity);
                ref var monsterSpawnerViewComponent = ref _viewPool.Value.Get(monsterSpawnerEntity);

                if (monsterSpawnerComponent.TimerCurrentValue > 0)
                {
                    monsterSpawnerComponent.TimerCurrentValue -= Time.deltaTime;
                    continue;
                }
                else
                {
                    monsterSpawnerComponent.TimerCurrentValue = monsterSpawnerComponent.TimerMaxValue;
                }

                var monsterEntity = _world.Value.NewEntity();

                ref var viewComponent = ref _viewPool.Value.Add(monsterEntity);
                viewComponent.EntityNumber = monsterEntity;

                viewComponent.GameObject = GameObject.Instantiate(_gameState.Value._monsterStorage.MainMonsterPrefab, monsterSpawnerViewComponent.Transform.position, Quaternion.identity); // to do ay write universale system for so more monsters in MonsterStorage
                viewComponent.Transform = viewComponent.GameObject.transform;
                viewComponent.Model = viewComponent.Transform.GetComponentInChildren<UnitModelMB>().gameObject;
                viewComponent.Model = GameObject.Instantiate(monsterSpawnerComponent.MonsterStorage[_spawnOnlyFirstMonster].VisualAndAnimations[monsterSpawnerComponent.MonsterLevel - 1].ModelPrefab, viewComponent.GameObject.transform.position, Quaternion.identity);
                viewComponent.Model.transform.SetParent(viewComponent.Transform);

                ref var fractionComponent = ref _fractionPool.Value.Add(monsterEntity);
                fractionComponent.isFriendly = false;

                ref var physicsComponent = ref _physicsPool.Value.Add(monsterEntity);
                physicsComponent.Rigidbody = viewComponent.GameObject.GetComponent<Rigidbody>();

                ref var animableComponent = ref _animablePool.Value.Add(monsterEntity);
                animableComponent.Animator = viewComponent.GameObject.GetComponent<Animator>();

                animableComponent.Animator.runtimeAnimatorController = monsterSpawnerComponent.MonsterStorage[_spawnOnlyFirstMonster].VisualAndAnimations[monsterSpawnerComponent.MonsterLevel - 1].RuntimeAnimatorController;
                animableComponent.Animator.avatar = monsterSpawnerComponent.MonsterStorage[_spawnOnlyFirstMonster].VisualAndAnimations[monsterSpawnerComponent.MonsterLevel - 1].Avatar;

                ref var unitComponent = ref _unitPool.Value.Add(monsterEntity);

                ref var movableComponent = ref _movablePool.Value.Add(monsterEntity);
                movableComponent.NavMeshAgent = viewComponent.GameObject.GetComponent<NavMeshAgent>();
                movableComponent.NavMeshAgent.speed = monsterSpawnerComponent.MonsterStorage[_spawnOnlyFirstMonster].MoveSpeed;

                viewComponent.EcsInfoMB = viewComponent.GameObject.GetComponent<EcsInfoMB>();
                viewComponent.EcsInfoMB?.Init(_world, monsterEntity);

                ref var targetableComponent = ref _targetablePool.Value.Add(monsterEntity);
                targetableComponent.TargetEntity = BattleState.NULL_ENTITY;
                targetableComponent.EntitysInDetectionZone = new List<int>();
                targetableComponent.EntitysInMeleeZone = new List<int>();
                targetableComponent.EntitysInRangeZone = new List<int>();

                ref var healthComponent = ref _healthPool.Value.Add(monsterEntity);
                healthComponent.MaxValue = monsterSpawnerComponent.MonsterStorage[_spawnOnlyFirstMonster].Health;
                healthComponent.CurrentValue = healthComponent.MaxValue;
                healthComponent.HealthBar = viewComponent.Transform.GetComponentInChildren<HealthBarMB>().gameObject;
                healthComponent.HealthBarMaxWidth = healthComponent.HealthBar.transform.localScale.x;
                healthComponent.HealthBar.SetActive(false);

                ref var elementalComponent = ref _elementalPool.Value.Add(monsterEntity);
                elementalComponent.CurrentType = monsterSpawnerComponent.MonsterStorage[_spawnOnlyFirstMonster].Elemental;

                ref var levelComponent = ref _levelPool.Value.Add(monsterEntity);
                ref var damageComponent = ref _damagePool.Value.Add(monsterEntity);

                levelComponent.Value = monsterSpawnerComponent.MonsterLevel;
                damageComponent.Value = monsterSpawnerComponent.MonsterStorage[_spawnOnlyFirstMonster].Damage;

                ref var droppingGoldComponent = ref _droppingGoldPool.Value.Add(monsterEntity);
                droppingGoldComponent.GoldValue = _standartGoldValue;

                // to do ay write method for different components for monsters
            }
        }
    }
}