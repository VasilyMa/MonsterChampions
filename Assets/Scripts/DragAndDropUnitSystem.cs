using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
namespace Client {
    sealed class DragAndDropUnitSystem : IEcsRunSystem {
        readonly EcsWorldInject _world = default;
        readonly EcsFilterInject<Inc<TouchEvent, DragAndDropUnitComponent>> _touchFilter = default;
        readonly EcsPoolInject<DragAndDropUnitComponent> _unitPool = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        readonly EcsPoolInject<MergeUnitEvent> _mergeEventPool = default;
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
                }
                if (Input.GetMouseButtonUp(0))
                {
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, LayerMask.GetMask("Board")))
                    {
                        if (hit.transform.childCount >= 1)
                        {
                            if (hit.transform.GetComponentInChildren<UnitMB>().unitID == viewComp.EcsInfoMB.unitID)
                            {
                                ref var mergeComp = ref _mergeEventPool.Value.Add(_world.Value.NewEntity());
                                mergeComp.EntityfirstUnit = entity;
                                mergeComp.EntitysecondUnit = hit.transform.GetComponentInChildren<UnitMB>().Entity;
                            }
                            else
                            {
                                viewComp.Transform.position = _unitPool.Value.Get(entity).defaultPos;
                                viewComp.Transform.rotation = _unitPool.Value.Get(entity).defaultRot;
                                viewComp.GameObject.GetComponent<Collider>().enabled = true;
                            }
                        }
                        else
                        {
                            viewComp.Transform.position = new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y + 1.5f, hit.collider.transform.position.z);
                            viewComp.Transform.SetParent(hit.collider.transform);
                            viewComp.GameObject.GetComponent<Collider>().enabled = true;
                        }
                    }
                    else
                    {
                        viewComp.Transform.position = _unitPool.Value.Get(entity).defaultPos;
                        viewComp.Transform.rotation = _unitPool.Value.Get(entity).defaultRot;
                        viewComp.GameObject.GetComponent<Collider>().enabled = true;
                    }
                    _touchFilter.Pools.Inc2.Del(entity);
                    _touchFilter.Pools.Inc1.Del(entity);
                }
            }
        }
    }
}