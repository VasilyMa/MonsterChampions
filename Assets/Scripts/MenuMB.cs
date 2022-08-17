using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Client
{
    public class MenuMB : MonoBehaviour
    {
        private EcsWorld _world;
        private GameState _state;
        private EcsPool<InterfaceComponent> _interfacePool;
        public void Init(EcsWorld world, GameState state)
        {
            _world = world;
            _state = state;
            _interfacePool = _world.GetPool<InterfaceComponent>();
        }
        public void Play()
        {
            SceneManager.LoadScene(_state.CurrentLevel);
        }
        public void ToCollection()
        {
            ref var interfaceComp = ref _interfacePool.Get(_state.InterfaceEntity);
            interfaceComp.MenuHolder.gameObject.SetActive(false);
            interfaceComp.CollectionMenu.gameObject.SetActive(true);
        }
        public void Exit()
        {
            Application.Quit();
        }
    }
}

