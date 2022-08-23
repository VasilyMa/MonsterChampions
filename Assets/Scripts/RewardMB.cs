using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.EcsLite;
namespace Client
{
    public class RewardMB : MonoBehaviour
    {
        private EcsWorld _world;
        private GameState _state;
        private EcsPool<InterfaceComponent> _interfacePool;
        private EcsPool<NewMonster> _newMonster;
        public void Init(EcsWorld world, GameState state)
        {
            _world = world;
            _state = state;
            _interfacePool = _world.GetPool<InterfaceComponent>();
            _newMonster = _world.GetPool<NewMonster>();
        }

    }
}
