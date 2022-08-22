using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class LoseEventSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<LoseEvent>> _loseEventFilter = default;

        public void Run (IEcsSystems systems)
        {
            foreach (var eventEntity in _loseEventFilter.Value)
            {
                Debug.Log("Ты всрал, дружок-пирожок");
            }
        }
    }
}