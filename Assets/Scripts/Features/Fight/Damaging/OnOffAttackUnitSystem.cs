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

        public void Run (IEcsSystems systems)
        {
            foreach (var unitEntity in _unitInFightFilter.Value)
            {
                ref var targetableComponent = ref _targetablePool.Value.Get(unitEntity);


            }
        }
    }
}