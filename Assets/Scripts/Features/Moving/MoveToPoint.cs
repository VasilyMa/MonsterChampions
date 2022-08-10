using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.AI;

namespace Client
{
    sealed class MoveToPoint : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<Movable>> _movableFilter = default;
        readonly EcsPoolInject<Movable> _movablePool = default;

        public void Run (IEcsSystems systems)
        {
            foreach (var movableEntity in _movableFilter.Value)
            {
                ref var movableComponent = ref _movablePool.Value.Get(movableEntity);
                movableComponent.NavMeshAgent.SetDestination(Vector3.zero);
            }
        }
    }
}