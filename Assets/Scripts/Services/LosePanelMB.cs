using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.EcsLite;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace Client
{
    public class LosePanelMB : MonoBehaviour
    {
        private EcsWorld _world;
        private GameState _state;
        private EcsPool<InterfaceComponent> _interfacePool;
        private Vector3 defaultPosLose;
        public void Init(EcsWorld world, GameState state)
        {
            _world = world;
            _state = state;
            _interfacePool = _world.GetPool<InterfaceComponent>();
            defaultPosLose = _interfacePool.Get(_state.InterfaceEntity).LoseHolder.position;
        }
        public void NextLevel()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(1);
        }
        public void BackToMenu()
        {
            ref var interfaceComp = ref _interfacePool.Get(_state.InterfaceEntity);
            interfaceComp.LoseHolder.transform.DOMove(defaultPosLose, 1f, false);
            interfaceComp.MenuHolder.gameObject.SetActive(true);
        }
    }
}
