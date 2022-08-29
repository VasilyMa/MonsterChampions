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
        readonly EcsPoolInject<RangeUnitComponent> _rangeUnitPool = default;

        readonly EcsPoolInject<StoonComponent> _stoonPool = default;
        readonly EcsPoolInject<SlevComponent> _slevPool = default;
        readonly EcsPoolInject<SparkyComponent> _sparkyPool = default;
        readonly EcsPoolInject<TinkiComponent> _tinkiPool = default;
        readonly EcsPoolInject<BableComponent> _bablePool = default;

        private int _monsterEntity = BattleState.NULL_ENTITY;
        private int _monsterSpawnerEntity = BattleState.NULL_ENTITY;

        private int _standartGoldValue = 5;

        private int _spawnOnlyFirstMonster = 0;

        public void Run (IEcsSystems systems)
        {
            foreach (var monsterSpawnerEntity in _monsterSpawnerFilter.Value)
            {
                _monsterSpawnerEntity = monsterSpawnerEntity;

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

                _monsterEntity = _world.Value.NewEntity();

                ref var viewComponent = ref _viewPool.Value.Add(_monsterEntity);
                viewComponent.EntityNumber = _monsterEntity;

                viewComponent.GameObject = GameObject.Instantiate(_gameState.Value._monsterStorage.MainMonsterPrefab, monsterSpawnerViewComponent.Transform.position, Quaternion.identity); // to do ay write universale system for so more monsters in MonsterStorage
                viewComponent.Transform = viewComponent.GameObject.transform;
                viewComponent.Model = viewComponent.Transform.GetComponentInChildren<UnitModelMB>().gameObject;
                viewComponent.Model = GameObject.Instantiate(monsterSpawnerComponent.MonsterStorage[_spawnOnlyFirstMonster].VisualAndAnimations[monsterSpawnerComponent.MonsterLevel - 1].ModelPrefab, viewComponent.GameObject.transform.position, Quaternion.identity);
                viewComponent.Model.transform.SetParent(viewComponent.Transform);
                viewComponent.HealthBarMB = viewComponent.GameObject.GetComponentInChildren<HealthbarMB>();
                viewComponent.HealthBarMB.Init(_world, systems.GetShared<GameState>());
                

                ref var fractionComponent = ref _fractionPool.Value.Add(_monsterEntity);
                fractionComponent.isFriendly = false;

                ref var physicsComponent = ref _physicsPool.Value.Add(_monsterEntity);
                physicsComponent.Rigidbody = viewComponent.GameObject.GetComponent<Rigidbody>();

                ref var animableComponent = ref _animablePool.Value.Add(_monsterEntity);
                animableComponent.Animator = viewComponent.GameObject.GetComponent<Animator>();

                animableComponent.Animator.runtimeAnimatorController = monsterSpawnerComponent.MonsterStorage[_spawnOnlyFirstMonster].VisualAndAnimations[monsterSpawnerComponent.MonsterLevel - 1].RuntimeAnimatorController;
                animableComponent.Animator.avatar = monsterSpawnerComponent.MonsterStorage[_spawnOnlyFirstMonster].VisualAndAnimations[monsterSpawnerComponent.MonsterLevel - 1].Avatar;

                ref var unitComponent = ref _unitPool.Value.Add(_monsterEntity);

                ref var movableComponent = ref _movablePool.Value.Add(_monsterEntity);
                movableComponent.NavMeshAgent = viewComponent.GameObject.GetComponent<NavMeshAgent>();
                movableComponent.NavMeshAgent.speed = monsterSpawnerComponent.MonsterStorage[_spawnOnlyFirstMonster].MoveSpeed;

                viewComponent.EcsInfoMB = viewComponent.GameObject.GetComponent<EcsInfoMB>();
                viewComponent.EcsInfoMB?.Init(_world, _monsterEntity);

                ref var targetableComponent = ref _targetablePool.Value.Add(_monsterEntity);
                targetableComponent.TargetEntity = BattleState.NULL_ENTITY;
                targetableComponent.EntitysInDetectionZone = new List<int>();
                targetableComponent.EntitysInMeleeZone = new List<int>();
                targetableComponent.EntitysInRangeZone = new List<int>();
                targetableComponent.MeleeZone = viewComponent.GameObject.GetComponentInChildren<MeleeZoneMB>().gameObject;
                targetableComponent.RangeZone = viewComponent.GameObject.GetComponentInChildren<RangeZoneMB>().gameObject;

                ref var healthComponent = ref _healthPool.Value.Add(_monsterEntity);
                healthComponent.MaxValue = monsterSpawnerComponent.MonsterStorage[_spawnOnlyFirstMonster].Health;
                healthComponent.CurrentValue = healthComponent.MaxValue;
                healthComponent.HealthBar = viewComponent.Transform.GetComponentInChildren<HealthBarMB>().gameObject;
                healthComponent.HealthBarMaxWidth = healthComponent.HealthBar.transform.localScale.x;
                healthComponent.HealthBar.SetActive(false);
                viewComponent.HealthBarMB.SetMaxHealth(healthComponent.MaxValue);
                viewComponent.HealthBarMB.gameObject.SetActive(true);

                ref var elementalComponent = ref _elementalPool.Value.Add(_monsterEntity);
                elementalComponent.CurrentType = monsterSpawnerComponent.MonsterStorage[_spawnOnlyFirstMonster].Elemental;

                ref var levelComponent = ref _levelPool.Value.Add(_monsterEntity);
                ref var damageComponent = ref _damagePool.Value.Add(_monsterEntity);

                levelComponent.Value = monsterSpawnerComponent.MonsterLevel;
                damageComponent.Value = monsterSpawnerComponent.MonsterStorage[_spawnOnlyFirstMonster].Damage;

                ref var droppingGoldComponent = ref _droppingGoldPool.Value.Add(_monsterEntity);
                droppingGoldComponent.GoldValue = _standartGoldValue;

                AddMonstersSpecificity();

                DisableAttackZonesIfNeed();

                _monsterEntity = BattleState.NULL_ENTITY;
                _monsterSpawnerEntity = BattleState.NULL_ENTITY;
            }
        }

        private void DisableAttackZonesIfNeed()
        {
            ref var targetableComponent = ref _targetablePool.Value.Get(_monsterEntity);

            if (_rangeUnitPool.Value.Has(_monsterEntity))
            {
                targetableComponent.MeleeZone.SetActive(false);
            }
            else
            {
                targetableComponent.RangeZone.SetActive(false);
            }
        }

        private void AddMonstersSpecificity()
        {
            ref var monsterSpawnerComponent = ref _monsterSpawnerPool.Value.Get(_monsterSpawnerEntity); // to do rewrite this, be couse this will do more problem with more different monsters in enemyBaseTag

            switch (monsterSpawnerComponent.MonsterStorage[_spawnOnlyFirstMonster].MonsterID)
            {
                case MonstersID.Value.Default:
                    break;
                case MonstersID.Value.Stoon:
                    StoonsComponents();
                    break;
                case MonstersID.Value.Sparky:
                    SparkysComponents();
                    break;
                case MonstersID.Value.Tinki:
                    TinkisComponents();
                    break;
                case MonstersID.Value.Bable:
                    BablesComponents();
                    break;
                case MonstersID.Value.Slev:
                    SlevsComponents();
                    break;
                default:
                    break;
            }
        }

        private void StoonsComponents()
        {
            ref var stoonComponent = ref _stoonPool.Value.Add(_monsterEntity);
        }

        private void SlevsComponents()
        {
            ref var slevComponent = ref _slevPool.Value.Add(_monsterEntity);
        }

        private void SparkysComponents()
        {
            ref var sparkyComponent = ref _sparkyPool.Value.Add(_monsterEntity);
        }

        private void TinkisComponents()
        {
            ref var tinkiComponent = ref _tinkiPool.Value.Add(_monsterEntity);
            ref var rangeUnitComponent = ref _rangeUnitPool.Value.Add(_monsterEntity);

            ref var viewComponent = ref _viewPool.Value.Get(_monsterEntity);

            rangeUnitComponent.FirePoint = viewComponent.Model.GetComponent<FirePointMB>().GetFirePoint();
        }

        private void BablesComponents()
        {
            ref var bableComponent = ref _bablePool.Value.Add(_monsterEntity);
        }
    }
}