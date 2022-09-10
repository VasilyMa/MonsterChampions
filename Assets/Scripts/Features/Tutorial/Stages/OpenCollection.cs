using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using DG.Tweening;

namespace Client
{
    sealed class OpenCollection : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _gameState = default;

        readonly EcsFilterInject<Inc<TutorialComponent>> _tutorialFilter = default;

        readonly EcsPoolInject<TutorialComponent> _tutorialPool = default;
        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;

        private Sequence _sequence;

        private bool _isEnabledUI = false;

        public void Run (IEcsSystems systems)
        {
            if (Tutorial.CurrentStage != Tutorial.Stage.OpenCollection)
            {
                return;
            }

            if (!_gameState.Value.HubSystems)
            {
                return;
            }

            if (!Tutorial.StageIsEnable)
            {
                Tutorial.StageIsEnable = true;
            }

            if (!_isEnabledUI)
            {
                EnableUI();
                DoAnimation();
            }

            if (Tutorial.OpenCollection.isOpened())
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
                tutorialComponent.MessageText.text = "Open your\ncollection!";

                ref var interfaceComponent = ref _interfacePool.Value.Get(_gameState.Value.InterfaceEntity);

                var focusPosition = interfaceComponent.MenuHolder.transform.GetChild(1).transform.position;

                tutorialComponent.Hand.position = focusPosition;
                tutorialComponent.Focus.position = focusPosition;
                tutorialComponent.Message.position = focusPosition;
                tutorialComponent.MessageRectTransform.pivot = new Vector2(1f, 1);

                _isEnabledUI = true;
            }
        }

        private void DoAnimation()
        {
            _sequence = DOTween.Sequence();

            ref var tutorialComponent = ref _tutorialPool.Value.Get(_gameState.Value.InterfaceEntity);

            _sequence.Append(tutorialComponent.Hand.transform.DOScale(0.8f, 0.5f));
            _sequence.SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        }

        private void EndStage()
        {
            ref var tutorialComponent = ref _tutorialPool.Value.Get(_gameState.Value.InterfaceEntity);

            tutorialComponent.Hand.gameObject.SetActive(false);
            tutorialComponent.Focus.gameObject.SetActive(false);
            tutorialComponent.MessageText.gameObject.SetActive(false);

            _sequence.Kill();
            tutorialComponent.Hand.transform.localScale = Vector3.one;

            Tutorial.StageIsEnable = false;
            Tutorial.SetNextStage(_gameState, isSave: false);
        }
    }
}