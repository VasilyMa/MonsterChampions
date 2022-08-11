using Leopotam.EcsLite;
using UnityEngine;

namespace Client {
    sealed class InitInput : IEcsInitSystem {

        public void Init (IEcsSystems systems) {
            var world = systems.GetWorld();
            var state = systems.GetShared<GameState>();

            var entity = world.NewEntity();
            state.InputEntity = entity;

            world.GetPool<InputComponent>().Add(entity);
        }
    }
}