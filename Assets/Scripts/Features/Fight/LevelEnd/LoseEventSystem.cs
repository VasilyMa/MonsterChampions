using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
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
                interfaceComp.LoseHolder.DOMove(GameObject.Find("TargetLoseWin").transform.position, 1f, false);
                _state.Value.runSysytem = false;
                Debug.Log("Ты всрал, дружок-пирожок");
                _loseEventFilter.Pools.Inc1.Del(eventEntity);
            }
        }
    }
}