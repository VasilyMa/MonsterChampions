using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class ActivateEnemyBaseEventSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<ActivateEnemyBaseEvent>> _activateEnemyBaseEventFilter = default;

        readonly EcsPoolInject<ActivateEnemyBaseEvent> _activateEnemyBaseEventPool = default;
        readonly EcsPoolInject<DisabledBaseTag> _disabledBaseTagPool = default;

        public void Run (IEcsSystems systems)
        {
            foreach (var activateEnemyBaseEventEntity in _activateEnemyBaseEventFilter.Value)
            {
                _disabledBaseTagPool.Value.Del(activateEnemyBaseEventEntity);

                DeleteEvent(activateEnemyBaseEventEntity);
            }
        }

        private void DeleteEvent(int eventEntity)
        {
            _activateEnemyBaseEventPool.Value.Del(eventEntity);
        }
    }
}