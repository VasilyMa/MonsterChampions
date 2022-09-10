using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
namespace Client
{
    sealed class DragAndDropCardSystem : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _state = default;
        readonly EcsFilterInject<Inc<DragCardEvent>> _touchFilter = default;
        readonly EcsPoolInject<InputComponent> _inputPool = default;
        readonly EcsPoolInject<DragCardEvent> _dragPool = default;
        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _touchFilter.Value)
            {
                ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
                ref var inputComp = ref _inputPool.Value.Get(_state.Value.InputEntity);
                ref var dragComp = ref _dragPool.Value.Get(entity);
                var deck = _state.Value.Deck.DeckPlayer;
                var collection = _state.Value.Collection.CollectionUnits;
                var cardInfo = dragComp.CardObject.GetComponent<CardInfo>();


                if (Input.GetMouseButton(0)) //if finger touch already
                {
                    dragComp.CardObject.transform.position = Input.mousePosition;

                    if (Tutorial.CurrentStage == Tutorial.Stage.DragAndDropNewCardInDeck)
                    {
                        Tutorial.DragAndDropNewCardInDeck.SetCardIsDragged();
                    }
                }
                if (Input.GetMouseButtonUp(0)) //if finger end touch
                {
                    inputComp.PointerEventData = new PointerEventData(inputComp.EventSystem);
                    //set the Pointer Event Position to that of the mouse position
                    inputComp.PointerEventData.position = Input.mousePosition;
                    //create a list of Raycast Results
                    List<RaycastResult> results = new List<RaycastResult>();
                    //raycast using the Graphics Raycaster and mouse click position
                    inputComp.Raycaster.Raycast(inputComp.PointerEventData, results);
                    if (results.Count == 0) //if under finger is nothing then we return our card to default place
                    {
                        dragComp.CardObject.transform.parent = dragComp.DefaultParent;
                        dragComp.CardObject.GetComponent<Image>().raycastTarget = true;
                    }
                    //for every result returned, output the name of the GameObject on the Canvas hit by the Ray
                    foreach (RaycastResult result in results) 
                    {
                        if (result.gameObject.CompareTag("Deck"))
                        {
                            DragInDeck(result, entity);

                            Debug.Log("Отработал Deck");
                            if (Tutorial.CurrentStage == Tutorial.Stage.DragAndDropNewCardInDeck)
                            {
                                Tutorial.DragAndDropNewCardInDeck.SetCardIsDroppedInDeck();
                            }
                            break;
                        }
                        if (result.gameObject.CompareTag("Collection"))
                        {
                            DragInCollection(result, entity);

                            Debug.Log("Отработал Collection");
                            if (Tutorial.CurrentStage == Tutorial.Stage.DragAndDropNewCardInDeck)
                            {
                                Tutorial.DragAndDropNewCardInDeck.SetCardIsDroppedBack();
                            }
                            break;
                        }
                        if (result.gameObject.CompareTag("Remove"))
                        {
                            RemoveCard(result, entity);
                            break;
                        }
                        dragComp.CardObject.transform.SetParent(dragComp.DefaultParent);
                        dragComp.CardObject.GetComponent<Image>().raycastTarget = true;
                        //break;
                    }
                    _state.Value.Save();
                    dragComp.CardObject.transform.DOScale(1f, 0.2f).OnComplete(()=>Complete());
                    interfaceComp.CollectionHolder.GetComponentInParent<ScrollRect>().enabled = true;
                    _touchFilter.Pools.Inc1.Del(entity);
                }
            }
        }

        private void RemoveCard(RaycastResult result, int entity)
        {
            ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
            ref var inputComp = ref _inputPool.Value.Get(_state.Value.InputEntity);
            ref var dragComp = ref _dragPool.Value.Get(entity);
            var deck = _state.Value.Deck.DeckPlayer;
            var collection = _state.Value.Collection.CollectionUnits;
            var cardInfo = dragComp.CardObject.GetComponent<CardInfo>();
            foreach (var card in collection)
            {
                if (card.UniqueID == cardInfo.UniqueID)
                {
                    collection.Remove(card);
                    break;
                }
            }
            for (int i = 0; i < deck.Length; i++)
            {
                if (cardInfo.UniqueID == deck[i].UniqueID)
                {
                    deck[i].UniqueID = 0;
                    deck[i].Sprite = null;
                    deck[i].Cost = 0;
                    deck[i].Damage = 0;
                    deck[i].MonsterID = 0;
                    deck[i].Health = 0;
                    deck[i].Elemental = 0;
                    deck[i].Prefabs = null;
                    deck[i].MoveSpeed = 0;
                    break;
                }
            }
            GameObject.Destroy(dragComp.CardObject);
            UpdateCollection();
        }

        private void DragInDeck(RaycastResult result, int entity)
        {
            ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
            ref var inputComp = ref _inputPool.Value.Get(_state.Value.InputEntity);
            ref var dragComp = ref _dragPool.Value.Get(entity);
            var deck = _state.Value.Deck.DeckPlayer;
            var collection = _state.Value.Collection.CollectionUnits;
            var cardInfo = dragComp.CardObject.GetComponent<CardInfo>();
            if (result.gameObject.transform.childCount <= 2)
            {
                if (dragComp.DefaultParent.name == "Cards")
                {
                    for (int i = 0; i < deck.Length; i++) //add card to deck
                    {
                        if (deck[i].UniqueID == 0)
                        {
                            deck[i].UniqueID = cardInfo.UniqueID;
                            deck[i].Cost = cardInfo.Cost;
                            deck[i].Sprite = cardInfo.Sprite;
                            deck[i].MonsterID = cardInfo.MonsterID;
                            deck[i].Damage = cardInfo.Damage;
                            deck[i].Health = cardInfo.Health;
                            deck[i].Elemental = cardInfo.Elemental;
                            deck[i].MoveSpeed = cardInfo.MoveSpeed;
                            deck[i].VisualAndAnimations = cardInfo.VisualAndAnimations;
                            break;
                        }
                    }
                    for (int y = 0; y < collection.Count; y++) //remove are card from collection 
                    {
                        if (collection[y].UniqueID == cardInfo.UniqueID)
                        {
                            collection.Remove(collection[y]);
                            break;
                        }
                    }
                    dragComp.CardObject.transform.SetParent(result.gameObject.transform);
                }
                dragComp.DefaultParent = result.gameObject.transform;
                dragComp.CardObject.GetComponent<Image>().raycastTarget = true;
                Debug.Log("Hit " + result.gameObject.name);
            }
            else //there we swap are card in deck and dragged
            {
                var PointerEventData = new PointerEventData(inputComp.EventSystem);
                PointerEventData.position = Input.mousePosition;
                List<RaycastResult> resultsNew = new List<RaycastResult>();
                inputComp.Raycaster.Raycast(inputComp.PointerEventData, resultsNew);
                foreach (var resultNew in resultsNew)
                {
                    if (resultNew.gameObject.CompareTag("Card"))
                    {
                        for (int i = 0; i < collection.Count; i++)
                        {
                            if (collection[i].UniqueID == cardInfo.UniqueID)
                            {
                                collection.Remove(collection[i]);
                                UnitData unitData = new UnitData(); //there save the new card in collection
                                unitData.UniqueID = resultNew.gameObject.GetComponent<CardInfo>().UniqueID;
                                unitData.Sprite = resultNew.gameObject.GetComponent<CardInfo>().Sprite;
                                unitData.Cost = resultNew.gameObject.GetComponent<CardInfo>().Cost;
                                unitData.MonsterID = resultNew.gameObject.GetComponent<CardInfo>().MonsterID;
                                unitData.Damage = resultNew.gameObject.GetComponent<CardInfo>().Damage;
                                unitData.Health = resultNew.gameObject.GetComponent<CardInfo>().Health;
                                unitData.Elemental = resultNew.gameObject.GetComponent<CardInfo>().Elemental;
                                unitData.VisualAndAnimations = resultNew.gameObject.GetComponent<CardInfo>().VisualAndAnimations;
                                unitData.MoveSpeed = resultNew.gameObject.GetComponent<CardInfo>().MoveSpeed;
                                collection.Add(unitData);
                                break;
                            }
                        }
                        for (int i = 0; i < deck.Length; i++)
                        {
                            if (deck[i].UniqueID == resultNew.gameObject.GetComponent<CardInfo>().UniqueID)
                            {
                                deck[i].UniqueID = cardInfo.UniqueID;
                                deck[i].Cost = cardInfo.Cost;
                                deck[i].Sprite = cardInfo.Sprite;
                                deck[i].MonsterID = cardInfo.MonsterID;
                                deck[i].Damage = cardInfo.Damage;
                                deck[i].Health = cardInfo.Health;
                                deck[i].Elemental = cardInfo.Elemental;
                                deck[i].MoveSpeed = cardInfo.MoveSpeed;
                                deck[i].VisualAndAnimations = cardInfo.VisualAndAnimations;
                                break;
                            }
                        }
                        dragComp.CardObject.transform.SetParent(resultNew.gameObject.transform.parent);
                        resultNew.gameObject.transform.SetParent(dragComp.DefaultParent);
                        dragComp.CardObject.GetComponent<Image>().raycastTarget = true;
                        break;
                    }
                }
            }
            UpdateCollection();
        }
        private void DragInCollection(RaycastResult result, int entity)
        {
            ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
            ref var inputComp = ref _inputPool.Value.Get(_state.Value.InputEntity);
            ref var dragComp = ref _dragPool.Value.Get(entity);
            var deck = _state.Value.Deck.DeckPlayer;
            var collection = _state.Value.Collection.CollectionUnits;
            var cardInfo = dragComp.CardObject.GetComponent<CardInfo>();
            if (dragComp.DefaultParent.CompareTag("Deck"))
            {
                for (int i = 0; i < deck.Length; i++)
                {
                    if (deck[i].UniqueID == cardInfo.UniqueID)
                    {
                        deck[i].UniqueID = 0;
                        deck[i].Sprite = null;
                        deck[i].Cost = 0;
                        deck[i].Damage = 0;
                        deck[i].MonsterID = 0;
                        deck[i].Health = 0;
                        deck[i].Elemental = 0;
                        deck[i].Prefabs = null;
                        deck[i].MoveSpeed = 0;
                        break;
                    }
                }
                UnitData unitData = new UnitData(); //there save the new card in collection
                unitData.UniqueID = cardInfo.UniqueID;
                unitData.Sprite = cardInfo.Sprite;
                unitData.Cost = cardInfo.Cost;
                unitData.MonsterID = cardInfo.MonsterID;
                unitData.Damage = cardInfo.Damage;
                unitData.Health = cardInfo.Health;
                unitData.Elemental = cardInfo.Elemental;
                unitData.VisualAndAnimations = cardInfo.VisualAndAnimations;
                unitData.MoveSpeed = cardInfo.MoveSpeed;
                collection.Add(unitData);
                dragComp.DefaultParent = result.gameObject.transform.GetChild(0).transform.GetChild(0).transform;
                dragComp.CardObject.transform.SetParent(dragComp.DefaultParent);
            }
            else
            {
                dragComp.CardObject.transform.SetParent(dragComp.DefaultParent);
            }
            UpdateCollection();
            dragComp.CardObject.GetComponent<Image>().raycastTarget = true;
        }
        private void UpdateCollection()
        {
            ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
            var collection = _state.Value.Collection.CollectionUnits;
            var holder = interfaceComp.CollectionHolder;

            if (collection.Count <= 3)
                holder.parent.parent.GetComponentInParent<Image>().sprite = _state.Value.InterfaceConfigs.collectionBackground[0];
            if (collection.Count > 3 && collection.Count < 6)
                holder.parent.parent.GetComponentInParent<Image>().sprite = _state.Value.InterfaceConfigs.collectionBackground[1];
        }

        private void Complete()
        {
            _state.Value.isDrag = false;
        }
    }
}