using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.AI;

namespace Client
{
    sealed class SetPointToMoveEventSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<Movable, SetPointToMoveEvent>> _movableFilter = default;
        readonly EcsPoolInject<SetPointToMoveEvent> _setPointToMoveEventPool = default;
        readonly EcsPoolInject<Movable> _movablePool = default;

        public void Run (IEcsSystems systems)
        {
            foreach (var movableEntity in _movableFilter.Value)
            {
                ref var movableComponent = ref _movablePool.Value.Get(movableEntity);
                ref var setPointToMoveEvent = ref _setPointToMoveEventPool.Value.Get(movableEntity);

                if (setPointToMoveEvent.NewDestination == setPointToMoveEvent.OldDestination)
                {
                    DeleteEvent(movableEntity);
                    continue;
                }

                if (setPointToMoveEvent.ToEnemyBase)
                {
                    //to do ay vector3 point of EnemyBase. Probably from GameState
                }
                else
                {
                    movableComponent.NavMeshAgent.SetDestination(setPointToMoveEvent.NewDestination);
                }

                DeleteEvent(movableEntity);
            }
        }

        private void DeleteEvent(int movableEntity)
        {
            _setPointToMoveEventPool.Value.Del(movableEntity);
        }
    }
}