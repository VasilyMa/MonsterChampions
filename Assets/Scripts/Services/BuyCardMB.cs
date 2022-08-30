using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;
using UnityEngine;
using DG.Tweening;
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
            ref var interfaceComp = ref _interfacePool.Get(_state.InterfaceEntity);
            for (int i = 0; i < boardComp.Transform.childCount; i++)
            {
                if (boardComp.Transform.GetChild(i).transform.childCount == 0)
                {
                    var dataCard = transform.GetChild(buttonId).GetComponentInChildren<CardInfo>();
                    if (_state.GetPlayerGold() >= dataCard.Cost)
                    {
                        interfaceComp.HolderCards.GetChild(buttonId).transform.DOScale(0.9f, 0.2f).OnComplete(() => ScaleDefault(buttonId));
                        _state.RevomePlayerGold(dataCard.Cost);
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
        private void ScaleDefault(int index)
        {
            ref var interfaceComp = ref _interfacePool.Get(_state.InterfaceEntity);
            interfaceComp.HolderCards.GetChild(index).transform.DOScale(1, 0.2f);
        }
    }
}
