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
        readonly EcsPoolInject<Movable> _movablePool = default;
        readonly EcsPoolInject<OnBoardUnitTag> _onBoardPool = default;
        public void Run (IEcsSystems systems) {
            foreach (var entity in _touchFilter.Value)
            {
                ref var unitComp = ref _unitPool.Value.Get(entity);
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
                    if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, LayerMask.GetMask("Place")))
                    {
                        if (hit.transform.childCount >= 1)
                        {
                            if (hit.transform.GetComponentInChildren<EcsInfoMB>().unitID == viewComp.EcsInfoMB.unitID)
                            {
                                ref var mergeComp = ref _mergeEventPool.Value.Add(_world.Value.NewEntity());
                                mergeComp.EntityfirstUnit = _unitPool.Value.Get(entity).entity;
                                mergeComp.EntitysecondUnit = hit.transform.GetComponentInChildren<EcsInfoMB>().Entity;
                            }
                            else
                            {
                                viewComp.Transform.position = _unitPool.Value.Get(entity).defaultPos;
                                viewComp.Transform.rotation = _unitPool.Value.Get(entity).defaultRot;
                                viewComp.Transform.parent = _unitPool.Value.Get(entity).defaultParent;
                                _movablePool.Value.Get(unitComp.entity).NavMeshAgent.enabled = true;
                                viewComp.GameObject.GetComponent<Collider>().enabled = true;
                            }
                        }
                        else
                        {
                            viewComp.Transform.position = new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y + 1.5f, hit.collider.transform.position.z);
                            viewComp.Transform.SetParent(hit.collider.transform);
                            _movablePool.Value.Get(unitComp.entity).NavMeshAgent.enabled = true;
                            viewComp.GameObject.GetComponent<Collider>().enabled = true;
                        }
                    }
                    else
                    {
                        viewComp.Transform.position = _unitPool.Value.Get(entity).defaultPos;
                        viewComp.Transform.rotation = _unitPool.Value.Get(entity).defaultRot;
                        viewComp.Transform.parent = _unitPool.Value.Get(entity).defaultParent;
                        _movablePool.Value.Get(unitComp.entity).NavMeshAgent.enabled = true;
                        viewComp.GameObject.GetComponent<Collider>().enabled = true;
                    }
                    if (Physics.Raycast(ray, out RaycastHit hitGround, float.MaxValue, LayerMask.GetMask("Ground")))
                    {
                        viewComp.Transform.parent = null;
                        _onBoardPool.Value.Del(_unitPool.Value.Get(entity).entity);
                    }
                    _touchFilter.Pools.Inc2.Del(entity);
                    _touchFilter.Pools.Inc1.Del(entity);
                }
            }
        }
    }
}