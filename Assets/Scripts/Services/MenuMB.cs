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
        private Vector3 deafaultPosDeck;
        public void Init(EcsWorld world, GameState state)
        {
            _world = world;
            _state = state;
            _state.inCollection = false;
            _interfacePool = _world.GetPool<InterfaceComponent>();
            _playableDeckEventPool = _world.GetPool<PlayableDeckEvent>();
            ref var interfaceComp = ref _interfacePool.Get(_state.InterfaceEntity);
            defaultPosCollectionButton = interfaceComp.MenuHolder.transform.GetChild(1).transform.position;
            defaultPosPlayButton = interfaceComp.MenuHolder.transform.GetChild(0).transform.GetChild(0).transform.position;
            defaultPosCollection = interfaceComp.CollectionMenu.transform.GetChild(1).transform.position;
            defaultPosCardPanel = interfaceComp.HolderCards.transform.position;
            deafaultPosDeck = interfaceComp.DeckHolder.transform.position;
            interfaceComp.DeckHolder.transform.DOMove(GameObject.Find("TargetDeck").transform.position, 1f, false);
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
            else if (!_state.isDrag && !_state.inCollection && emptyCard < 3)
            {
                _state.hubSystem = false;
                _state.runSysytem = true;
                _state.inCollection = false;
                interfaceComp.Resources.gameObject.SetActive(true);
                interfaceComp.HolderCards.gameObject.SetActive(true);
                interfaceComp.DeckHolder.transform.DOMove(deafaultPosDeck, 1f, false);
                interfaceComp.RemoveHolder.transform.DOMove(interfaceComp.defaultPosRemoveButton, 1f, false);
                interfaceComp.CollectionMenu.transform.GetChild(1).transform.DOMove(defaultPosCollection, 1f, false);
                interfaceComp.HolderCards.transform.DOMove(GameObject.Find("TargetCardPanel").transform.position, 1f, false);
                interfaceComp.Progress.transform.GetChild(0).transform.DOMove(GameObject.Find("TargetProgress").transform.position, 1f, false);
                interfaceComp.MenuHolder.gameObject.SetActive(false);
                _playableDeckEventPool.Add(_world.NewEntity());
            }
        }
        public void ToCollection()
        {
            ref var interfaceComp = ref _interfacePool.Get(_state.InterfaceEntity);
            if (!isOpen)
            {
                _state.inCollection = true;
                UpdateCollection();
                interfaceComp.RemoveHolder.transform.DOMove((GameObject.Find("TargetRemove").transform.position), 1f, false);
                interfaceComp.CollectionMenu.transform.GetChild(1).transform.DOMove((GameObject.Find("TargetCollection").transform.position), 1f, false);
                interfaceComp.MenuHolder.transform.GetChild(0).GetChild(0).transform.DOMove((GameObject.Find("TargetPlayButton").transform.position), 1f, false);
                interfaceComp.MenuHolder.transform.GetChild(0).GetChild(0).transform.DOScale(0.75f, 0.5f);
                interfaceComp.MenuHolder.transform.GetChild(1).transform.DOMove((GameObject.Find("TargetCollectionName").transform.position), 1f, false);
                isOpen = true;
            }
            else if (isOpen)
            {
                interfaceComp.MenuHolder.transform.GetChild(1).GetComponent<Button>().interactable = false;
                _state.inCollection = false;
                interfaceComp.RemoveHolder.transform.DOMove(interfaceComp.defaultPosRemoveButton, 1f, false);
                interfaceComp.CollectionMenu.transform.GetChild(1).transform.DOMove(defaultPosCollection, 1f, false);
                interfaceComp.MenuHolder.transform.GetChild(0).GetChild(0).transform.DOMove(defaultPosPlayButton, 1f, false);
                interfaceComp.MenuHolder.transform.GetChild(0).GetChild(0).transform.DOScale(1f, 0.5f);
                interfaceComp.MenuHolder.transform.GetChild(1).transform.DOMove(defaultPosCollectionButton, 1f, false).OnComplete(() => RemoveCollection());
                isOpen = false;
            }
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
                    cardInfo.UpdateCardInfo(_state.InterfaceConfigs.elementsShirt);
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
                    cardInfo.UpdateCardInfo(_state.InterfaceConfigs.elementsShirt);
                }
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

