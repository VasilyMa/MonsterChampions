using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;
using System;

namespace Client
{
    public class MenuMB : MonoBehaviour
    {
        private EcsWorld _world;
        private GameState _state;
        private EcsPool<InterfaceComponent> _interfacePool;
        private EcsPool<PlayableDeckEvent> _playableDeckEventPool;
        private bool isOpen;
        public void Init(EcsWorld world, GameState state)
        {
            _world = world;
            _state = state;
            _state.inCollection = false;
            _interfacePool = _world.GetPool<InterfaceComponent>();
            _playableDeckEventPool = _world.GetPool<PlayableDeckEvent>();
            ref var interfaceComp = ref _interfacePool.Get(_state.InterfaceEntity);
            interfaceComp.DeckHolder.transform.DOMove(interfaceComp.TargetDeck.transform.position, 1f, false);
        }
        public void Play()
        {
            ref var interfaceComp = ref _interfacePool.Get(_state.InterfaceEntity);
            int emptyCard = 0;
            if (_state.inCollection)
            {
                ToCollection();
                return;
            }
            else if (!_state.isDrag && !_state.inCollection)
            {
                for (int i = 0; i < _state.Deck.DeckPlayer.Length; i++)
                {
                    if (_state.Deck.DeckPlayer[i].MonsterID == MonstersID.Value.Default)
                        emptyCard++;
                    if (emptyCard == 3)
                    {
                        interfaceComp.AttentionHolder.gameObject.SetActive(true);
                        interfaceComp.AttentionHolder.DOScale(1, 0.25f).OnComplete(() => StartCoroutine(WaitScale()));
                        break;
                    }
                }
            }
            if (!_state.isDrag && !_state.inCollection && emptyCard < 3)
            {
                if (Tutorial.CurrentStage == Tutorial.Stage.OpenCollection)
                {
                    return;
                }
                _state.HubSystems = false;
                _state.PreparedSystems = true;
                _state.FightSystems = false;
                _state.inCollection = false;
                interfaceComp.Resources.gameObject.SetActive(true);
                interfaceComp.HolderCards.gameObject.SetActive(true);
                interfaceComp.DeckHolder.transform.DOMove(interfaceComp.deafaultPosDeck, 1f, false);
                interfaceComp.RemoveHolder.transform.DOMove(interfaceComp.defaultPosRemoveButton, 1f, false);
                interfaceComp.CollectionMenu.transform.GetChild(1).transform.DOMove(interfaceComp.defaultPosCollection, 1f, false);
                interfaceComp.HolderCards.transform.DOMove(interfaceComp.TargetCardPanel.position, 1f, false);
                interfaceComp.Progress.transform.GetChild(0).transform.DOMove(interfaceComp.TargetProgressBar.position, 1f, false);
                interfaceComp.MenuHolder.gameObject.SetActive(false);
                _playableDeckEventPool.Add(_world.NewEntity());
            }
        }
        public void ToCollection()
        {
            ref var interfaceComp = ref _interfacePool.Get(_state.InterfaceEntity);
            if (!isOpen)
            {
                interfaceComp.MenuHolder.transform.GetChild(1).GetComponent<Button>().interactable = false;
                if (Tutorial.CurrentStage == Tutorial.Stage.OpenCollection)
                {
                    Tutorial.OpenCollection.SetCollectionIsOpened();
                }
                _state.inCollection = true;
                UpdateCollection();
                //interfaceComp.RemoveHolder.transform.DOMove((GameObject.Find("TargetRemove").transform.position), 1f, false);
                interfaceComp.CollectionMenu.transform.GetChild(1).transform.DOMove(interfaceComp.TargetCollection.position, 1f, false).OnComplete(()=>Open());
                interfaceComp.MenuHolder.transform.GetChild(0).GetChild(0).transform.DOMove(interfaceComp.TargetPlayButton.position, 1f, false);
                interfaceComp.MenuHolder.transform.GetChild(0).GetChild(0).transform.DOScale(0.75f, 0.5f);
                interfaceComp.MenuHolder.transform.GetChild(1).transform.DOMove(interfaceComp.TargetCollectionButton.position, 1f, false);
                if (Tutorial.CurrentStage == Tutorial.Stage.OpenCollection || Tutorial.CurrentStage == Tutorial.Stage.DragAndDropNewCardInDeck)
                {
                    interfaceComp.MenuHolder.transform.GetChild(1).transform.DOMove((GameObject.Find("TargetCollectionName").transform.position), 1f, false).OnComplete(() => Tutorial.DragAndDropNewCardInDeck.SetPanelIsOpened());
                }
                interfaceComp.MenuHolder.transform.GetChild(1).transform.DOMove((GameObject.Find("TargetCollectionName").transform.position), 1f, false);
                //interfaceComp.MenuHolder.transform.GetChild(1).transform.DOScale(0.75f, 0.5f);
                isOpen = true;
            }
            else if (isOpen)
            {
                if (Tutorial.CurrentStage == Tutorial.Stage.DragAndDropNewCardInDeck)
                {
                    return;
                }

                interfaceComp.MenuHolder.transform.GetChild(1).GetComponent<Button>().interactable = false;
                _state.inCollection = false;
                interfaceComp.RemoveHolder.transform.DOMove(interfaceComp.defaultPosRemoveButton, 1f, false);
                interfaceComp.CollectionMenu.transform.GetChild(1).transform.DOMove(interfaceComp.defaultPosCollection, 1f, false).OnComplete(() => RemoveCollection());
                interfaceComp.MenuHolder.transform.GetChild(0).GetChild(0).transform.DOMove(interfaceComp.defaultPosPlayButton, 1f, false);
                interfaceComp.MenuHolder.transform.GetChild(0).GetChild(0).transform.DOScale(1f, 0.5f);
                interfaceComp.MenuHolder.transform.GetChild(1).transform.DOMove(interfaceComp.defaultPosCollectionButton, 1f, false);
                isOpen = false;
            }
        }

