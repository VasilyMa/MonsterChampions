using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class StopUnitSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<Targetable, Movable, UnitTag>, Exc<DeadTag, OnBoardUnitTag>> _unitsFilter = default;

        readonly EcsPoolInject<Targetable> _targetablePool = default;
        readonly EcsPoolInject<PhysicsComponent> _physicsPool = default;
        readonly EcsPoolInject<Movable> _movablePool = default;
        readonly EcsPoolInject<IsForcedStoppedTag> _isForcedStoppedTagPool = default;

        private bool _enemyExistInDamageZone;
        private bool _hasStoppedTag;

        public void Run (IEcsSystems systems)
        {
            foreach (var unitEntity in _unitsFilter.Value)
            {
                _enemyExistInDamageZone = false;
                _hasStoppedTag = false;

                ref var targetableComponent = ref _targetablePool.Value.Get(unitEntity);
                ref var movableComponent = ref _movablePool.Value.Get(unitEntity);
                ref var physicsComponent = ref _physicsPool.Value.Get(unitEntity);

                _enemyExistInDamageZone = targetableComponent.EntitysInMeleeZone.Count > 0 || targetableComponent.EntitysInRangeZone.Count > 0;
                _hasStoppedTag = _isForcedStoppedTagPool.Value.Has(unitEntity);

                if (_enemyExistInDamageZone)
                {
                    if (!_hasStoppedTag)
                        _isForcedStoppedTagPool.Value.Add(unitEntity);

                    movableComponent.NavMeshAgent.ResetPath();
                }
                else
                {
                    if (_hasStoppedTag)
                        _isForcedStoppedTagPool.Value.Del(unitEntity);
                }
            }
        }
    }
}