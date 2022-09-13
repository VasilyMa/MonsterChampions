using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.EcsLite;
using UnityEngine.SceneManagement;

namespace Client
{
    public class RewardPanelMB : MonoBehaviour
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
        private void OpenBiomPanel()
        {
            ref var interfaceComp = ref _interfacePool.Get(_state.InterfaceEntity);
            interfaceComp.BiomHolder.gameObject.SetActive(true);
            interfaceComp.RewardHolder.gameObject.SetActive(false);
        }
        public void StartEndLevel()
        {
            OpenBiomPanel();
            ref var interfaceComp = ref _interfacePool.Get(_state.InterfaceEntity);
            interfaceComp.RewardPanelHolder.gameObject.SetActive(false);
            StartCoroutine(WaitNextLevel());
        }
        private IEnumerator WaitNextLevel()
        {
            yield return new WaitForSeconds(2.5f);
            NextLevel();
        }
        public void NextLevel()
        {

            SceneManager.LoadScene(_state.Settings.SceneNumber);
            ref var interfaceComp = ref _interfacePool.Get(_state.InterfaceEntity);
            interfaceComp.RewardPanelHolder.gameObject.SetActive(false);
            interfaceComp.HolderCards.gameObject.SetActive(false);
            interfaceComp.MenuHolder.gameObject.SetActive(true);
        }
    }
}
