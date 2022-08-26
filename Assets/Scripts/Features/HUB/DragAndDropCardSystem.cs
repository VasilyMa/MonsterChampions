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
                    dragComp.CardObject.transform.position = Input.mousePosition;
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
                        switch (result.gameObject.tag)
                        {
                            case "Deck": //if its deck then add to deck new item and add to array new card
                                if (result.gameObject.transform.childCount <= 2)
                                {
                                    dragComp.CardObject.transform.SetParent(result.gameObject.transform);
                                    dragComp.CardObject.GetComponent<Image>().raycastTarget = true;
                                    if (dragComp.DefaultParent.name == "Cards")
                                    {
                                        for (int i = 0; i < deck.Length; i++) //add card to deck
                                        {
                                            if (deck[i].MonsterID == 0)
                                            {
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
                                            if (collection[y].MonsterID == cardInfo.MonsterID)
                                            {
                                                collection.Remove(collection[y]);
                                                break;
                                            }   
                                        }
                                        _state.Value.Save();
                                    }
                                    dragComp.DefaultParent = result.gameObject.transform;
                                    Debug.Log("Hit " + result.gameObject.name);
                                }
                                else
                                {
                                    dragComp.CardObject.transform.parent = dragComp.DefaultParent;
                                    dragComp.CardObject.GetComponent<Image>().raycastTarget = true;
                                }
                                break;
                            case "Collection": //if its collection then add card from deck to collection and remove its from deck
                                dragComp.CardObject.transform.SetParent(result.gameObject.transform.GetChild(0));
                                dragComp.CardObject.GetComponent<Image>().raycastTarget = true;
                                for (int i = 0; i < deck.Length; i++)
                                {
                                    if (deck[i].MonsterID == cardInfo.MonsterID)
                                    {
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
                                if (dragComp.DefaultParent.CompareTag("Deck"))
                                {
                                    UnitData unitData = new UnitData(); //there save the new card in collection
                                    unitData.Sprite = cardInfo.Sprite;
                                    unitData.Cost  = cardInfo.Cost;
                                    unitData.MonsterID = cardInfo.MonsterID;
                                    unitData.Damage = cardInfo.Damage;
                                    unitData.Health = cardInfo.Health;
                                    unitData.Elemental = cardInfo.Elemental;
                                    unitData.VisualAndAnimations = cardInfo.VisualAndAnimations;
                                    unitData.MoveSpeed = cardInfo.MoveSpeed;
                                    collection.Add(unitData);
                                }
                                dragComp.DefaultParent = result.gameObject.transform;
                                _state.Value.Save();
                                break;
                            default:
                                dragComp.CardObject.transform.parent = dragComp.DefaultParent;
                                dragComp.CardObject.GetComponent<Image>().raycastTarget = true;
                                break;
                        }
                    }
                    _state.Value.isDrag = false;
                    dragComp.CardObject.transform.DOScale(1f, 0.2f);
                    interfaceComp.CollectionHolder.GetComponentInParent<ScrollRect>().enabled = true;
                    _touchFilter.Pools.Inc1.Del(entity);
                }
            }
        }
    }
}