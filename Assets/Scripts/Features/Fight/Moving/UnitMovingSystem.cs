using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.AI;

namespace Client
{
    sealed class UnitMovingSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<Movable>> _movableFilter = default;
        readonly EcsPoolInject<Movable> _movablePool = default;

        public void Run (IEcsSystems systems)
        {
            foreach (var movableEntity in _movableFilter.Value)
            {
                ref var movableComponent = ref _movablePool.Value.Get(movableEntity);

                if (movableComponent.Destination == null)
                {
                    continue;
                }

                movableComponent.NavMeshAgent.SetDestination(movableComponent.Destination);
            }
        }
    }
}