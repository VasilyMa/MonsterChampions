using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Client
{
    public class MenuMB : MonoBehaviour
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
        public void Play()
        {
            SceneManager.LoadScene(_state.CurrentLevel);
        }
        public void ToCollection()
        {
            ref var interfaceComp = ref _interfacePool.Get(_state.InterfaceEntity);
            foreach (var item in _state.Collection.CollectionUnits)
            {
                if (item.UnitID > 0)
                {
                    UpdateCollection();
                    break;
                }
            }
            UpdateDeck();
            interfaceComp.MenuHolder.gameObject.SetActive(false);
            interfaceComp.CollectionMenu.gameObject.SetActive(true);

        }
        public void UpdateCollection()
        {
            ref var interfaceComp = ref _interfacePool.Get(_state.InterfaceEntity);
            var collection = _state.Collection.CollectionUnits;
            var holder = interfaceComp.CollectionHolder;
            if (collection.Count > 0)
            {
                foreach (var card in collection)
                {
                    if (card.UnitID > 0)
                    {
                        if (holder.childCount == 0)
                        {
                            var addedCard = (GameObject)GameObject.Instantiate(Resources.Load("CollectionCard"), holder);
                            var cardInfo = addedCard.GetComponent<CardInfo>();
                            cardInfo.unitID = card.UnitID;
                            cardInfo.NameUnit = card.NameUnit;
                            cardInfo.Health = card.Health;
                            cardInfo.Damage = card.Damage;
                            cardInfo.Elemental = card.Elemental;
                            cardInfo.MoveSpeed = card.MoveSpeed;
                            cardInfo.Prefabs = card.Prefabs;
                            cardInfo.UpdateCardInfo();
                        }
                        else
                        {
                            for (int i = 0; i < holder.childCount; i++)
                            {
                                if (holder.GetChild(i).GetComponent<CardInfo>().unitID != card.UnitID)
                                {
                                    var addedCard = (GameObject)GameObject.Instantiate(Resources.Load("CollectionCard"), holder);
                                    var cardInfo = addedCard.GetComponent<CardInfo>();
                                    cardInfo.unitID = card.UnitID;
                                    cardInfo.NameUnit = card.NameUnit;
                                    cardInfo.Health = card.Health;
                                    cardInfo.Damage = card.Damage;
                                    cardInfo.Elemental = card.Elemental;
                                    cardInfo.MoveSpeed = card.MoveSpeed;
                                    cardInfo.Prefabs = card.Prefabs;
                                    cardInfo.UpdateCardInfo();
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        public void UpdateDeck()
        {
            ref var interfaceComp = ref _interfacePool.Get(_state.InterfaceEntity);
            var deck = _state.Deck.DeckPlayer;
            var holder = interfaceComp.DeckHolder;
            foreach (var item in deck)
            {
                if (item.UnitID == 0)
                {
                    for (int i = 0; i < holder.childCount; i++)
                    {
                        if (holder.GetChild(i).GetComponent<CardInfo>().unitID == 0)
                        {
                            var addedCard = (GameObject)GameObject.Instantiate(Resources.Load("CollectionCard"), interfaceComp.DeckHolder);
                            var cardInfo = addedCard.GetComponent<CardInfo>();
                            cardInfo.unitID = item.UnitID;
                            cardInfo.NameUnit = item.NameUnit;
                            cardInfo.Health = item.Health;
                            cardInfo.Damage = item.Damage;
                            cardInfo.Elemental = item.Elemental;
                            cardInfo.MoveSpeed = item.MoveSpeed;
                            cardInfo.Prefabs = item.Prefabs;
                            cardInfo.UpdateCardInfo();
                        }
                    }
                }
            }
        }
        public void Exit()
        {
            Application.Quit();
        }
    }
}

