using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CreateTinkiThunderboltEventSystem : IEcsRunSystem
    {
        readonly EcsWorldInject _world;

        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsFilterInject<Inc<CreateTinkiThunderboltEvent>> _tinkiThunderboltEventFilter = default;

        readonly EcsPoolInject<CreateTinkiThunderboltEvent> _tinkiThunderboltEventPool = default;
        readonly EcsPoolInject<DamagingEvent> _damagingEventPool = default;

        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        readonly EcsPoolInject<UnitTag> _unitPool = default;
        readonly EcsPoolInject<OnBoardUnitTag> _onBoardUnitPool = default;
        readonly EcsPoolInject<FractionComponent> _fractionPool = default;
        readonly EcsPoolInject<HealthComponent> _healthPool = default;
        readonly EcsPoolInject<DamageComponent> _damagePool = default;
        readonly EcsPoolInject<ElementalComponent> _elementalPool = default;
        readonly EcsPoolInject<LevelComponent> _levelPool = default;
        readonly EcsPoolInject<ThunderboltComponent> _thunderboltPool = default;

        private int _aliveUnitLayer = LayerMask.GetMask(nameof(ViewComponent.AliveUnit));

        private int _tinkiEntity = BattleState.NULL_ENTITY;
        private int _targetEntity = BattleState.NULL_ENTITY;
        private int _thunderboltEntity = BattleState.NULL_ENTITY;

        public void Run(IEcsSystems systems)
        {
            foreach (var eventEntity in _tinkiThunderboltEventFilter.Value)
            {
                ref var tinkiThunderboltEvent = ref _tinkiThunderboltEventPool.Value.Get(eventEntity);

                _tinkiEntity = tinkiThunderboltEvent.TinkiEntity;
                _targetEntity = tinkiThunderboltEvent.TargetEntity;

                ref var fractionComponent = ref _fractionPool.Value.Get(_tinkiEntity);
                ref var healthComponent = ref _healthPool.Value.Get(_tinkiEntity);
                ref var viewComponent = ref _viewPool.Value.Get(_tinkiEntity);

                CreateThunderboltEntity();

                var _allUnitsInThunderboltRange = Physics.OverlapSphere(viewComponent.Transform.position, 10f, _aliveUnitLayer);

                Debug.Log($"����� �������: {_allUnitsInThunderboltRange.Length}");

                foreach (var unitInAura in _allUnitsInThunderboltRange)
                {
                    var unitEcsInfoMB = unitInAura.GetComponent<EcsInfoMB>();
                    var unitEntity = unitEcsInfoMB.GetEntity();

                    if (unitEntity == _targetEntity)
                    {
                        continue;
                    }

                    if (!_unitPool.Value.Has(unitEntity))
                    {
                        continue;
                    }

                    if (_onBoardUnitPool.Value.Has(unitEntity))
                    {
                        continue;
                    }

                    ref var unitFractionComponent = ref _fractionPool.Value.Get(unitEntity);

                    if (unitFractionComponent.isFriendly != fractionComponent.isFriendly)
                    {
                        InvokeDamageFromThunderbolt(unitEntity);
                    }
                }

                ref var thunderboltComponent = ref _thunderboltPool.Value.Get(_thunderboltEntity);
                thunderboltComponent.isCausedDamage = true;

                DeleteEvent(eventEntity);
            }
        }

        private void InvokeDamageFromThunderbolt(int undergoEntity)
        {
            ref var thunderboltDamageComponent = ref _damagePool.Value.Get(_thunderboltEntity);

            _damagingEventPool.Value.Add(_world.Value.NewEntity()).Invoke(undergoEntity, _thunderboltEntity, thunderboltDamageComponent.Value);
        }

        private void DeleteEvent(int eventEntity)
        {
            _tinkiThunderboltEventPool.Value.Del(eventEntity);

            _tinkiEntity = BattleState.NULL_ENTITY;
            _targetEntity = BattleState.NULL_ENTITY;
            _thunderboltEntity = BattleState.NULL_ENTITY;
        }

        private void CreateThunderboltEntity()
        {
            _thunderboltEntity = _world.Value.NewEntity();

            ref var thunderboltComponent = ref _thunderboltPool.Value.Add(_thunderboltEntity);
            thunderboltComponent.isCausedDamage = false;

            ref var thunderboltElementalComponent = ref _elementalPool.Value.Add(_thunderboltEntity);
            thunderboltElementalComponent.CurrentType = ElementalType.Air;

            ref var tinkiDamageComponent = ref _damagePool.Value.Get(_tinkiEntity);

            ref var thunderboltDamageComponent = ref _damagePool.Value.Add(_thunderboltEntity);
            thunderboltDamageComponent.Value = Mathf.Round(tinkiDamageComponent.Value / 2);

            ref var levelComponent = ref _levelPool.Value.Add(_thunderboltEntity);
            levelComponent.Value = _levelPool.Value.Get(_tinkiEntity).Value;
        }
    }
}