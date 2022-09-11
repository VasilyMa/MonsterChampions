using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
namespace Client {
    sealed class InputSystem : IEcsRunSystem {
        readonly EcsSharedInject<GameState> _state = default;
        readonly EcsWorldInject _world = default;
        readonly EcsFilterInject<Inc<InputComponent>> _inputFilter = default;
        readonly EcsFilterInject<Inc<TouchEvent>> _touchFilter = default;
        readonly EcsPoolInject<TouchEvent> _touchPool = default;
        readonly EcsPoolInject<DragAndDropUnitComponent> _dragUnitPool = default;
        readonly EcsPoolInject<Movable> _movablePool = default;
        readonly EcsPoolInject<NewMonster> _newMonsterPool = default;
        readonly EcsPoolInject<DragCardEvent> _cardEventPool = default;
        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;
        readonly EcsPoolInject<DragWaitEvent> _waitPool = default;
        public void Run (IEcsSystems systems) {
            foreach (var entity in _inputFilter.Value)
            {
                ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
                ref var inputComp = ref _inputFilter.Pools.Inc1.Get(entity);

                if (!Input.GetMouseButtonDown(0))
                    return;
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition); //create are raycast to target
                if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, LayerMask.GetMask("Place"))&&_state.Value.PreparedSystems)
                {
                    if (hit.transform.childCount >= 1) //find the object under finger and save it
                    {
                        var unit = hit.transform.GetComponentInChildren<EcsInfoMB>();
                        ref var unitComp = ref _dragUnitPool.Value.Add(entity);
                        unitComp.entity = unit.Entity;
                        unitComp.defaultPos = new Vector3(hit.transform.position.x, hit.transform.position.y + 1, hit.transform.position.z);
                        unitComp.defaultRot = hit.transform.rotation.normalized;
                        unitComp.unit = unit.gameObject;
                        _movablePool.Value.Get(unitComp.entity).NavMeshAgent.enabled = false;
                        unitComp.unit.GetComponent<Collider>().enabled = false;
                        unitComp.defaultParent = unitComp.unit.transform.parent;
                        unitComp.unit.transform.parent = null;
                        _touchPool.Value.Add(entity);
                        Debug.Log($"StartDrag, {unit.Entity}");
                    }
                }

                //Set up the new Pointer Event
                inputComp.PointerEventData = new PointerEventData(inputComp.EventSystem);
                //Set the Pointer Event Position to that of the mouse position
                inputComp.PointerEventData.position = Input.mousePosition;
                //Create a list of Raycast Results
                List<RaycastResult> results = new List<RaycastResult>();
                //Raycast using the Graphics Raycaster and mouse click position
                inputComp.Raycaster.Raycast(inputComp.PointerEventData, results);
                //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
                
                foreach (RaycastResult result in results)
                {
                    if (result.gameObject.CompareTag("Card") && _state.Value.HubSystems && _state.Value.inCollection)
                    {
                        ref var waitComp = ref _waitPool.Value.Add(_world.Value.NewEntity());
                        waitComp.CardObject = result.gameObject;
                        waitComp.DefaultPos = result.gameObject.transform.position;
                        waitComp.DefaultParent = result.gameObject.transform.parent;
                        waitComp.dragPosition = Input.mousePosition;
                        waitComp.timerDrag = 0.4f;
                        Debug.Log("Hit " + result.gameObject.name);
                    }
                    else if (result.gameObject.CompareTag("Card") && _state.Value.HubSystems && !_state.Value.inCollection)
                    {
                        interfaceComp.MainMenu.ToCollection();
                    }
                }
            }
            //Debug.Log("Touch!");
            if (Input.GetMouseButtonUp(0)) 
            {
                foreach (var touch in _touchFilter.Value)
                {
                    _touchFilter.Pools.Inc1.Del(touch);
                }
            } 
        }
    }
}