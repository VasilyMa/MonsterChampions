using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.AI;

namespace Client
{
    sealed class UnitMoveToTargetSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<Movable, Targetable>, Exc<IsForcedStoppedTag, DeadTag>> _movableFilter = default;

        readonly EcsPoolInject<Movable> _movablePool = default;
        readonly EcsPoolInject<Targetable> _targetablePool = default;

        public void Run (IEcsSystems systems)
        {
            foreach (var movableEntity in _movableFilter.Value)
            {
                ref var movableComponent = ref _movablePool.Value.Get(movableEntity);
                ref var targetableComponent = ref _targetablePool.Value.Get(movableEntity);

                if (BattleState.isNullableEntity(targetableComponent.TargetEntity))
                {
                    continue;
                }

                var targetPosition = targetableComponent.TargetObject.transform.position;

                if (movableComponent.NavMeshAgent.destination == targetPosition)
                {
                    continue;
                }

                movableComponent.NavMeshAgent.SetDestination(targetPosition);
            }
        }
    }
}