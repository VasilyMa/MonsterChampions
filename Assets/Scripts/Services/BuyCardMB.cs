using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Client
{
    public class BuyCardMB : MonoBehaviour
    {
        private EcsWorld _world;
        private GameState _state;
        private EcsPool<BuyUnitEvent> _buyPool;
        private EcsPool<MonsterSpawnEvent> _monsterSpawnEventPool;
        private EcsPool<ViewComponent> _viewPool;
        private EcsPool<InterfaceComponent> _interfacePool;
        public void Init(EcsWorld world, GameState state)
        {
            _world = world;
            _state = state;
            _buyPool = _world.GetPool<BuyUnitEvent>();
            _monsterSpawnEventPool = _world.GetPool<MonsterSpawnEvent>();
            _viewPool = _world.GetPool<ViewComponent>(); 
            _interfacePool = _world.GetPool<InterfaceComponent>();
            
        }

        public void BuyUnit(int buttonId)
        {
            ref var boardViewComp = ref _viewPool.Get(_state.BoardEntity);
            ref var interfaceComp = ref _interfacePool.Get(_state.InterfaceEntity);
            for (int i = 0; i < boardViewComp.Transform.childCount; i++)
            {
                if (boardViewComp.Transform.GetChild(i).transform.childCount == 0)
                {
                    var dataCard = transform.GetChild(buttonId).GetComponentInChildren<CardInfo>();
                    if (_state.GetPlayerGold() >= dataCard.Cost)
                    {
                        interfaceComp.HolderCards.GetChild(buttonId).transform.DOScale(0.9f, 0.2f).OnComplete(() => ScaleDefault(buttonId));
                        _state.RevomePlayerGold(dataCard.Cost);
                        _interfacePool.Get(_state.InterfaceEntity).Resources.UpdatePlayerCoinAmount();

                        var slot = FindEmptySlot();

                        ref var monsterSpawnEvent = ref _monsterSpawnEventPool.Add(_world.NewEntity());
                        monsterSpawnEvent.Invoke(   slot.position,
                                                    Quaternion.identity,
                                                    isFriendly: true,
                                                    dataCard.Cost,
                                                    level: 1,
                                                    dataCard.Damage,
                                                    dataCard.Health,
                                                    dataCard.MoveSpeed,
                                                    dataCard.Elemental,
                                                    dataCard.MonsterID,
                                                    dataCard.VisualAndAnimations,
                                                    slot);

                        break;
                    }
                    else
                        break;
                }
            }
            CheckButtons();
        }
        private void ScaleDefault(int index)
        {
            ref var interfaceComp = ref _interfacePool.Get(_state.InterfaceEntity);
            interfaceComp.HolderCards.GetChild(index).transform.DOScale(1, 0.2f);
        }

        private Transform FindEmptySlot() //find the empty slot on board for buy unit and add it
        {
            Transform slot = null;
            for (int i = 0; i < _viewPool.Get(_state.BoardEntity).GameObject.transform.childCount; i++)
            {
                slot = _viewPool.Get(_state.BoardEntity).GameObject.transform.GetChild(i);
                if (slot.childCount >= 1)
                {
                    continue;
                }
                else
                {
                    return slot;
                }
            }
            return slot;
        }
        public void CheckButtons()
        {
            ref var interfaceComp = ref _interfacePool.Get(_state.InterfaceEntity);
            var holder = interfaceComp.HolderCards;
            for (int i = 0; i < holder.childCount; i++)
            {
                if (holder.GetChild(i).gameObject.activeSelf)
                {
                    if (_state.GetPlayerGold() >= 10)
                        holder.GetChild(i).transform.GetChild(0).transform.GetChild(2).GetComponent<Image>().enabled = false;
                    else
                        holder.GetChild(i).transform.GetChild(0).transform.GetChild(2).GetComponent<Image>().enabled = true;
                }
            }
        }
    }
}
