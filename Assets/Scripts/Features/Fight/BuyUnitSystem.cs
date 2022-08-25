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

        public void Run (IEcsSystems systems) {
            foreach (var entity in _buyFilter.Value)
            {
                ref var buyInfoComp = ref _buyFilter.Pools.Inc1.Get(entity); //there save are info of new monster buyed buyInfoComp.CardInfo...
                int unitEntity = _world.Value.NewEntity();
                _onboardUnit.Value.Add(unitEntity);
                var slot = FindEmptySlot();
                var unitObject = GameObject.Instantiate(_state.Value._monsterStorage.MainMonsterPrefab, slot.position, Quaternion.identity);
                unitObject.transform.SetParent(slot);

                ref var viewComponent = ref _viewPool.Value.Add(unitEntity);
                viewComponent.EntityNumber = unitEntity;

                viewComponent.GameObject = unitObject;
                viewComponent.Transform = viewComponent.GameObject.transform;
                viewComponent.GameObject.tag = "Friendly";

                viewComponent.CardInfo = buyInfoComp.CardInfo;

                viewComponent.Model = viewComponent.Transform.GetComponentInChildren<UnitModelMB>().gameObject;

                ref var fractionComponent = ref _fractionPool.Value.Add(unitEntity);
                fractionComponent.isFriendly = true;
                viewComponent.GameObject.GetComponent<UnitTagMB>().IsFriendly = true;

                ref var physicsComponent = ref _physicsPool.Value.Add(unitEntity);
                physicsComponent.Rigidbody = viewComponent.GameObject.GetComponent<Rigidbody>();

                ref var animableComponent = ref _animablePool.Value.Add(unitEntity);
                animableComponent.Animator = viewComponent.GameObject.GetComponent<Animator>();

                ref var unitComponent = ref _unitPool.Value.Add(unitEntity);

                ref var movableComponent = ref _movablePool.Value.Add(unitEntity);
                movableComponent.NavMeshAgent = viewComponent.GameObject.GetComponent<NavMeshAgent>();
                movableComponent.NavMeshAgent.speed = buyInfoComp.CardInfo.MoveSpeed;
                movableComponent.NavMeshAgent.enabled = false;
                viewComponent.Transform.position = slot.position;

                viewComponent.EcsInfoMB = viewComponent.GameObject.GetComponent<EcsInfoMB>();
                viewComponent.EcsInfoMB.monsterID = buyInfoComp.CardInfo.MonsterID;
                viewComponent.EcsInfoMB?.Init(_world, unitEntity);

                ref var targetableComponent = ref _targetablePool.Value.Add(unitEntity);
                targetableComponent.TargetEntity = BattleState.NULL_ENTITY;
                targetableComponent.EntitysInDetectionZone = new List<int>();
                targetableComponent.EntitysInMeleeZone = new List<int>();
                targetableComponent.EntitysInRangeZone = new List<int>();

                ref var healthComponent = ref _healthPool.Value.Add(unitEntity);
                healthComponent.MaxValue = buyInfoComp.CardInfo.Health;
                healthComponent.CurrentValue = healthComponent.MaxValue;
                healthComponent.HealthBar = viewComponent.Transform.GetComponentInChildren<HealthBarMB>().gameObject;
                healthComponent.HealthBarMaxWidth = healthComponent.HealthBar.transform.localScale.x;
                healthComponent.HealthBar.SetActive(false);

                ref var elementalComponent = ref _elementalPool.Value.Add(unitEntity);
                elementalComponent.CurrentType = buyInfoComp.CardInfo.Elemental;

                ref var levelComponent = ref _levelPool.Value.Add(unitEntity);
                ref var damageComponent = ref _damagePool.Value.Add(unitEntity);

                levelComponent.Value = 1;
                damageComponent.Value = buyInfoComp.CardInfo.Damage;

                ref var slevComponent = ref _slevPool.Value.Add(unitEntity); // to do ay method for definition unitType
                slevComponent.TimerToCreateAuraMaxValue = 1f;
                slevComponent.TimerToCreateAuraCurrentValue = 0;

                _buyFilter.Pools.Inc1.Del(entity);
            }

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
    }
}