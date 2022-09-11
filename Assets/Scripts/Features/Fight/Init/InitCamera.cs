using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;


namespace Client
{
    sealed class InitCamera : IEcsInitSystem
    {
        readonly EcsWorldInject _world;

        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsPoolInject<CameraComponent> _cameraPool = default;

        private string MainCamera;

        public void Init (IEcsSystems systems)
        {
            var cameraEntity = _world.Value.NewEntity();

            _gameState.Value.CameraEntity = cameraEntity;

            var cameraGameObject = GameObject.FindGameObjectWithTag(nameof(MainCamera));

            ref var cameraComponent = ref _cameraPool.Value.Add(cameraEntity);
            cameraComponent.CameraObject = cameraGameObject;
            cameraComponent.HolderObject = cameraGameObject.transform.parent.gameObject;
            cameraComponent.CameraTransform = cameraGameObject.transform;
            cameraComponent.HolderTransform = cameraGameObject.transform.parent.transform;
            cameraComponent.CameraAnimationCurve = cameraComponent.CameraObject.GetComponent<CameraMB>().AnimationCurve;
            cameraComponent.Camera = cameraComponent.CameraObject.GetComponent<Camera>();
        }
    }
}