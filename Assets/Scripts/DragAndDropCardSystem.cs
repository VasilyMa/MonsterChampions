using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Client
{
    sealed class DragAndDropCardSystem : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _state = default;
        readonly EcsFilterInject<Inc<DragCardEvent>> _touchFilter = default;
        readonly EcsPoolInject<InputComponent> _inputPool = default;
        readonly EcsPoolInject<DragCardEvent> _dragPool = default;
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _touchFilter.Value)
            {
                ref var inputComp = ref _inputPool.Value.Get(_state.Value.InputEntity);
                ref var dragComp = ref _dragPool.Value.Get(entity);
                var deck = _state.Value.Deck.DeckPlayer;
                var collection = _state.Value.Collection.CollectionUnits;
                var cardInfo = dragComp.CardObject.GetComponent<CardInfo>();
                if (Input.GetMouseButton(0))
                    dragComp.CardObject.transform.position = Input.mousePosition;
                if (Input.GetMouseButtonUp(0))
                {
                    inputComp.PointerEventData = new PointerEventData(inputComp.EventSystem);
                    //set the Pointer Event Position to that of the mouse position
                    inputComp.PointerEventData.position = Input.mousePosition;
                    //create a list of Raycast Results
                    List<RaycastResult> results = new List<RaycastResult>();
                    //raycast using the Graphics Raycaster and mouse click position
                    inputComp.Raycaster.Raycast(inputComp.PointerEventData, results);
                    if (results.Count == 0)
                    {
                        dragComp.CardObject.transform.parent = dragComp.DefaultParent;
                        dragComp.CardObject.GetComponent<Image>().raycastTarget = true;
                    }
                    //for every result returned, output the name of the GameObject on the Canvas hit by the Ray
                    foreach (RaycastResult result in results) 
                    {
                        switch (result.gameObject.tag)
                        {
                            case "Deck":
                                dragComp.CardObject.transform.SetParent(result.gameObject.transform);
                                dragComp.DefaultParent = result.gameObject.transform;
                                dragComp.CardObject.GetComponent<Image>().raycastTarget = true;
                                for (int i = 0; i < deck.Length; i++) //add card to deck
                                {
                                    if (deck[i].UnitID == 0)
                                    {
                                        deck[i].UnitID = cardInfo.unitID;
                                        break;
                                    }
                                }
                                for (int y = 0; y < collection.Count; y++) //remove are card from collection 
                                {
                                    if (collection[y].UnitID == cardInfo.unitID)
                                        collection.Remove(collection[y]);
                                }
                                _state.Value.Save();
                                Debug.Log("Hit " + result.gameObject.name);
                                break;
                            case "Collection":
                                dragComp.CardObject.transform.SetParent(result.gameObject.transform);
                                dragComp.CardObject.GetComponent<Image>().raycastTarget = true;
                                for (int i = 0; i < deck.Length; i++)
                                {
                                    if (deck[i].UnitID == cardInfo.unitID)
                                    {
                                        deck[i].UnitID = 0;

                                        break;
                                    }
                                }
                                if (dragComp.DefaultParent.CompareTag("Deck"))
                                {
                                    UnitData unitData = new UnitData(); //there save the new card in collection
                                    unitData.UnitID = cardInfo.unitID;
                                    collection.Add(unitData);
                                }
                                dragComp.DefaultParent = result.gameObject.transform;
                                _state.Value.Save();
                                break;
                            default:
                                break;
                        }
                    }
                    _touchFilter.Pools.Inc1.Del(entity);
                }
            }
        }
    }
}