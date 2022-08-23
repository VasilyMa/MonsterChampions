using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class WinEventSystem : IEcsRunSystem
    {
        readonly EcsWorldInject _world = default;
        readonly EcsFilterInject<Inc<WinEvent>> _winEventFilter = default;
        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;
        readonly EcsPoolInject<RewardComponentEvent> _rewardPool = default;
        readonly EcsSharedInject<GameState> _state = default;

        public void Run (IEcsSystems systems)
        {
            foreach (var eventEntity in _winEventFilter.Value)
            {
                ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
                interfaceComp.RewardPanelHolder.gameObject.SetActive(true);
                _rewardPool.Value.Add(_world.Value.NewEntity());
                Time.timeScale = 0;
                Debug.Log("Ты победил, умничка");
                _winEventFilter.Pools.Inc1.Del(eventEntity);
            }
        }
    }
}