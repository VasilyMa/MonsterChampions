using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class WinEventSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<WinEvent>> _winEventFilter = default;
        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;
        readonly EcsSharedInject<GameState> _state = default;

        public void Run (IEcsSystems systems)
        {
            foreach (var eventEntity in _winEventFilter.Value)
            {
                ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
                interfaceComp.RewardHolder.gameObject.SetActive(true);
                Time.timeScale = 0;
                Debug.Log("Ты победил, умничка");
                _winEventFilter.Pools.Inc1.Del(eventEntity);
            }
        }
    }
}