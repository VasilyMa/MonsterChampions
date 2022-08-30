using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CreateBableProtectionAuraSystem : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _gameState = default;

        readonly EcsFilterInject<Inc<BableComponent, ViewComponent>, Exc<DeadTag, OnBoardUnitTag>> _bableFilter = default;

        readonly EcsPoolInject<BableComponent> _bablePool = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        readonly EcsPoolInject<FractionComponent> _fractionPool = default;
        readonly EcsPoolInject<UnitTag> _unitPool = default;
        readonly EcsPoolInject<OnBoardUnitTag> _onBoardUnitPool = default;
        readonly EcsPoolInject<HealthComponent> _healthPool = default;
        readonly EcsPoolInject<BableProtectionComponent> _bableProtectPool = default;

        private float _timeToCreateAuraMaxValue = 1f;
        private float _timeToCreateAuraCurrentValue = 1f;

        private float _protectEffectMaxDuration = 5f;

        private int _aliveUnitLayer = LayerMask.NameToLayer(nameof(ViewComponent.AliveUnit));

        public void Run (IEcsSystems systems)
        {
            if (_timeToCreateAuraCurrentValue > 0)
            {
                _timeToCreateAuraCurrentValue -= Time.deltaTime;
                return;
            }
            else
            {
                _timeToCreateAuraCurrentValue = _timeToCreateAuraMaxValue;
            }

            foreach (var bableEntity in _bableFilter.Value)
            {

                ref var viewComponent = ref _viewPool.Value.Get(bableEntity);
                ref var fractionComponent = ref _fractionPool.Value.Get(bableEntity);
                ref var healthComponent = ref _healthPool.Value.Get(bableEntity);

                var _allUnitsInAura = Physics.OverlapSphere(viewComponent.Transform.position, 10f);

                foreach (var monsterInAura in _allUnitsInAura)
                {
                    if (monsterInAura.gameObject.layer != _aliveUnitLayer)
                    {
                        continue;
                    }

                    var monsterInAuraEcsInfoMB = monsterInAura.GetComponent<EcsInfoMB>();
                    var monsterInAuraEntity = monsterInAuraEcsInfoMB.GetEntity();

                    if (!_unitPool.Value.Has(monsterInAuraEntity))
                    {
                        continue;
                    }

                    if (_onBoardUnitPool.Value.Has(monsterInAuraEntity))
                    {
                        continue;
                    }

                    ref var monsterInAuraFractionComponent = ref _fractionPool.Value.Get(monsterInAuraEntity);

                    if (monsterInAuraFractionComponent.isFriendly != fractionComponent.isFriendly)
                    {
                        continue;
                    }

                    if (!_bableProtectPool.Value.Has(monsterInAuraEntity))
                        _bableProtectPool.Value.Add(monsterInAuraEntity);

                    ref var bableProtectComponent = ref _bableProtectPool.Value.Get(monsterInAuraEntity);

                    bableProtectComponent.TimerToClearMaxValue = _protectEffectMaxDuration;
                    bableProtectComponent.TimerToClearCurrentValue = bableProtectComponent.TimerToClearMaxValue;

                    if (!bableProtectComponent.isWork)
                    {
                        bableProtectComponent.isWork = true;
                        bableProtectComponent.ProtectionValue = healthComponent.MaxValue;

                        ref var protectedMonsterViewComponent = ref _viewPool.Value.Get(monsterInAuraEntity);
                        protectedMonsterViewComponent.HealthBarMB.SetMaxShield(bableProtectComponent.ProtectionValue);

                        GameObject.Instantiate(_gameState.Value.EffectsPool.MonstersEffects.BableProtectionBuff, protectedMonsterViewComponent.GameObject.transform);
                    }
                }
            }
        }
    }
}