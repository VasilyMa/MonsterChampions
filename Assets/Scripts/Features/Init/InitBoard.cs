using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class InitBoard : IEcsInitSystem
    {
        readonly EcsWorldInject _world;

        readonly EcsPoolInject<ViewComponent> _viewPool = default;


        public void Init (IEcsSystems systems)
        {
            var board = GameObject.FindObjectOfType<BoardRagMB>()?.gameObject;

            if (board == null)
            {
                return;
            }

            var boardEntity = _world.Value.NewEntity();
            ref var viewComponent = ref _viewPool.Value.Add(boardEntity);
            viewComponent.EntityNumber = boardEntity;
            viewComponent.GameObject = board;
            viewComponent.Transform = board.transform;
        }
    }
}