using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CreateSlevDebuffAuraSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<SlevComponent, ViewComponent>, Exc<DeadTag, OnBoardUnitTag>> _slevFilter = default;

        readonly EcsPoolInject<SlevComponent> _slevPool = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        readonly EcsPoolInject<FractionComponent> _fractionPool = default;
        readonly EcsPoolInject<UnitTag> _unitPool = default;
        readonly EcsPoolInject<OnBoardUnitTag> _onBoardUnitPool = default;
        readonly EcsPoolInject<SlevAuraDebuff> _slevAuraDebuffPool = default;

        private float _timeToCreateAuraMaxValue = 1f;
        private float _timeToCreateAuraCurrentValue = 1f;
        private float _auraEffectMaxDuration = 5f;

        private int _aliveUnitLayer = LayerMask.GetMask(nameof(ViewComponent.AliveUnit));

        public void Run (IEcsSystems systems) // to do ay rewrite this system with methods. And check how OverlapSphere working with layers
        {
            foreach (var slevEntity in _slevFilter.Value)
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

                ref var viewComponent = ref _viewPool.Value.Get(slevEntity);
                ref var fractionComponent = ref _fractionPool.Value.Get(slevEntity);

                var _allUnitsInAura = Physics.OverlapSphere(viewComponent.Transform.position, 10f, _aliveUnitLayer);

                Debug.Log($"Всего найдено: {_allUnitsInAura.Length}");

                int collidersCount = 0;
                int enemyCount = 0;

                foreach (var unitInAura in _allUnitsInAura)
                {
                    collidersCount++;

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

                    if (unitFractionComponent.isFriendly == fractionComponent.isFriendly)
                    {
                        continue;
                    }

                    if (!_slevAuraDebuffPool.Value.Has(unitEntity))
                        _slevAuraDebuffPool.Value.Add(unitEntity);

                    ref var slevAuraDebuff = ref _slevAuraDebuffPool.Value.Get(unitEntity);

                    if (!slevAuraDebuff.isWork)
                    {
                        slevAuraDebuff.TimerToClearMaxValue = _auraEffectMaxDuration;
                    }

                    slevAuraDebuff.TimerToClearCurrentValue = slevAuraDebuff.TimerToClearMaxValue;

                    enemyCount++;
                }

                Debug.Log($"Найдено коллайдеров: {collidersCount}. Найдено врагов: {enemyCount}");
            }
        }
    }
}