        private void Open()
        {
            ref var interfaceComp = ref _interfacePool.Get(_state.InterfaceEntity);
            interfaceComp.MenuHolder.transform.GetChild(1).GetComponent<Button>().interactable = true;
        }

        public void UpdateCollection()
        {
            ref var interfaceComp = ref _interfacePool.Get(_state.InterfaceEntity);
            var collection = _state.Collection.CollectionUnits;
            var holder = interfaceComp.CollectionHolder;

            if (collection.Count <= 3)
                holder.parent.parent.GetComponentInParent<Image>().sprite = _state.InterfaceConfigs.collectionBackground[0];
            if(collection.Count > 3 && collection.Count < 6)
                holder.parent.parent.GetComponentInParent<Image>().sprite = _state.InterfaceConfigs.collectionBackground[1];

            if (collection.Count > 0)
            {
                foreach (var card in collection)
                {
                    var addedCard = (GameObject)GameObject.Instantiate(Resources.Load("CollectionCard"), holder);
                    var cardInfo = addedCard.GetComponent<CardInfo>();
                    cardInfo.LevelCard = card.LevelCard;
                    cardInfo.UniqueID = card.UniqueID;
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
            var holderDeck = interfaceComp.DeckHolder;
            foreach (var item in deck)
            {
                if (item.MonsterID > 0)
                {
                    for (int i = 0; i < holderDeck.childCount; i++)
                    {
                        if (holderDeck.GetChild(i).childCount == 0)
                        {
                            var addedCard = (GameObject)GameObject.Instantiate(Resources.Load("CollectionCard"), holderDeck.GetChild(i).transform);
                            var cardInfo = addedCard.GetComponent<CardInfo>();
                            cardInfo.LevelCard = item.LevelCard;
                            cardInfo.UniqueID = item.UniqueID;
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
                            break;
                        }
                    }
                }
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
        private IEnumerator WaitScale()
        {
            yield return new WaitForSeconds(1.5f);
            ScaleToZero();
        }
        void ScaleToZero()
        {
            ref var interfaceComp = ref _interfacePool.Get(_state.InterfaceEntity);
            interfaceComp.AttentionHolder.transform.DOScale(0, 0.25f).OnComplete(()=>Off());
        }
        void Off()
        {
            ref var interfaceComp = ref _interfacePool.Get(_state.InterfaceEntity);
            interfaceComp.AttentionHolder.gameObject.SetActive(false);
        }
        public void Exit()
        {
            Application.Quit();
        }
    }
}

