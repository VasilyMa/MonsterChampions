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
        private EcsPool<ViewComponent> _board;
        public void Init(EcsWorld world, GameState state)
        {
            _world = world;
            _state = state;
            _buyPool = _world.GetPool<BuyUnitEvent>();
            _board = _world.GetPool<ViewComponent>(); 
        }

        public void BuyUnit()
        {
            ref var boardComp = ref _board.Get(_state.BoardEntity);
            for (int i = 0; i < boardComp.Transform.childCount; i++)
            {
                if (boardComp.Transform.GetChild(i).transform.childCount == 0)
                {
                    var dataCard = GetComponentInChildren<CardInfo>();
                    ref var buyComp = ref _buyPool.Add(_world.NewEntity());
                    buyComp.CardInfo = dataCard;
                    break;
                    Debug.Log("Zap zap");
                }
            }
            
        }
    }
}
