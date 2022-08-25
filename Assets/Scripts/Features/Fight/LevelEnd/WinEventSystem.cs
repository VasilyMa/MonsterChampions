using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.SceneManagement;

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
                Time.timeScale = 1;
                _state.Value.hubSystem = true;
                _state.Value.runSysytem = false;
                Debug.Log("�� �������, �������"); 
                int index = 0;
                if (SceneManager.GetActiveScene().buildIndex + 1 > SceneManager.sceneCountInBuildSettings - 1)
                {
                    index = 0;
                }
                else
                {
                    index = SceneManager.GetActiveScene().buildIndex + 1;
                }
                _state.Value.Settings.SceneNumber = index;
                _state.Value.Settings.Level += 1;
                _state.Value.Save();
                _winEventFilter.Pools.Inc1.Del(eventEntity);
            }
        }
    }
}