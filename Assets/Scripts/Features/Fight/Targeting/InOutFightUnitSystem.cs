using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class InOutFightUnitSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<Targetable, UnitTag>, Exc<DeadTag, OnBoardUnitTag>> _aliveTargetableUnitsFilter = default;

        readonly EcsPoolInject<Targetable> _targetablePool = default;
        readonly EcsPoolInject<UnitTag> _unitPool = default;
        readonly EcsPoolInject<InFightTag> _inFightPool = default;

        public void Run (IEcsSystems systems)
        {
            foreach (var unitEntity in _aliveTargetableUnitsFilter.Value)
            {
                ref var targetableComponent = ref _targetablePool.Value.Get(unitEntity);

                if (targetableComponent.EntitysInDetectionZone.Count > 0)
                {
                    if (!_inFightPool.Value.Has(unitEntity))
                        _inFightPool.Value.Add(unitEntity);
                }
                else
                {
                    if (_inFightPool.Value.Has(unitEntity))
                        _inFightPool.Value.Del(unitEntity);
                }
            }
        }
    }
}