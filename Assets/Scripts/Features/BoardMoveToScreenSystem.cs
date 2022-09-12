using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class BoardMoveToScreenSystem : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _gameState = default;

        readonly EcsFilterInject<Inc<CameraComponent>> _cameraFilter = default;

        readonly EcsPoolInject<CameraComponent> _cameraPool = default;

        readonly EcsPoolInject<ViewComponent> _viewPool = default;

        private Vector3 _startPosition;
        private Vector3 _endPosition = new Vector3(-1.7f, -8.8f, 19f);

        private float _currentTime = 0;
        private float _timeToMove = 0;
        private float _totalTime = 0;

        private bool _boardIsOnScreen = false;
        private bool _firstStart = true;

        public void Run (IEcsSystems systems)
        {
            if (_boardIsOnScreen)
            {
                return;
            }

            foreach (var cameraEntity in _cameraFilter.Value)
            {
                ref var cameraComponent = ref _cameraPool.Value.Get(cameraEntity);

                ref var boardViewComponent = ref _viewPool.Value.Get(_gameState.Value.BoardEntity);

                if (_firstStart)
                {
                    _totalTime = cameraComponent.CameraAnimationCurve.keys[cameraComponent.CameraAnimationCurve.keys.Length - 1].time;

                    _startPosition = boardViewComponent.Transform.localPosition;

                    _firstStart = false;
                }

                _currentTime = cameraComponent.CameraAnimationCurve.Evaluate(_timeToMove);

                boardViewComponent.Transform.localPosition = Vector3.Lerp(_startPosition, _endPosition, _currentTime);

                _timeToMove += Time.deltaTime;

                if (_timeToMove >= _totalTime)
                {
                    _boardIsOnScreen = true;
                }
            }
        }
    }
}