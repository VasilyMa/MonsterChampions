using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class MoveTinkiThunderboltEffectSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<ThunderboltComponent>> _thunderboltFilter = default;

        readonly EcsPoolInject<ThunderboltComponent> _thunderboltPool = default;

        public void Run (IEcsSystems systems)
        {
            foreach (var thunderboltEntity in _thunderboltFilter.Value)
            {
                ref var thunderboltComponent = ref _thunderboltPool.Value.Get(thunderboltEntity);

                foreach (var thunderboltEffect in thunderboltComponent.ThunderboltEffects)
                {
                    if (thunderboltEffect.isMoved)
                    {
                        continue;
                    }

                    thunderboltEffect.Object.transform.position = thunderboltEffect.Destination;
                    thunderboltEffect.isMoved = true;
                }
            }
        }
    }
}