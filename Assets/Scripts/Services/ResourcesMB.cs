using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine.UI;

namespace Client 
{
    public class ResourcesMB : MonoBehaviour
    {
        private EcsWorld _world;
        private GameState _state;
        private EcsPool<InterfaceComponent> _interfacePool;
        [SerializeField] private Text _playerCoinAmount;
        [SerializeField] private Text _enemyCoinAmount;
        public void Init(EcsWorld world, GameState state)
        {
            _world = world;
            _state = state;
            _interfacePool = _world.GetPool<InterfaceComponent>();
        }

        public void UpdatePlayerCoinAmount()
        {
            _playerCoinAmount.text = _state.GetPlayerGold().ToString();
        }

        public void UpdateEnemyCoinAmount()
        {
            _enemyCoinAmount.text = _state.GetEnemyGold().ToString();
        }
    }
}

