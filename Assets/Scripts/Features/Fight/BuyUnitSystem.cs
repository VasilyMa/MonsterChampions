using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace Client {
    sealed class BuyUnitSystem : IEcsRunSystem 
    {
        readonly EcsWorldInject _world = default;
        readonly EcsSharedInject<GameState> _state = default;
        readonly EcsFilterInject<Inc<BuyUnitEvent>> _buyFilter = default;
        readonly EcsPoolInject<OnBoardUnitTag> _onboardUnit = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        readonly EcsPoolInject<PhysicsComponent> _physicsPool = default;
        readonly EcsPoolInject<UnitTag> _unitPool = default;
        readonly EcsPoolInject<Movable> _movablePool = default;
        readonly EcsPoolInject<Targetable> _targetablePool = default;
        readonly EcsPoolInject<HealthComponent> _healthPool = default;
        readonly EcsPoolInject<ElementalComponent> _elementalPool = default;
        readonly EcsPoolInject<LevelComponent> _levelPool = default;
        readonly EcsPoolInject<DamageComponent> _damagePool = default;

        readonly EcsPoolInject<FractionComponent> _fractionPool = default;
        readonly EcsPoolInject<Animable> _animablePool = default;

        readonly EcsPoolInject<SlevComponent> _slevPool = default;
        readonly EcsPoolInject<SparkyComponent> _sparkyPool = default;
        readonly EcsPoolInject<StoonComponent> _stoonPool = default;
        readonly EcsPoolInject<TinkiComponent> _tinkiPool = default;
        readonly EcsPoolInject<BableComponent> _bablePool = default;

        private int _unitEntity = BattleState.NULL_ENTITY;

        public void Run (IEcsSystems systems)
        {
            foreach (var entity in _buyFilter.Value)
            {
                ref var buyInfoComp = ref _buyFilter.Pools.Inc1.Get(entity); //there save are info of new monster buyed buyInfoComp.CardInfo...
                _unitEntity = _world.Value.NewEntity();
                _onboardUnit.Value.Add(_unitEntity);
                var slot = FindEmptySlot();
                var unitObject = GameObject.Instantiate(_state.Value._monsterStorage.MainMonsterPrefab, slot.position, Quaternion.identity);
                unitObject.transform.SetParent(slot);

                ref var viewComponent = ref _viewPool.Value.Add(_unitEntity);
                viewComponent.EntityNumber = _unitEntity;

                viewComponent.GameObject = unitObject;
                viewComponent.Transform = viewComponent.GameObject.transform;
                viewComponent.GameObject.tag = "Friendly";
                viewComponent.HealthBarMB = viewComponent.GameObject.transform.GetChild(4).GetComponent<HealthbarMB>();
                viewComponent.HealthBarMB.Init(_world, systems.GetShared<GameState>());

                viewComponent.CardInfo = buyInfoComp.CardInfo;

                viewComponent.Model = viewComponent.Transform.GetComponentInChildren<UnitModelMB>().gameObject;
                viewComponent.Model = GameObject.Instantiate(viewComponent.CardInfo.VisualAndAnimations[0].ModelPrefab, viewComponent.GameObject.transform.position, Quaternion.identity);
                viewComponent.Model.transform.SetParent(viewComponent.Transform);

                SetActualModelView(); // to do ay write this method

                ref var fractionComponent = ref _fractionPool.Value.Add(_unitEntity);
                fractionComponent.isFriendly = true;
                viewComponent.GameObject.GetComponent<UnitTagMB>().IsFriendly = true;

                ref var physicsComponent = ref _physicsPool.Value.Add(_unitEntity);
                physicsComponent.Rigidbody = viewComponent.GameObject.GetComponent<Rigidbody>();

                ref var animableComponent = ref _animablePool.Value.Add(_unitEntity);
                animableComponent.Animator = viewComponent.GameObject.GetComponent<Animator>();

                animableComponent.Animator.runtimeAnimatorController = viewComponent.CardInfo.VisualAndAnimations[0].RuntimeAnimatorController;
                animableComponent.Animator.avatar = viewComponent.CardInfo.VisualAndAnimations[0].Avatar;

                ref var unitComponent = ref _unitPool.Value.Add(_unitEntity);

                ref var movableComponent = ref _movablePool.Value.Add(_unitEntity);
                movableComponent.NavMeshAgent = viewComponent.GameObject.GetComponent<NavMeshAgent>();
                movableComponent.NavMeshAgent.speed = buyInfoComp.CardInfo.MoveSpeed;
                movableComponent.NavMeshAgent.enabled = false;
                viewComponent.Transform.position = slot.position;

                viewComponent.EcsInfoMB = viewComponent.GameObject.GetComponent<EcsInfoMB>();
                viewComponent.EcsInfoMB.monsterID = buyInfoComp.CardInfo.MonsterID;
                viewComponent.EcsInfoMB?.Init(_world, _unitEntity);

                ref var targetableComponent = ref _targetablePool.Value.Add(_unitEntity);
                targetableComponent.TargetEntity = BattleState.NULL_ENTITY;
                targetableComponent.EntitysInDetectionZone = new List<int>();
                targetableComponent.EntitysInMeleeZone = new List<int>();
                targetableComponent.EntitysInRangeZone = new List<int>();

                ref var healthComponent = ref _healthPool.Value.Add(_unitEntity);
                healthComponent.MaxValue = buyInfoComp.CardInfo.Health;
                healthComponent.CurrentValue = healthComponent.MaxValue;
                healthComponent.HealthBar = viewComponent.Transform.GetComponentInChildren<HealthBarMB>().gameObject;
                healthComponent.HealthBarMaxWidth = healthComponent.HealthBar.transform.localScale.x;
                healthComponent.HealthBar.SetActive(false);

                viewComponent.HealthBarMB.SetMaxHealth(healthComponent.MaxValue);
                viewComponent.HealthBarMB.gameObject.SetActive(false);

                ref var elementalComponent = ref _elementalPool.Value.Add(_unitEntity);
                elementalComponent.CurrentType = buyInfoComp.CardInfo.Elemental;

                ref var levelComponent = ref _levelPool.Value.Add(_unitEntity);
                ref var damageComponent = ref _damagePool.Value.Add(_unitEntity);

                levelComponent.Value = 1;
                damageComponent.Value = buyInfoComp.CardInfo.Damage;

                AddMonstersSpecificity();

                

                _unitEntity = BattleState.NULL_ENTITY;
                _buyFilter.Pools.Inc1.Del(entity);
            }

        }

        private void SetActualModelView()
        {

        }

        private Transform FindEmptySlot() //find the empty slot on board for buy unit and add it
        {
            Transform slot = null;
            for (int i = 0; i < _viewPool.Value.Get(_state.Value.BoardEntity).GameObject.transform.childCount; i++)
            {
                slot = _viewPool.Value.Get(_state.Value.BoardEntity).GameObject.transform.GetChild(i);
                if (slot.childCount >= 1)
                {
                    continue;
                }
                else
                {
                    return slot;
                }
            }
            return slot;
        }

        private void AddMonstersSpecificity()
        {
            ref var viewComponent = ref _viewPool.Value.Get(_unitEntity);

            switch (viewComponent.CardInfo.MonsterID)
            {
                case MonstersID.Value.Default:
                    Debug.Log($"Monster {viewComponent.GameObject} have Default MonsterID.");
                    break;
                case MonstersID.Value.Stoon:
                    StoonComponents();
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
                    Debug.Log($"Monster {viewComponent.GameObject} have Unknown MonsterID or dont have it.");
                    break;
            }
        }

        private void StoonComponents()
        {
            ref var stoonComponent = ref _stoonPool.Value.Add(_unitEntity);
        }

        private void SlevsComponents()
        {
            ref var slevComponent = ref _slevPool.Value.Add(_unitEntity);
        }

        private void SparkysComponents()
        {
            ref var sparkyComponent = ref _sparkyPool.Value.Add(_unitEntity);
        }

        private void TinkisComponents()
        {
            ref var tinkiComponent = ref _tinkiPool.Value.Add(_unitEntity);
        }

        private void BablesComponents()
        {
            ref var bableComponent = ref _bablePool.Value.Add(_unitEntity);
        }
    }
}