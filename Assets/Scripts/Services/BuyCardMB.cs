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
        private EcsPool<InterfaceComponent> _interfacePool;
        public void Init(EcsWorld world, GameState state)
        {
            _world = world;
            _state = state;
            _buyPool = _world.GetPool<BuyUnitEvent>();
            _board = _world.GetPool<ViewComponent>(); 
            _interfacePool = _world.GetPool<InterfaceComponent>();
        }

        public void BuyUnit(int buttonId)
        {
            ref var boardComp = ref _board.Get(_state.BoardEntity);
            for (int i = 0; i < boardComp.Transform.childCount; i++)
            {
                if (boardComp.Transform.GetChild(i).transform.childCount == 0)
                {
                    var dataCard = transform.GetChild(buttonId).GetComponentInChildren<CardInfo>();
                    if (_state.PlayerGold >= dataCard.Cost)
                    {
                        _state.PlayerGold-=dataCard.Cost;
                        _interfacePool.Get(_state.InterfaceEntity).Resources.UpdateCoin();
                        ref var buyComp = ref _buyPool.Add(_world.NewEntity());
                        buyComp.CardInfo = dataCard;
                        break;
                    }
                    else
                        break;
                }
            }
        }
    }
}
