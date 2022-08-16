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
                if (Input.GetMouseButton(0))
                    dragComp.CardObject.transform.position = Input.mousePosition;
                if (Input.GetMouseButtonUp(0))
                {
                    inputComp.PointerEventData = new PointerEventData(inputComp.EventSystem);
                    //Set the Pointer Event Position to that of the mouse position
                    inputComp.PointerEventData.position = Input.mousePosition;
                    //Create a list of Raycast Results
                    List<RaycastResult> results = new List<RaycastResult>();
                    //Raycast using the Graphics Raycaster and mouse click position
                    inputComp.Raycaster.Raycast(inputComp.PointerEventData, results);
                    //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
                    if (results.Count == 0)
                    {
                        dragComp.CardObject.transform.parent = dragComp.DefaultParent;
                        dragComp.CardObject.GetComponent<Image>().raycastTarget = true;
                    }
                    foreach (RaycastResult result in results)
                    {
                        switch (result.gameObject.tag)
                        {
                            case "Deck":
                                dragComp.CardObject.transform.SetParent(result.gameObject.transform);
                                dragComp.DefaultParent = result.gameObject.transform;
                                dragComp.CardObject.GetComponent<Image>().raycastTarget = true;
                                var cardInfo = dragComp.CardObject.GetComponent<CardInfo>();
                                for (int i = 0; i < deck.Length; i++)
                                {
                                    if (deck[i].UnitID == 0)
                                    {
                                        deck[i].UnitID = cardInfo.unitID;
                                        break;
                                    }
                                }
                                Debug.Log("Hit " + result.gameObject.name);
                                break;
                            case "Collection":
                                dragComp.CardObject.transform.SetParent(result.gameObject.transform);
                                dragComp.DefaultParent = result.gameObject.transform;
                                dragComp.CardObject.GetComponent<Image>().raycastTarget = true;
                                Debug.Log("Hit " + result.gameObject.name);
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