using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using DG.Tweening;

namespace Client
{
    sealed class DragAndDropNewCardInDeck : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _gameState = default;

        readonly EcsFilterInject<Inc<TutorialComponent, InterfaceComponent>> _tutorialFilter = default;

        readonly EcsPoolInject<TutorialComponent> _tutorialPool = default;
        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;

        private Vector3 _newCardPositoin;
        private Vector3 _deckPosition;

        private Sequence _sequence;

        private bool _isEnabledUI = false;
        private bool _messageChangedOnDroppedBack = false;
        private bool _messageIsChanged = false;

        public void Run(IEcsSystems systems)
        {
            if (Tutorial.CurrentStage != Tutorial.Stage.DragAndDropNewCardInDeck)
            {
                return;
            }

            if (!_gameState.Value.HubSystems)
            {
                return;
            }

            if (!Tutorial.DragAndDropNewCardInDeck.PanelIsOpened())
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
                IdentifyPositions();
                SetStartPositionsAndTextForUI();
                DoStartAnimation();

                _isEnabledUI = true;
            }

            if (!_messageChangedOnDroppedBack && Tutorial.DragAndDropNewCardInDeck.isDroppedBack())
            {
                SetStartPositionsAndTextForUI();
                DoStartAnimation();

                _messageChangedOnDroppedBack = true;
                _messageIsChanged = false;
            }

            if (!_messageIsChanged && Tutorial.DragAndDropNewCardInDeck.isDragged())
            {
                ChangeUI();
                ChangeAnimation();

                _messageIsChanged = true;
                _messageChangedOnDroppedBack = false;
            }

            if (Tutorial.DragAndDropNewCardInDeck.isDroppedInDeck())
            {
                EndStage();
            }
        }

        private void ChangeUI()
        {
            foreach (var interfaceEntity in _tutorialFilter.Value)
            {
                ref var tutorialComponent = ref _tutorialPool.Value.Get(interfaceEntity);

                tutorialComponent.MessageText.text = "Drag and drop\nin your deck!";

                ref var interfaceComponent = ref _interfacePool.Value.Get(interfaceEntity);

                var positionForMessage = Vector3.Lerp(_newCardPositoin, _deckPosition, 0.5f);

                tutorialComponent.Focus.position = _deckPosition;
                tutorialComponent.Message.position = positionForMessage;
                tutorialComponent.MessageRectTransform.pivot = new Vector2(0, 0.5f);
            }
        }

        private void ChangeAnimation()
        {
            ref var tutorialComponent = ref _tutorialPool.Value.Get(_gameState.Value.InterfaceEntity);

            _sequence.Kill();

            tutorialComponent.Hand.transform.localScale = Vector3.one;

            _sequence = DOTween.Sequence();

            _sequence.Append(tutorialComponent.Hand.transform.DOMove(_deckPosition, 2f));
            _sequence.SetLoops(-1);
        }

        private void IdentifyPositions()
        {
            foreach (var interfaceEntity in _tutorialFilter.Value)
            {
                ref var interfaceComponent = ref _interfacePool.Value.Get(interfaceEntity);

                _newCardPositoin = interfaceComponent.CollectionHolder.transform.GetComponentInChildren<CardInfo>().transform.position;
                _deckPosition = interfaceComponent.DeckHolder.position;
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
            }
        }

        private void SetStartPositionsAndTextForUI()
        {
            foreach (var interfaceEntity in _tutorialFilter.Value)
            {
                ref var tutorialComponent = ref _tutorialPool.Value.Get(interfaceEntity);

                tutorialComponent.Hand.position = _newCardPositoin;
                tutorialComponent.Focus.position = _newCardPositoin;
                tutorialComponent.Message.position = _newCardPositoin;
                tutorialComponent.MessageRectTransform.pivot = new Vector2(0.5f, 1);
                tutorialComponent.MessageText.text = "Take your\nnew card!";
            }
        }

        private void DoStartAnimation()
        {
            _sequence?.Kill();

            _sequence = DOTween.Sequence();

            ref var tutorialComponent = ref _tutorialPool.Value.Get(_gameState.Value.InterfaceEntity);

            tutorialComponent.Hand.transform.localScale = Vector3.one;

            _sequence.Append(tutorialComponent.Hand.transform.DOScale(0.7f, 2f));
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
            Tutorial.SetNextStage(_gameState);
        }
    }
}