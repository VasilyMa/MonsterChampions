using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client {
    sealed class InputSystem : IEcsRunSystem {

        readonly EcsFilterInject<Inc<InputComponent>> _inputFilter = default;
        readonly EcsFilterInject<Inc<TouchEvent>> _touchFilter = default;
        readonly EcsPoolInject<TouchEvent> _touchPool = default;
        readonly EcsPoolInject<DragAndDropUnitComponent> _dragUnitPool = default;
        public void Run (IEcsSystems systems) {
            foreach (var entity in _inputFilter.Value)
            {
                if (!Input.GetMouseButtonDown(0))
                    return;
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition); //create are raycast to target
                if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, LayerMask.GetMask("Board")))
                {
                    try
                    {
                        if (hit.transform.childCount >= 1) //find the object under finger and save it
                        {
                            var unit = hit.transform.GetComponentInChildren<UnitMB>();
                            ref var unitComp = ref _dragUnitPool.Value.Add(entity);
                            unitComp.entity = unit.Entity;
                            unitComp.defaultPos = new Vector3(hit.transform.position.x, hit.transform.position.y + 1, hit.transform.position.z);
                            unitComp.defaultRot = hit.transform.rotation.normalized;
                            Debug.Log($"StartDrag, {unit.Entity}");
                            _touchPool.Value.Add(entity);
                        }
                    }
                    catch (System.Exception)
                    {
                        throw;
                    }
                }
                Debug.Log("Touch!");
                foreach (var touch in _touchFilter.Value)
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        _touchFilter.Pools.Inc1.Del(touch);
                    }
                }
            }
        }
    }
}