using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class DoBaseSoSmall : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _gameState = default;

        readonly EcsFilterInject<Inc<CameraComponent>> _cameraFilter = default;

        readonly EcsPoolInject<CameraComponent> _cameraPool = default;

        readonly EcsPoolInject<ViewComponent> _viewPool = default;

        private Vector3 _startScale = Vector3.one;
        private Vector3 _endScale = new Vector3(1f, 0.3f, 1f);

        private float _currentTime = 0;
        private float _timeToMove = 0;
        private float _totalTime = 0;

        private bool _baseIsSmall = false;
        private bool _firstStart = true;

        public void Run (IEcsSystems systems)
        {
            if (_baseIsSmall)
            {
                return;
            }

            ref var cameraComponent = ref _cameraPool.Value.Get(_gameState.Value.CameraEntity);

            if (_firstStart)
            {
                _totalTime = cameraComponent.CameraAnimationCurve.keys[cameraComponent.CameraAnimationCurve.keys.Length - 1].time;

                _firstStart = false;
            }

            _currentTime = cameraComponent.CameraAnimationCurve.Evaluate(_timeToMove);

            ref var baseViewComponent = ref _viewPool.Value.Get(_gameState.Value.GetPlayerBaseEntity());

            baseViewComponent.Model.transform.localScale = Vector3.Lerp(_startScale, _endScale, _currentTime);

            _timeToMove += Time.deltaTime;

            if (_timeToMove >= _totalTime)
            {
                _baseIsSmall = true;
            }
        }
    }
}