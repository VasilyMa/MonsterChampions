using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CreateSparkyExplosionEventSystem : IEcsRunSystem
    {
        readonly EcsWorldInject _world;

        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsFilterInject<Inc<CreateSparkyExplosionEvent>> _sparkyExplosionEventFilter = default;

        readonly EcsPoolInject<CreateSparkyExplosionEvent> _sparkyExplosionEventPool = default;
        readonly EcsPoolInject<DamagingEvent> _damagingEventPool = default;

        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        readonly EcsPoolInject<UnitTag> _unitPool = default;
        readonly EcsPoolInject<OnBoardUnitTag> _onBoardUnitPool = default;
        readonly EcsPoolInject<FractionComponent> _fractionPool = default;
        readonly EcsPoolInject<HealthComponent> _healthPool = default;
        readonly EcsPoolInject<DamageComponent> _damagePool = default;
        readonly EcsPoolInject<ElementalComponent> _elementalPool = default;
        readonly EcsPoolInject<LevelComponent> _levelPool = default;
        readonly EcsPoolInject<ExplosionComponent> _explosionPool = default;

        private int _aliveUnitLayer = LayerMask.NameToLayer(nameof(ViewComponent.AliveUnit));

        private float _explosionRange = 10f;

        private int _sparkyEntity = BattleState.NULL_ENTITY;
        private int _explosionEntity = BattleState.NULL_ENTITY;

        public void Run (IEcsSystems systems)
        {
            foreach (var eventEntity in _sparkyExplosionEventFilter.Value)
            {
                ref var sparkyExplosionEvent = ref _sparkyExplosionEventPool.Value.Get(eventEntity);

                _sparkyEntity = sparkyExplosionEvent.SparkyEntity;

                ref var fractionComponent = ref _fractionPool.Value.Get(_sparkyEntity);
                ref var healthComponent = ref _healthPool.Value.Get(_sparkyEntity);
                ref var viewComponent = ref _viewPool.Value.Get(_sparkyEntity);

                GameObject.Instantiate(_gameState.Value.EffectsPool.MonstersEffects.SparkyExplosion, viewComponent.GameObject.transform.position, Quaternion.identity);

                CreateExplosionEntity();

                var _allUnitsInExplosion = Physics.OverlapSphere(viewComponent.Transform.position, _explosionRange);

                Debug.Log($"Всего найдено: {_allUnitsInExplosion.Length}");

                foreach (var unitInAura in _allUnitsInExplosion)
                {
                    if (unitInAura.gameObject.layer != _aliveUnitLayer)
                    {
                        continue;
                    }

                    var unitEcsInfoMB = unitInAura.GetComponent<EcsInfoMB>();
                    var unitEntity = unitEcsInfoMB.GetEntity();
                    Debug.Log("Заход");
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
                        InvokeDamageFromExplosion(unitEntity);
                    }
                }

                InvokeDamageFromExplosion(_sparkyEntity);

                ref var explosionComponent = ref _explosionPool.Value.Get(_explosionEntity);
                explosionComponent.OwnerEntity = _sparkyEntity;
                explosionComponent.isCausedDamage = true;

                DeleteEvent(eventEntity);
            }
        }

        private void DeleteEvent(int eventEntity)
        {
            _sparkyExplosionEventPool.Value.Del(eventEntity);

            _sparkyEntity = BattleState.NULL_ENTITY;
            _explosionEntity = BattleState.NULL_ENTITY;
        }

        private void InvokeDamageFromExplosion(int undergoEntity)
        {
            ref var explosionDamageComponent = ref _damagePool.Value.Get(_explosionEntity);

            _damagingEventPool.Value.Add(_world.Value.NewEntity()).Invoke(undergoEntity, _explosionEntity, explosionDamageComponent.Value);
        }

        private void CreateExplosionEntity()
        {
            _explosionEntity = _world.Value.NewEntity();

            ref var explosionComponent = ref _explosionPool.Value.Add(_explosionEntity);
            explosionComponent.isCausedDamage = false;

            ref var explosionElementalComponent = ref _elementalPool.Value.Add(_explosionEntity);
            explosionElementalComponent.CurrentType = ElementalType.Fire;

            ref var sparkyHealthCompnent = ref _healthPool.Value.Get(_sparkyEntity);

            ref var explosionDamageComponent = ref _damagePool.Value.Add(_explosionEntity);
            explosionDamageComponent.Value = sparkyHealthCompnent.MaxValue;

            ref var levelComponent = ref _levelPool.Value.Add(_explosionEntity);
            levelComponent.Value = _levelPool.Value.Get(_sparkyEntity).Value;
        }
    }
}