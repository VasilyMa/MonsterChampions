using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CheckAndDeleteBableProtectSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<UnitTag, BableProtectionComponent>, Exc<DeadTag, OnBoardUnitTag>> _protectsUnitsFilter = default;

        readonly EcsPoolInject<BableProtectionComponent> _bableProtectPool = default;

        public void Run (IEcsSystems systems)
        {
            foreach (var protectedEntity in _protectsUnitsFilter.Value)
            {
                ref var bableProtectComponent = ref _bableProtectPool.Value.Get(protectedEntity);

                bableProtectComponent.TimerToClearCurrentValue -= Time.deltaTime;

                if (bableProtectComponent.TimerToClearCurrentValue <= 0)
                {
                    _bableProtectPool.Value.Del(protectedEntity);
                }
            }
        }
    }
}