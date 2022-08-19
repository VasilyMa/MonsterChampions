using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class DieEventSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<DieEvent>> _dieEventFilter = default;

        readonly EcsPoolInject<DieEvent> _dieEventPool = default;

        readonly EcsPoolInject<DeadTag> _deadPool = default;
        readonly EcsPoolInject<BaseTag> _basePool = default;
        readonly EcsPoolInject<Targetable> _targetablePool = default;

        public void Run (IEcsSystems systems)
        {
            foreach (var dieEventEntity in _dieEventFilter.Value)
            {
                ref var dieEvent = ref _dieEventPool.Value.Get(dieEventEntity);

                if (_deadPool.Value.Has(dieEvent.DyingEntity))
                {
                    DeleteEvent(dieEventEntity);
                    continue;
                }

                DisableTargetableComponent(dieEvent.DyingEntity);

                _deadPool.Value.Add(dieEvent.DyingEntity);

                if (_basePool.Value.Has(dieEvent.DyingEntity))
                {
                    // to do ay win or lose
                }

                DeleteEvent(dieEventEntity);
            }
        }

        private void DeleteEvent(int dieEventEntity)
        {
            _dieEventPool.Value.Del(dieEventEntity);
        }

        private void DisableTargetableComponent(int dyingEntity)
        {
            if (!_targetablePool.Value.Has(dyingEntity))
            {
                return;
            }

            ref var targetableComponent = ref _targetablePool.Value.Get(dyingEntity);
            targetableComponent.TargetEntity = BattleState.NULL_ENTITY;
            targetableComponent.TargetObject = null;
            targetableComponent.EntitysInDetectionZone?.Clear();
            targetableComponent.EntitysInMeleeZone?.Clear();
            targetableComponent.EntitysInRangeZone?.Clear();
        }
    }
}