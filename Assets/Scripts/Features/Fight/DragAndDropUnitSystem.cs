using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
namespace Client {
    sealed class DragAndDropUnitSystem : IEcsRunSystem {
        readonly EcsWorldInject _world = default;
        readonly EcsFilterInject<Inc<TouchEvent, DragAndDropUnitComponent>> _touchFilter = default;
        readonly EcsPoolInject<DragAndDropUnitComponent> _unitPool = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        readonly EcsPoolInject<LevelComponent> _levelPool = default;
        readonly EcsPoolInject<MergeUnitEvent> _mergeEventPool = default;
        readonly EcsPoolInject<Movable> _movablePool = default;
        readonly EcsPoolInject<OnBoardUnitTag> _onBoardPool = default;
        readonly EcsPoolInject<HealthComponent> _healthPool = default;
        readonly EcsFilterInject<Inc<WinEvent>> _winPool = default;
        readonly EcsFilterInject<Inc<LoseEvent>> _losePool = default;

        private int _maxLevelForMerge = 4;

        public void Run (IEcsSystems systems) {
            foreach (var entity in _touchFilter.Value)
            {
                if (Tutorial.CurrentStage == Tutorial.Stage.TwoBuysMonsters)
                {
                    ReturnToDefault(entity);
                    _touchFilter.Pools.Inc2.Del(entity);
                    _touchFilter.Pools.Inc1.Del(entity);
                    break;
                }

                ref var unitComp = ref _unitPool.Value.Get(entity);
                ref var viewComp = ref _viewPool.Value.Get(_unitPool.Value.Get(entity).entity);
                if (Input.GetMouseButton(0))
                {
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition); //check when input
                    if (Physics.Raycast(ray, out RaycastHit hitGround, float.MaxValue, LayerMask.GetMask("Ground"))) 
                    {
                        var point = hitGround.point;
                        viewComp.Transform.position = new Vector3(point.x, point.y, point.z);
                    }
                    if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, LayerMask.GetMask("BoardRaycast")))
                    {
                        var point = hit.point;
                        viewComp.Transform.position = new Vector3(point.x, point.y, point.z);
                    }
                    foreach (var win in _winPool.Value)
                    {
                        ReturnToDefault(entity);
                        _touchFilter.Pools.Inc2.Del(entity);
                        _touchFilter.Pools.Inc1.Del(entity);
                        return;
                    }
                    foreach (var lose in _losePool.Value)
                    {
                        ReturnToDefault(entity);
                        _touchFilter.Pools.Inc2.Del(entity);
                        _touchFilter.Pools.Inc1.Del(entity);
                        return;
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
                                ref var levelComponentFirstUnit = ref _levelPool.Value.Get(_unitPool.Value.Get(entity).entity);
                                ref var levelComponentSecondUnit = ref _levelPool.Value.Get(hit.transform.GetComponentInChildren<EcsInfoMB>().Entity);

                                if (levelComponentFirstUnit.Value >= _maxLevelForMerge || levelComponentSecondUnit.Value >= _maxLevelForMerge)
                                {
                                    ReturnToDefault(entity);
                                    _touchFilter.Pools.Inc2.Del(entity);
                                    _touchFilter.Pools.Inc1.Del(entity);
                                    break;
                                }

                                if (levelComponentFirstUnit.Value == levelComponentSecondUnit.Value)
                                {
                                    ref var mergeComp = ref _mergeEventPool.Value.Add(_world.Value.NewEntity());
                                    mergeComp.EntityfirstUnit = _unitPool.Value.Get(entity).entity;
                                    mergeComp.EntitysecondUnit = hit.transform.GetComponentInChildren<EcsInfoMB>().Entity;
                                    
                                    _touchFilter.Pools.Inc2.Del(entity);
                                    _touchFilter.Pools.Inc1.Del(entity); 
                                    break;
                                }
                                else
                                {
                                    ReturnToDefault(entity);
                                    _touchFilter.Pools.Inc2.Del(entity);
                                    _touchFilter.Pools.Inc1.Del(entity); 
                                    break;
                                }
                            }
                            else
                            {
                                ReturnToDefault(entity);
                                _touchFilter.Pools.Inc2.Del(entity);
                                _touchFilter.Pools.Inc1.Del(entity);
                                break;
                            }
                        }
                        else
                        {
                            if (Tutorial.CurrentStage == Tutorial.Stage.MergeMonsters || Tutorial.CurrentStage == Tutorial.Stage.DragAndDropMonster)
                            {
                                ReturnToDefault(entity);
                                _touchFilter.Pools.Inc2.Del(entity);
                                _touchFilter.Pools.Inc1.Del(entity);
                                break;
                            }

                            viewComp.Transform.position = new Vector3(hit.collider.transform.position.x, hit.collider.transform.position.y + 1.5f, hit.collider.transform.position.z);
                            viewComp.Transform.SetParent(hit.collider.transform);
                            viewComp.GameObject.GetComponent<Collider>().enabled = true;
                            _touchFilter.Pools.Inc2.Del(entity);
                            _touchFilter.Pools.Inc1.Del(entity);
                            break;
                        }
                    }
                    if (Physics.Raycast(ray, out RaycastHit hitBoard, float.MaxValue, LayerMask.GetMask("BoardRaycast")))
                    {
                        if (Tutorial.CurrentStage == Tutorial.Stage.DragAndDropMonster)
                        {
                            Tutorial.DragAndDropMonster.SetTryingFarDrop();
                            ReturnToDefault(entity);
                            _touchFilter.Pools.Inc2.Del(entity);
                            _touchFilter.Pools.Inc1.Del(entity);
                            break;
                        }

                        TeleportToGround(entity);
                        _touchFilter.Pools.Inc2.Del(entity);
                        _touchFilter.Pools.Inc1.Del(entity);

                        break;
                    }
                    if (Physics.Raycast(ray, out RaycastHit hitGround, float.MaxValue, LayerMask.GetMask("Ground")))
                    {
                        if (Tutorial.CurrentStage == Tutorial.Stage.MergeMonsters)
                        {
                            ReturnToDefault(entity);
                            _touchFilter.Pools.Inc2.Del(entity);
                            _touchFilter.Pools.Inc1.Del(entity);
                            break;
                        }

                        ref var healthComp = ref _healthPool.Value.Get(_unitPool.Value.Get(entity).entity);
                        viewComp.HealthBarMB.gameObject.SetActive(true);
                        viewComp.HealthBarMB.SetMaxHealth(healthComp.MaxValue);
                        viewComp.Transform.parent = null; 
                        _movablePool.Value.Get(unitComp.entity).NavMeshAgent.enabled = true;
                        viewComp.GameObject.GetComponent<Collider>().enabled = true;
                        _onBoardPool.Value.Del(_unitPool.Value.Get(entity).entity);
                        _touchFilter.Pools.Inc2.Del(entity);
                        _touchFilter.Pools.Inc1.Del(entity);

                        viewComp.Model.transform.localRotation = Quaternion.Euler(Vector3.zero);

                        viewComp.GameObject.layer = LayerMask.NameToLayer(nameof(viewComp.AliveUnit));

                        if (Tutorial.CurrentStage == Tutorial.Stage.DragAndDropMonster)
                        {
                            Tutorial.DragAndDropMonster.SetMonsterIsDropped();
                        }

                        break;
                    }
                    _touchFilter.Pools.Inc2.Del(entity);
                    _touchFilter.Pools.Inc1.Del(entity);
                }
            }
        }
        private void TeleportToGround(int entity)
        {
            ref var unitComp = ref _unitPool.Value.Get(entity);
            ref var viewComp = ref _viewPool.Value.Get(_unitPool.Value.Get(entity).entity);

            ref var healthComp = ref _healthPool.Value.Get(_unitPool.Value.Get(entity).entity);
            viewComp.HealthBarMB.gameObject.SetActive(true);
            viewComp.HealthBarMB.SetMaxHealth(healthComp.MaxValue);

            viewComp.Transform.parent = null;
            viewComp.Transform.position = GameObject.Find("ForLanding").transform.position;

            _movablePool.Value.Get(unitComp.entity).NavMeshAgent.enabled = true;
            viewComp.GameObject.GetComponent<Collider>().enabled = true;
            _onBoardPool.Value.Del(_unitPool.Value.Get(entity).entity);
            _touchFilter.Pools.Inc2.Del(entity);
            _touchFilter.Pools.Inc1.Del(entity);

            viewComp.Model.transform.localRotation = Quaternion.Euler(Vector3.zero);

            viewComp.GameObject.layer = LayerMask.NameToLayer(nameof(viewComp.AliveUnit));
        }

        private void ReturnToDefault(int entity)
        {
            ref var unitComp = ref _unitPool.Value.Get(entity);
            ref var viewComp = ref _viewPool.Value.Get(_unitPool.Value.Get(entity).entity);

            viewComp.Transform.position = _unitPool.Value.Get(entity).defaultPos;
            viewComp.Transform.rotation = _unitPool.Value.Get(entity).defaultRot;
            viewComp.Transform.parent = _unitPool.Value.Get(entity).defaultParent;
            viewComp.GameObject.GetComponent<Collider>().enabled = true;
        }

        private void DeleteFilterEntity()
        {

        }
    }
}