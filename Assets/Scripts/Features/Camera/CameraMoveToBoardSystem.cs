using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CameraMoveToBoardSystem : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _gameState = default;

        readonly EcsFilterInject<Inc<CameraComponent>> _cameraFilter = default;

        readonly EcsPoolInject<CameraComponent> _cameraPool = default;

        private Vector3 _startPosition = new Vector3(50.5f, 35f, -8f);
        private Quaternion _startRotation = Quaternion.Euler(40, 297, 0);
        private float _startFieldOfView = 75;

        private Vector3 _endPosition = new Vector3(2, 19, 0);
        private Quaternion _endRotation = Quaternion.Euler(65, 356, 0);
        private float _endFieldOfView = 90;

        private float _currentTime = 0;
        private float _timeToMove = 0;
        private float _totalTime = 0;

        private int _cameraEntity = BattleState.NULL_ENTITY;

        private bool _cameraIsOnBoard = false;

        public void Run (IEcsSystems systems)
        {
            if (_cameraIsOnBoard)
            {
                return;
            }

            foreach (var cameraEntity in _cameraFilter.Value)
            {
                _cameraEntity = cameraEntity;

                ref var cameraComponent = ref _cameraPool.Value.Get(_cameraEntity);

                if (_timeToMove == 0)
                {
                    _totalTime = cameraComponent.CameraAnimationCurve.keys[cameraComponent.CameraAnimationCurve.keys.Length - 1].time;
                }

                _currentTime = cameraComponent.CameraAnimationCurve.Evaluate(_timeToMove);

                cameraComponent.CameraTransform.position = Vector3.Lerp(_startPosition, _endPosition, _currentTime);
                cameraComponent.CameraTransform.rotation = Quaternion.Slerp(_startRotation, _endRotation, _currentTime);
                cameraComponent.Camera.fieldOfView = Mathf.Lerp(_startFieldOfView, _endFieldOfView, _currentTime);

                _timeToMove += Time.deltaTime;

                if (_timeToMove >= _totalTime)
                {
                    EnableGame();
                }
            }
        }

        private void EnableGame()
        {
            ref var cameraComponent = ref _cameraPool.Value.Get(_cameraEntity);

            _cameraIsOnBoard = true;
            cameraComponent.isOnBoard = _cameraIsOnBoard;

            if (!Tutorial.StageIsEnable)
            {
                _gameState.Value.FightSystems = true;
            }
        }
    }
}