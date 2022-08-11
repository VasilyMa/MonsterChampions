using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
namespace Client {
    sealed class DragAndDropUnitSystem : IEcsRunSystem {
        readonly EcsFilterInject<Inc<TouchEvent, DragAndDropUnitComponent>> _touchFilter = default;
        readonly EcsPoolInject<DragAndDropUnitComponent> _unitPool = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        public void Run (IEcsSystems systems) {
            foreach (var entity in _touchFilter.Value)
            {
                ref var viewComp = ref _viewPool.Value.Get(_unitPool.Value.Get(entity).entity);
                if (Input.GetMouseButton(0))
                {
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, LayerMask.GetMask("Ground")))
                    {
                        var point = hit.point;
                        viewComp.Transform.position = new Vector3(point.x, 1.5f, point.z);
                    }
                    Debug.Log("GetTouch");
                }
                if (Input.GetMouseButtonUp(0))
                {
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, LayerMask.GetMask("Board")))
                    {
                        viewComp.Transform.position = hit.collider.transform.position;
                        viewComp.Transform.SetParent(hit.collider.transform);
                    }
                    else
                    {
                        viewComp.Transform.position = _unitPool.Value.Get(entity).defaultPos;
                        viewComp.Transform.rotation = _unitPool.Value.Get(entity).defaultRot;
                    }
                    Debug.Log("EndTouch!");
                    _touchFilter.Pools.Inc2.Del(entity);
                    _touchFilter.Pools.Inc1.Del(entity);
                }
            }
        }
    }
}