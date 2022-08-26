using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

namespace Client
{
    public class MenuMB : MonoBehaviour
    {
        private EcsWorld _world;
        private GameState _state;
        private EcsPool<InterfaceComponent> _interfacePool;
        private EcsPool<PlayableDeckEvent> _playableDeckEventPool;
        private bool isOpen;
        private Vector3 defaultPosCollectionButton;
        private Vector3 defaultPosPlayButton;
        private Vector3 defaultPosCollection;
        private Vector3 defaultPosCardPanel;
        public void Init(EcsWorld world, GameState state)
        {
            _world = world;
            _state = state;
            _state.inCollection = false;
            _interfacePool = _world.GetPool<InterfaceComponent>();
            _playableDeckEventPool = _world.GetPool<PlayableDeckEvent>();
            ref var interfaceComp = ref _interfacePool.Get(_state.InterfaceEntity);
            defaultPosCollectionButton = interfaceComp.MenuHolder.transform.GetChild(1).transform.position;
            defaultPosPlayButton = interfaceComp.MenuHolder.transform.GetChild(0).transform.position;
            defaultPosCollection = interfaceComp.CollectionMenu.transform.GetChild(1).transform.position;
            defaultPosCardPanel = interfaceComp.HolderCards.transform.position;
        }
        public void Play()
        {
            ref var interfaceComp = ref _interfacePool.Get(_state.InterfaceEntity);
            _state.hubSystem = false;
            _state.runSysytem = true;
            _state.inCollection = false;
            interfaceComp.Resources.gameObject.SetActive(true);
            interfaceComp.HolderCards.gameObject.SetActive(true);
            interfaceComp.DeckHolder.transform.DOMoveY(Screen.height * 1.5f, 1f, false);
            interfaceComp.CollectionMenu.transform.GetChild(1).transform.DOMove(defaultPosCollection, 1f, false);
            interfaceComp.HolderCards.transform.DOMove(GameObject.Find("TargetCardPanel").transform.position, 1f, false);
            interfaceComp.MenuHolder.gameObject.SetActive(false);
            _playableDeckEventPool.Add(_world.NewEntity());
        }
        public void ToCollection()
        {
            ref var interfaceComp = ref _interfacePool.Get(_state.InterfaceEntity);
            if (!isOpen)
            {
                _state.inCollection = true;
                UpdateCollection();
                interfaceComp.CollectionMenu.transform.GetChild(1).transform.DOMove((GameObject.Find("TargetCollection").transform.position), 1f, false);
                interfaceComp.MenuHolder.transform.GetChild(0).transform.DOMove((GameObject.Find("TargetPlayButton").transform.position), 1f, false);
                interfaceComp.MenuHolder.transform.GetChild(0).transform.DOScale(0.5f, 0.5f);
                interfaceComp.MenuHolder.transform.GetChild(1).transform.DOMove((GameObject.Find("TargetCollectionName").transform.position), 1f, false);
                interfaceComp.MenuHolder.transform.GetChild(1).transform.DOScale(0.5f, 0.5f);
                isOpen = true;
            }
            else if (isOpen)
            {
                interfaceComp.MenuHolder.transform.GetChild(1).GetComponent<Button>().interactable = false;
                _state.inCollection = false;
                interfaceComp.CollectionMenu.transform.GetChild(1).transform.DOMove(defaultPosCollection, 1f, false);
                interfaceComp.MenuHolder.transform.GetChild(0).transform.DOMove(defaultPosPlayButton, 1f, false);
                interfaceComp.MenuHolder.transform.GetChild(0).transform.DOScale(1f, 0.5f);
                interfaceComp.MenuHolder.transform.GetChild(1).transform.DOMove(defaultPosCollectionButton, 1f, false).OnComplete(() => RemoveCollection());
                interfaceComp.MenuHolder.transform.GetChild(1).transform.DOScale(1f, 0.5f);
                //StartCoroutine(RemoveCollectionTimer());
                isOpen = false;
            }
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
                    var addedCard = (GameObject)GameObject.Instantiate(Resources.Load("CollectionCard"), holder);
                    var cardInfo = addedCard.GetComponent<CardInfo>();
                    cardInfo.Cost = card.Cost;
                    cardInfo.Sprite = card.Sprite;
                    cardInfo.MonsterID = card.MonsterID;
                    cardInfo.Health = card.Health;
                    cardInfo.Damage = card.Damage;
                    cardInfo.Elemental = card.Elemental;
                    cardInfo.MoveSpeed = card.MoveSpeed;
                    cardInfo.Prefabs = card.Prefabs;
                    cardInfo.VisualAndAnimations = card.VisualAndAnimations;
                    cardInfo.UpdateCardInfo();
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
                if (item.MonsterID > 0)
                {
                    var addedCard = (GameObject)GameObject.Instantiate(Resources.Load("CollectionCard"), interfaceComp.DeckHolder);
                    var cardInfo = addedCard.GetComponent<CardInfo>();
                    cardInfo.Cost = item.Cost;
                    cardInfo.Sprite = item.Sprite;
                    cardInfo.MonsterID = item.MonsterID;
                    cardInfo.Health = item.Health;
                    cardInfo.Damage = item.Damage;
                    cardInfo.Elemental = item.Elemental;
                    cardInfo.MoveSpeed = item.MoveSpeed;
                    cardInfo.Prefabs = item.Prefabs;
                    cardInfo.VisualAndAnimations = item.VisualAndAnimations;
                    cardInfo.UpdateCardInfo();
                }
                
                //if (item.UnitID > 0 && holder.childCount < 3)
                //{
                //    for (int i = 0; i < holder.childCount; i++)
                //    {
                //        if (holder.GetChild(i).GetComponent<CardInfo>().unitID == 0)
                //        {
                            
                //        }
                //    }
                //}
            }
        }
        private IEnumerator RemoveCollectionTimer()
        {
            yield return new WaitForSeconds(1);
            if (!isOpen)
            {
                RemoveCollection();
            }
        }
        public void RemoveCollection()
        {
            ref var interfaceComp = ref _interfacePool.Get(_state.InterfaceEntity);
            interfaceComp.MenuHolder.transform.GetChild(1).GetComponent<Button>().interactable = true;
            var collection = _state.Collection.CollectionUnits;
            var holder = interfaceComp.CollectionHolder;
            for (int i = 0; i < holder.childCount; i++)
            {
                if (holder.GetChild(i).GetComponent<CardInfo>().MonsterID > 0)
                {
                    Destroy(holder.GetChild(i).gameObject);
                }
            }
        }
        public void RemoveDeck()
        {
            ref var interfaceComp = ref _interfacePool.Get(_state.InterfaceEntity);
            var deck = _state.Deck.DeckPlayer;
            var holder = interfaceComp.DeckHolder;
            for (int i = 0; i < holder.childCount; i++)
            {
                if (holder.GetChild(i).GetComponent<CardInfo>().MonsterID > 0)
                {
                    Destroy(holder.GetChild(i).gameObject);
                }
            }
        }
        public void Exit()
        {
            Application.Quit();
        }
    }
}

