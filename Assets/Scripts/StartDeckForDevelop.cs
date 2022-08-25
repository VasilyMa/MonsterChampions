using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.EcsLite;
namespace Client
{
    public class StartDeckForDevelop : MonoBehaviour
    {
        private EcsWorld _world;
        private GameState _state;
        private EcsPool<InterfaceComponent> _interfacePool;
        private EcsPool<NewMonster> _newMonsterPool;
        private EcsPool<RewardComponentEvent> _rewardPool;

        public void Init(EcsWorld world, GameState state)
        {
            _world = world;
            _state = state;
            _interfacePool = _world.GetPool<InterfaceComponent>();
            _newMonsterPool = _world.GetPool<NewMonster>();
            _rewardPool = _world.GetPool<RewardComponentEvent>();
        }
        public void GetBaseDeck()
        {
            _rewardPool.Add(_world.NewEntity());
            //Debug.LogWarning("You get base deck");
            //for (int i = 0; i < 2; i++)
            //{
            //    _newMonsterPool.Add(_world.NewEntity());
            //}
        }
    }
}
