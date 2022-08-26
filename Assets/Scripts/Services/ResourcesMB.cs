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
        [SerializeField] private Text amountCoin;
        public void Init(EcsWorld world, GameState state)
        {
            _world = world;
            _state = state;
            _interfacePool = _world.GetPool<InterfaceComponent>();
        }
        public void UpdateCoin()
        {
            amountCoin.text = _state.PlayerGold.ToString();
        }
    }
}

