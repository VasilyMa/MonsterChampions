using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

namespace Client
{
    sealed class MonsterSpawnEventSystem : IEcsRunSystem
    {
        readonly EcsWorldInject _world = default;

        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsFilterInject<Inc<MonsterSpawnEvent>> _monsterSpawnEventFilter = default;

        readonly EcsPoolInject<MonsterSpawnEvent> _monsterSpawnEventPool = default;

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
        readonly EcsPoolInject<OnBoardUnitTag> _onboardUnit = default;

        readonly EcsPoolInject<StoonComponent> _stoonPool = default;
        readonly EcsPoolInject<SlevComponent> _slevPool = default;
        readonly EcsPoolInject<SparkyComponent> _sparkyPool = default;
        readonly EcsPoolInject<TinkiComponent> _tinkiPool = default;
        readonly EcsPoolInject<BableComponent> _bablePool = default;

        private int _monsterSpawnEventEntity = BattleState.NULL_ENTITY;
        private int _monsterEntity = BattleState.NULL_ENTITY;

        public void Run (IEcsSystems systems)
        {
            foreach (var monsterSpawnEventEntity in _monsterSpawnEventFilter.Value)
            {
                _monsterSpawnEventEntity = monsterSpawnEventEntity;

                ref var monsterSpawnEvent = ref _monsterSpawnEventPool.Value.Get(_monsterSpawnEventEntity);

                _monsterEntity = _world.Value.NewEntity();

                ref var viewComponent = ref _viewPool.Value.Add(_monsterEntity);
                viewComponent.EntityNumber = _monsterEntity;

                ref var levelComponent = ref _levelPool.Value.Add(_monsterEntity);
                levelComponent.Value = monsterSpawnEvent.Level;

                viewComponent.GameObject = GameObject.Instantiate(_gameState.Value._monsterStorage.MainMonsterPrefab, monsterSpawnEvent.SpawnPoint, monsterSpawnEvent.Direction);
                viewComponent.Transform = viewComponent.GameObject.transform;

                viewComponent.Model = viewComponent.Transform.GetComponentInChildren<UnitModelMB>().gameObject;
                viewComponent.Model = GameObject.Instantiate(monsterSpawnEvent.VisualAndAnimations[levelComponent.Value - 1].ModelPrefab, viewComponent.GameObject.transform.position, monsterSpawnEvent.Direction);
                viewComponent.Model.transform.SetParent(viewComponent.Transform);
                viewComponent.VisualAndAnimations = monsterSpawnEvent.VisualAndAnimations;

                viewComponent.HealthBarMB = viewComponent.GameObject.GetComponentInChildren<HealthbarMB>();
                viewComponent.HealthBarMB.Init(_world, systems.GetShared<GameState>());

                viewComponent.EcsInfoMB = viewComponent.GameObject.GetComponent<EcsInfoMB>();
                viewComponent.EcsInfoMB?.Init(_world, _monsterEntity);

                ref var fractionComponent = ref _fractionPool.Value.Add(_monsterEntity);
                fractionComponent.isFriendly = monsterSpawnEvent.isFriendly;

                ref var physicsComponent = ref _physicsPool.Value.Add(_monsterEntity);
                physicsComponent.Rigidbody = viewComponent.GameObject.GetComponent<Rigidbody>();

                ref var animableComponent = ref _animablePool.Value.Add(_monsterEntity);
                animableComponent.Animator = viewComponent.GameObject.GetComponent<Animator>();

                animableComponent.Animator.runtimeAnimatorController = monsterSpawnEvent.VisualAndAnimations[levelComponent.Value - 1].RuntimeAnimatorController;
                animableComponent.Animator.avatar = monsterSpawnEvent.VisualAndAnimations[levelComponent.Value - 1].Avatar;

                ref var unitComponent = ref _unitPool.Value.Add(_monsterEntity);

                ref var movableComponent = ref _movablePool.Value.Add(_monsterEntity);
                movableComponent.NavMeshAgent = viewComponent.GameObject.GetComponent<NavMeshAgent>();
                movableComponent.NavMeshAgent.speed = monsterSpawnEvent.MoveSpeed;

                ref var targetableComponent = ref _targetablePool.Value.Add(_monsterEntity);
                targetableComponent.TargetEntity = BattleState.NULL_ENTITY;
                targetableComponent.EntitysInDetectionZone = new List<int>();
                targetableComponent.EntitysInMeleeZone = new List<int>();
                targetableComponent.EntitysInRangeZone = new List<int>();
                targetableComponent.MeleeZone = viewComponent.GameObject.GetComponentInChildren<MeleeZoneMB>().gameObject;
                targetableComponent.RangeZone = viewComponent.GameObject.GetComponentInChildren<RangeZoneMB>().gameObject;

                ref var healthComponent = ref _healthPool.Value.Add(_monsterEntity);
                healthComponent.MaxValue = monsterSpawnEvent.Health * Mathf.Pow(2, levelComponent.Value - 1);
                healthComponent.CurrentValue = healthComponent.MaxValue;
                viewComponent.HealthBarMB.SetMaxHealth(healthComponent.MaxValue);
                viewComponent.HealthBarMB.gameObject.SetActive(true);

                ref var elementalComponent = ref _elementalPool.Value.Add(_monsterEntity);
                elementalComponent.CurrentType = monsterSpawnEvent.Elemental;

                ref var damageComponent = ref _damagePool.Value.Add(_monsterEntity);
                damageComponent.Value = monsterSpawnEvent.Damage * Mathf.Pow(2, levelComponent.Value - 1);

                ref var droppingGoldComponent = ref _droppingGoldPool.Value.Add(_monsterEntity);
                droppingGoldComponent.GoldValue = monsterSpawnEvent.Cost * Mathf.RoundToInt(Mathf.Pow(2, levelComponent.Value - 1));

                if (fractionComponent.isFriendly)
                {
                    _onboardUnit.Value.Add(_monsterEntity);

                    viewComponent.Transform.SetParent(monsterSpawnEvent.Holder);
                    viewComponent.Transform.rotation = monsterSpawnEvent.Holder.rotation;

                    viewComponent.GameObject.tag = "Friendly";
                    viewComponent.GameObject.GetComponent<UnitTagMB>().IsFriendly = true;
                    viewComponent.GameObject.layer = LayerMask.NameToLayer(nameof(viewComponent.OnBoardUnit));
                    viewComponent.HealthBarMB.SetMaxHealth(healthComponent.MaxValue);

                    movableComponent.NavMeshAgent.enabled = false;

                    if (Tutorial.CurrentStage == Tutorial.Stage.TwoBuysMonsters)
                    {
                        Tutorial.TwoBuysMonsters.AddSpawn();
                    }
                }

                AddMonstersSpecificity();

                DisableAttackZonesIfNeed();

                DeleteEvent();
            }
        }

        private void AddMonstersSpecificity()
        {
            ref var monsterSpawnEvent = ref _monsterSpawnEventPool.Value.Get(_monsterSpawnEventEntity); // to do rewrite this, be couse this will do more problem with more different monsters in enemyBaseTag

            switch (monsterSpawnEvent.MonsterID)
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

        private void DeleteEvent()
        {
            _monsterSpawnEventPool.Value.Del(_monsterSpawnEventEntity);

            _monsterSpawnEventEntity = BattleState.NULL_ENTITY;
            _monsterEntity = BattleState.NULL_ENTITY;
    }
    }
}