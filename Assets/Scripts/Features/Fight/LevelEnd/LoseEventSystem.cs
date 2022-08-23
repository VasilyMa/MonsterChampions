using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Client
{
    sealed class LoseEventSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<LoseEvent>> _loseEventFilter = default;
        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;
        readonly EcsSharedInject<GameState> _state = default;
        public void Run (IEcsSystems systems)
        {
            foreach (var eventEntity in _loseEventFilter.Value)
            {
                ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
                interfaceComp.LoseHolder.gameObject.SetActive(true);
                Time.timeScale = 0;
                Debug.Log("Ты всрал, дружок-пирожок");
                _loseEventFilter.Pools.Inc1.Del(eventEntity);
            }
        }
    }
}