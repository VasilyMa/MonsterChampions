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
        readonly EcsPoolInject<SlevAuraDebuff> _slevAuraDebuffPool = default;

        private float _timerMaxValue = 5f;

        private int _aliveUnitLayer = LayerMask.NameToLayer(nameof(ViewComponent.AliveUnit));

        public void Run (IEcsSystems systems) // to do ay rewrite this system with methods. And check how OverlapSphere working with layers
        {
            foreach (var slevEntity in _slevFilter.Value)
            {
                ref var slevComponent = ref _slevPool.Value.Get(slevEntity);
                ref var viewComponent = ref _viewPool.Value.Get(slevEntity);
                ref var fractionComponent = ref _fractionPool.Value.Get(slevEntity);

                if (slevComponent.TimerToCreateAuraCurrentValue > 0)
                {
                    slevComponent.TimerToCreateAuraCurrentValue -= Time.deltaTime;
                    continue;
                }
                else
                {
                    slevComponent.TimerToCreateAuraCurrentValue = slevComponent.TimerToCreateAuraMaxValue;
                }

                var _allUnitsInAura = Physics.OverlapSphere(viewComponent.Transform.position, 10f);

                Debug.Log($"����� �������: {_allUnitsInAura.Length}");

                int collidersCount = 0;
                int enemyCount = 0;

                foreach (var unitInAura in _allUnitsInAura)
                {
                    if (unitInAura.gameObject.layer != _aliveUnitLayer)
                    {
                        continue;
                    }

                    collidersCount++;

                    var unitEcsInfoMB = unitInAura.GetComponent<EcsInfoMB>();
                    var unitEntity = unitEcsInfoMB.GetEntity();
                    Debug.Log("�����");
                    if (!_unitPool.Value.Has(unitEntity))
                    {
                        continue;
                    }

                    ref var unitFractionComponent = ref _fractionPool.Value.Get(unitEntity);

                    if (unitFractionComponent.isFriendly != fractionComponent.isFriendly)
                    {
                        if (!_slevAuraDebuffPool.Value.Has(unitEntity))
                        _slevAuraDebuffPool.Value.Add(unitEntity);

                        ref var slevAuraDebuff = ref _slevAuraDebuffPool.Value.Get(unitEntity);

                        if (!slevAuraDebuff.isWork)
                        {
                            slevAuraDebuff.TimerToClearMaxValue = _timerMaxValue;
                        }

                        slevAuraDebuff.TimerToClearCurrentValue = slevAuraDebuff.TimerToClearMaxValue;

                        enemyCount++;
                    }
                }

                Debug.Log($"������� �����������: {collidersCount}. ������� ������: {enemyCount}");
            }
        }
    }
}