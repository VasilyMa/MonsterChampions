using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class WorkingSlevDebuffAuraSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<SlevAuraDebuff, UnitTag, HealthComponent>, Exc<DeadTag, OnBoardUnitTag>> _debuffUnitsFilter = default;

        readonly EcsPoolInject<SlevAuraDebuff> _slevAuraDebuffPool = default;
        readonly EcsPoolInject<HealthComponent> _healthPool = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;

        public void Run (IEcsSystems systems) // to do ay rewrite this system with methods
        {
            foreach (var debuffedUnitEntity in _debuffUnitsFilter.Value)
            {
                ref var healthComponent = ref _healthPool.Value.Get(debuffedUnitEntity);
                ref var slevAuraDebuffComponent = ref _slevAuraDebuffPool.Value.Get(debuffedUnitEntity);
                ref var viewComp = ref _viewPool.Value.Get(debuffedUnitEntity);

                slevAuraDebuffComponent.TimerToClearCurrentValue -= Time.deltaTime;

                if (slevAuraDebuffComponent.TimerToClearCurrentValue <= 0)
                {
                    healthComponent.MaxValue += slevAuraDebuffComponent.CroppedHealthValueBeforeDebuff;
                    _slevAuraDebuffPool.Value.Del(debuffedUnitEntity);
                }

                if (slevAuraDebuffComponent.isWork)
                {
                    continue;
                }

                slevAuraDebuffComponent.CroppedHealthValueBeforeDebuff = Mathf.Round(healthComponent.MaxValue / 2);

                healthComponent.MaxValue -= slevAuraDebuffComponent.CroppedHealthValueBeforeDebuff;
                healthComponent.CurrentValue -= Mathf.Round(healthComponent.CurrentValue / 2);
                viewComp.HealthBarMB.SetMaxHealth(healthComponent.MaxValue);
                viewComp.HealthBarMB.UpdateHealth(healthComponent.CurrentValue);
                slevAuraDebuffComponent.isWork = true;
            }
        }
    }
}