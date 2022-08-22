using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class OnOffRunAminationSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<Movable, Animable>, Exc<DeadTag, OnBoardUnitTag>> _activeUnitsFilter = default;

        readonly EcsPoolInject<Movable> _movablePool = default;
        readonly EcsPoolInject<Animable> _animablePool = default;
        readonly EcsPoolInject<IsForcedStoppedTag> _isForcedStoppedTagPool = default;

        private bool _currentRunFlag;
        private bool _neededRunFlag;

        public void Run (IEcsSystems systems)
        {
            foreach (var unitEntity in _activeUnitsFilter.Value)
            {
                _currentRunFlag = false;
                _neededRunFlag = false;

                ref var movableComponent = ref _movablePool.Value.Get(unitEntity);
                ref var animableComponent = ref _animablePool.Value.Get(unitEntity);

                _currentRunFlag = animableComponent.Animator.GetBool(nameof(animableComponent.isRunning));
                _neededRunFlag = !_isForcedStoppedTagPool.Value.Has(unitEntity);

                if (_neededRunFlag != _currentRunFlag)
                {
                    animableComponent.Animator.SetBool(nameof(animableComponent.isRunning), _neededRunFlag);
                }
            }
        }
    }
}