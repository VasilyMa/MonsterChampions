using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class HealthBarLookToCamera : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsFilterInject<Inc<HealthComponent, UnitTag>, Exc<DeadTag, OnBoardUnitTag>> _aliveUnitsFilter = default;

        readonly EcsPoolInject<HealthComponent> _healthPool = default;
        readonly EcsPoolInject<CameraComponent> _cameraPool = default;

        public void Run (IEcsSystems systems)
        {
            foreach (var unitEntity in _aliveUnitsFilter.Value)
            {
                ref var healthComponent = ref _healthPool.Value.Get(unitEntity);

                ref var cameraComponent = ref _cameraPool.Value.Get(_gameState.Value.CameraEntity);

                if (healthComponent.HealthBar == null)
                {
                    Debug.LogWarning($"Для {unitEntity} небыло прописано наличие хпбара");
                    continue;
                }

                healthComponent.HealthBar.transform.LookAt(cameraComponent.CameraTransform);
            }
        }
    }
}