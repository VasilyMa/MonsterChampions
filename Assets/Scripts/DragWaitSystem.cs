using DG.Tweening;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.UI;

namespace Client 
{
    sealed class DragWaitSystem : IEcsRunSystem 
    {
        readonly EcsWorldInject _world = default;
        readonly EcsSharedInject<GameState> _state = default;
        readonly EcsFilterInject<Inc<DragCardEvent>> _dragEvent = default;
        readonly EcsPoolInject<DragCardEvent> _dragPool = default;
        readonly EcsFilterInject<Inc<DragWaitEvent>> _waitFilter = default;
        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;
        public void Run (IEcsSystems systems) 
        {
            foreach (var drag in _waitFilter.Value)
            {
                
                ref var waitComp = ref _waitFilter.Pools.Inc1.Get(drag);
                ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);

                if (Input.GetMouseButtonUp(0))
                {
                    _waitFilter.Pools.Inc1.Del(drag);
                    break;
                }

                if (waitComp.DefaultParent.CompareTag("Deck"))
                {
                    foreach (var item in _dragEvent.Value)
                    {
                        _dragEvent.Pools.Inc1.Del(item);
                    }
                    _state.Value.isDrag = true;
                    ref var deckDragComp = ref _dragPool.Value.Add(_world.Value.NewEntity());
                    deckDragComp.DefaultPos = waitComp.DefaultPos;
                    deckDragComp.DefaultParent = waitComp.DefaultParent;
                    deckDragComp.CardObject = waitComp.CardObject;

                    interfaceComp.CollectionHolder.GetComponentInParent<ScrollRect>().enabled = false;
                    waitComp.CardObject.transform.SetParent(GameObject.FindObjectOfType<CollectionMB>().transform);
                    waitComp.CardObject.GetComponent<Image>().raycastTarget = false;
                    waitComp.CardObject.transform.DOScale(0.8f, 0.2f);
                    _waitFilter.Pools.Inc1.Del(drag);

                    break;
                }
                if (waitComp.timerDrag > 0)
                {
                    waitComp.timerDrag-=Time.deltaTime;
                    continue;
                }
                if (Input.GetMouseButton(0))
                {
                    foreach (var item in _dragEvent.Value)
                    {
                        _dragEvent.Pools.Inc1.Del(item);
                    }
                    _state.Value.isDrag = true;
                    ref var dragComp = ref _dragPool.Value.Add(_world.Value.NewEntity());
                    dragComp.DefaultPos = waitComp.DefaultPos;
                    dragComp.DefaultParent = waitComp.DefaultParent;
                    dragComp.CardObject = waitComp.CardObject;

                    interfaceComp.CollectionHolder.GetComponentInParent<ScrollRect>().enabled = false;
                    waitComp.CardObject.transform.SetParent(GameObject.FindObjectOfType<CollectionMB>().transform);
                    waitComp.CardObject.GetComponent<Image>().raycastTarget = false;
                    waitComp.CardObject.transform.DOScale(0.8f, 0.2f);
                    _waitFilter.Pools.Inc1.Del(drag);

                    break;
                }
            }
        }
    }
}