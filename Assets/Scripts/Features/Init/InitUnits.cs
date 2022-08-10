using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.AI;

namespace Client
{
    sealed class InitUnits : IEcsInitSystem
    {
        readonly EcsWorldInject _world;

        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        readonly EcsPoolInject<Unit> _unitPool = default;
        readonly EcsPoolInject<Movable> _movablePool = default;

        public void Init (IEcsSystems systems)
        {
            var allUnitsMB = GameObject.FindObjectsOfType<UnitMB>();

            foreach (var unitsMB in allUnitsMB)
            {
                int unitEntity = _world.Value.NewEntity();

                ref var viewComponent = ref _viewPool.Value.Add(unitEntity);
                viewComponent.EntityNumber = unitEntity;

                viewComponent.GameObject = unitsMB.gameObject;
                viewComponent.Transform = viewComponent.GameObject.transform;
                viewComponent.Rigidbody = viewComponent.GameObject.GetComponent<Rigidbody>();

                ref var unitComponent = ref _unitPool.Value.Add(unitEntity);
                unitComponent.isFriendly = unitsMB.IsFriendly;

                ref var movableComponent = ref _movablePool.Value.Add(unitEntity);
                movableComponent.NavMeshAgent = viewComponent.GameObject.GetComponent<NavMeshAgent>();
                movableComponent.NavMeshAgent.speed = 10;

                movableComponent.NavMeshAgent.enabled = false;
            }
        }
    }
}