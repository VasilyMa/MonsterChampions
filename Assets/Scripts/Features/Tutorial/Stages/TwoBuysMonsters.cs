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

        private bool _animationIsWork = false;

        public void Run (IEcsSystems systems)
        {
            if (Tutorial.CurrentStage != Tutorial.Stage.TwoBuysMonsters)
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

            foreach (var interfaceEntity in _tutorialFilter.Value)
            {
                ref var tutorialComponent = ref _tutorialPool.Value.Get(interfaceEntity);

                tutorialComponent.Hand.gameObject.SetActive(true);
                tutorialComponent.Focus.gameObject.SetActive(true);

                ref var interfaceComponent = ref _interfacePool.Value.Get(_gameState.Value.InterfaceEntity);

                var cardTransform = interfaceComponent.HolderCards.GetChild(0);

                tutorialComponent.Hand.position = cardTransform.position;
                tutorialComponent.Focus.position = cardTransform.position;

                if (!_animationIsWork)
                {
                    DoAnimation();
                }
            }

            if (Tutorial.TwoBuysMonsters.GetBuysValue() >= Tutorial.TwoBuysMonsters.GetMaxBuysValue())
            {
                EndStage();
            }
        }

        private void DoAnimation()
        {
            ref var tutorialComponent = ref _tutorialPool.Value.Get(_gameState.Value.InterfaceEntity);

            tutorialComponent.Hand.transform.DOScale(0.8f, 1).SetLoops(-1, LoopType.Yoyo);

            _animationIsWork = true;
        }

        private void EndStage()
        {
            ref var tutorialComponent = ref _tutorialPool.Value.Get(_gameState.Value.InterfaceEntity);

            tutorialComponent.Hand.gameObject.SetActive(false);
            tutorialComponent.Focus.gameObject.SetActive(false);

            _gameState.Value.FightSystems = true;

            Tutorial.StageIsEnable = false;
            Tutorial.SetNextStage(_gameState);
        }
    }
}