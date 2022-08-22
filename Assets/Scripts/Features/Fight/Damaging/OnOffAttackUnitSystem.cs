using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class OnOffAttackUnitSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<InFightTag, UnitTag, Targetable, Animable>, Exc<DeadTag, OnBoardUnitTag>> _unitInFightFilter = default;

        readonly EcsPoolInject<Targetable> _targetablePool = default;
        readonly EcsPoolInject<Animable> _animablePool = default;

        private bool _currentMeleeFlag;
        private bool _currentRangeFlag;

        private bool _neededMeleeFlag = false;
        private bool _neededRangeFlag = false;

        public void Run (IEcsSystems systems)
        {
            foreach (var unitEntity in _unitInFightFilter.Value)
            {
                ref var targetableComponent = ref _targetablePool.Value.Get(unitEntity);
                ref var animableComponent = ref _animablePool.Value.Get(unitEntity);

                _currentMeleeFlag = animableComponent.Animator.GetBool(nameof(animableComponent.Melee));
                _currentRangeFlag = animableComponent.Animator.GetBool(nameof(animableComponent.Range));

                if (targetableComponent.EntitysInMeleeZone.Count > 0)
                {
                    _neededMeleeFlag = true;
                }

                if (targetableComponent.EntitysInRangeZone.Count > 0)
                {
                    _neededRangeFlag = true;
                }

                if (_neededMeleeFlag != _currentMeleeFlag)
                {
                    animableComponent.Animator.SetBool(nameof(animableComponent.Melee), _neededMeleeFlag);
                }

                if (_neededRangeFlag != _currentRangeFlag)
                {
                    animableComponent.Animator.SetBool(nameof(animableComponent.Range), _neededRangeFlag);
                }

                _currentMeleeFlag = false;
                _currentRangeFlag = false;

                _neededMeleeFlag = false;
                _neededRangeFlag = false;
            }
        }
    }
}