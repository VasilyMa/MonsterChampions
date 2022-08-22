using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class RetargetOnEnemyInDetectionZoneSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<UnitTag, Targetable>, Exc<DeadTag, OnBoardUnitTag>> _aliveUnitsFilter = default;

        readonly EcsPoolInject<Targetable> _targetablePool = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;

        public void Run (IEcsSystems systems)
        {
            foreach (var unitEntity in _aliveUnitsFilter.Value)
            {
                ref var targetable = ref _targetablePool.Value.Get(unitEntity);

                if (targetable.EntitysInDetectionZone.Count <= 0)
                {
                    continue;
                }

                targetable.TargetEntity = targetable.EntitysInDetectionZone[0];
                targetable.TargetObject = _viewPool.Value.Get(targetable.TargetEntity).GameObject;
            }
        }
    }
}