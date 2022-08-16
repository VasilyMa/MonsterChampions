using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class InitBoard : IEcsInitSystem
    {
        readonly EcsWorldInject _world;
        readonly EcsSharedInject<GameState> _state = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;


        public void Init (IEcsSystems systems)
        {
            var board = GameObject.FindObjectOfType<BoardRagMB>()?.gameObject;

            if (board == null)
            {
                return;
            }

            var boardEntity = _world.Value.NewEntity();
            _state.Value.BoardEntity = boardEntity;
            ref var viewComponent = ref _viewPool.Value.Add(boardEntity);
            viewComponent.EntityNumber = boardEntity;
            viewComponent.GameObject = board;
            viewComponent.Transform = board.transform;
        }
    }
}