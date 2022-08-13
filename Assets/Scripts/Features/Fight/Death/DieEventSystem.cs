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
    }
}
}