using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace Client
{
    sealed class TargetingSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<Targetable>> _targetableFilter = default;

        readonly EcsPoolInject<Targetable> _targetablePool = default;

        public void Run (IEcsSystems systems)
        {
            foreach (var targetabelEntity in _targetableFilter.Value)
            {
                ref var targetableComponent = ref _targetablePool.Value.Get(targetabelEntity);

                if (targetableComponent.TargetEntity > 0)
                {
                    continue;
                }

                // to do ay targetable
            }
        }
    }
}