using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class WinEventSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<WinEvent>> _winEventFilter = default;

        public void Run (IEcsSystems systems)
        {
            foreach (var eventEntity in _winEventFilter.Value)
            {
                Debug.Log("Ты победил, умничка");
            }
        }
    }
}