using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;
using UnityEngine;
namespace Client
{
    public class BuyCardMB : MonoBehaviour
    {
        private EcsWorld _world;
        private GameState _state;
        private EcsPool<BuyUnitEvent> _buyPool;
        public void Init(EcsWorld world, GameState state)
        {
            _world = world;
            _state = state;
            _buyPool = _world.GetPool<BuyUnitEvent>();
        }

        public void BuyUnit()
        {
            if (transform.GetChild(0).transform.childCount >= 1)
            {
                var dataCard = GetComponentInChildren<CardInfo>();
                ref var buyComp = ref _buyPool.Add(_world.NewEntity());
                buyComp.CardInfo = dataCard;
                Debug.Log("Zap zap");
            }
        }
    }
}
