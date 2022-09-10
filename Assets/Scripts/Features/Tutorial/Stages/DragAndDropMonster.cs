using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using DG.Tweening;

namespace Client
{
    sealed class DragAndDropMonster : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _gameState = default;

        readonly EcsFilterInject<Inc<TutorialComponent>> _tutorialFilter = default;
        readonly EcsFilterInject<Inc<UnitTag, OnBoardUnitTag, ViewComponent>> _unitsOnBoardFilter = default;

        readonly EcsPoolInject<TutorialComponent> _tutorialPool = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        readonly EcsPoolInject<CameraComponent> _cameraPool = default;

        private Sequence _sequence;

        private Vector3 _dropPositiom = new Vector3(0, 0, 10);

        private bool _isEnabledUI = false;
        private bool _textIsChanged = false;

        public void Run (IEcsSystems systems)
        {
            if (Tutorial.CurrentStage != Tutorial.Stage.DragAndDropMonster)
            {
                return;
            }

            if (!Tutorial.StageIsEnable)
            {
                Tutorial.StageIsEnable = true;
                _gameState.Value.FightSystems = false;
            }

            if (!_isEnabledUI)
            {
                EnableUI();
                DoAnimation();
            }

            if (!_textIsChanged && Tutorial.DragAndDropMonster.isTryDropSoFar())
            {
                ChangeText();
            }

            if (Tutorial.DragAndDropMonster.isDropped())
            {
                EndStage();
            }
        }

        private void EnableUI()
        {
            foreach (var interfaceEntity in _tutorialFilter.Value)
            {
                ref var tutorialComponent = ref _tutorialPool.Value.Get(interfaceEntity);

                tutorialComponent.Hand.gameObject.SetActive(true);
                tutorialComponent.Focus.gameObject.SetActive(true);
                tutorialComponent.Message.gameObject.SetActive(true);
                tutorialComponent.MessageText.text = "Drag and drop\nmonster\non the floor!";

                _isEnabledUI = true;
            }
        }

        private void DoAnimation()
        {
            _sequence = DOTween.Sequence();

            ref var tutorialComponent = ref _tutorialPool.Value.Get(_gameState.Value.InterfaceEntity);

            var positionOne = Vector3.zero;
            var positionTwo = _dropPositiom;

            foreach (var unitEntity in _unitsOnBoardFilter.Value)
            {
                ref var viewComponent = ref _viewPool.Value.Get(unitEntity);
                positionOne = viewComponent.Transform.position;
            }

            ref var cameraCompoennt = ref _cameraPool.Value.Get(_gameState.Value.CameraEntity);

            var tablePositionOne = cameraCompoennt.Camera.WorldToScreenPoint(positionOne);
            var tablePositionTwo = cameraCompoennt.Camera.WorldToScreenPoint(positionTwo);

            tutorialComponent.Hand.position = tablePositionOne;

            var positionForMessage = Vector3.Lerp(tablePositionOne, tablePositionTwo, 0.5f);
            tutorialComponent.Focus.position = positionForMessage;
            tutorialComponent.Message.position = positionForMessage;
            tutorialComponent.MessageRectTransform.pivot = new Vector2(0.5f, 1);

            _sequence.Append(tutorialComponent.Hand.transform.DOMove(tablePositionTwo, 1.5f));
            _sequence.Append(tutorialComponent.Hand.transform.DOScale(0.8f, 0.5f));
            _sequence.Append(tutorialComponent.Hand.transform.DOScale(1f, 0.5f));
            _sequence.Append(tutorialComponent.Hand.transform.DOMove(tablePositionOne, 1.5f));
            _sequence.Append(tutorialComponent.Hand.transform.DOScale(0.8f, 0.5f));
            _sequence.Append(tutorialComponent.Hand.transform.DOScale(1f, 0.5f));
            _sequence.SetLoops(-1);
        }

        private void ChangeText()
        {
            ref var tutorialComponent = ref _tutorialPool.Value.Get(_gameState.Value.InterfaceEntity);

            tutorialComponent.MessageText.text = "You try\nso far!";

            _textIsChanged = true;
        }

        private void EndStage()
        {
            ref var tutorialComponent = ref _tutorialPool.Value.Get(_gameState.Value.InterfaceEntity);

            tutorialComponent.Hand.gameObject.SetActive(false);
            tutorialComponent.Focus.gameObject.SetActive(false);
            tutorialComponent.MessageText.gameObject.SetActive(false);

            _gameState.Value.FightSystems = true;

            _sequence.Kill();
            tutorialComponent.Hand.transform.localScale = Vector3.one;

            Tutorial.StageIsEnable = false;
            Tutorial.SetNextStage(_gameState, isSave: false);
        }
    }
}