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
                ref var setPointToMoveEventComponent = ref _setPointToMoveEventPool.Value.Get(movableEntity);

                if (setPointToMoveEventComponent.ToEnemyBase)
                {
                    //to do ay vector3 point of EnemyBase. Probably from GameState
                }
                else
                {
                    movableComponent.NavMeshAgent.SetDestination(setPointToMoveEventComponent.DestinationPoint);
                }

                _setPointToMoveEventPool.Value.Del(movableEntity);
            }
        }
    }
}