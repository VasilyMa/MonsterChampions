using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class InitInput : IEcsInitSystem
    {
        readonly EcsWorldInject _world = default;

        readonly EcsSharedInject<GameState> _state = default;

        readonly EcsPoolInject<InputComponent> _inputPool = default;

        public void Init (IEcsSystems systems)
        {
            var entity = _world.Value.NewEntity();
            _state.Value.InputEntity = entity;

            _inputPool.Value.Add(entity);
        }
    }
}