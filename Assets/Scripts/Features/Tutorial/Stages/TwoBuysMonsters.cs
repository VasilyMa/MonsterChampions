using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using DG.Tweening;

namespace Client
{
    sealed class TwoBuysMonsters : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _gameState = default;

        readonly EcsFilterInject<Inc<TutorialComponent>> _tutorialFilter = default;

        readonly EcsPoolInject<TutorialComponent> _tutorialPool = default;
        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;
        readonly EcsPoolInject<CameraComponent> _cameraPool = default;

        private Sequence _sequence;

        private bool _isEnabledUI = false;
        private bool _textIsChanged = false;

        public void Run (IEcsSystems systems)
        {
            if (Tutorial.CurrentStage != Tutorial.Stage.TwoBuysMonsters)
            {
                return;
            }

            if (_gameState.Value.FightSystems == false && Tutorial.StageIsEnable == false)
            {
                return;
            }

            if (!Tutorial.StageIsEnable)
            {
                Tutorial.StageIsEnable = true;
                _gameState.Value.FightSystems = false;
            }

            ref var cameraComponent = ref _cameraPool.Value.Get(_gameState.Value.CameraEntity);

            if (!cameraComponent.isOnBoard)
            {
                return;
            }

            if (!_isEnabledUI)
            {
                EnableUI();
                DoAnimation();
            }

            if (!_textIsChanged && MonsterIsSpawningButNotAll())
            {
                ChangeText();
            }

            if (AllMonstersIsSpawning())
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

                ref var interfaceComponent = ref _interfacePool.Value.Get(_gameState.Value.InterfaceEntity);

                var cardTransform = interfaceComponent.HolderCards.GetChild(0);

                tutorialComponent.Hand.position = cardTransform.position;
                tutorialComponent.Focus.position = cardTransform.position;

                tutorialComponent.MessageRectTransform.pivot = new Vector2(1f, 0);
                tutorialComponent.Message.position = cardTransform.position;
                tutorialComponent.MessageText.text = "Buy Monster!";

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

        private bool MonsterIsSpawningButNotAll()
        {
            return Tutorial.TwoBuysMonsters.GetSpawnsValue() > 0 && !AllMonstersIsSpawning();
        }

        private void ChangeText()
        {
            ref var tutorialComponent = ref _tutorialPool.Value.Get(_gameState.Value.InterfaceEntity);

            tutorialComponent.MessageText.text = "And another one";

            _textIsChanged = true;
        }

        private bool AllMonstersIsSpawning()
        {
            return Tutorial.TwoBuysMonsters.GetSpawnsValue() >= Tutorial.TwoBuysMonsters.GetMaxSpawnsValue();
        }

        private void EndStage()
        {
            ref var tutorialComponent = ref _tutorialPool.Value.Get(_gameState.Value.InterfaceEntity);

            tutorialComponent.Hand.gameObject.SetActive(false);
            tutorialComponent.Focus.gameObject.SetActive(false);
            tutorialComponent.MessageText.gameObject.SetActive(false);

            _sequence.Kill();
            tutorialComponent.Hand.transform.localScale = Vector3.one;

            _gameState.Value.FightSystems = true;

            Tutorial.StageIsEnable = false;
            Tutorial.SetNextStage(_gameState, isSave: false);
        }
    }
}