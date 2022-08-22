using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class ResetIsTargetComponentAfterDeath : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<IsTarget, DeadTag>> _deadTargetFilter = default;

        readonly EcsPoolInject<IsTarget> _isTargetPool = default;
        readonly EcsPoolInject<Targetable> _targetablePool = default;

        private int _deadTargetEntity = BattleState.NULL_ENTITY;

        public void Run (IEcsSystems systems)
        {
            foreach (var deadTargetEntity in _deadTargetFilter.Value)
            {
                _deadTargetEntity = deadTargetEntity;

                ref var isTarget = ref _isTargetPool.Value.Get(deadTargetEntity);

                foreach (var targetableEntity in isTarget.OfEntitys)
                {
                    ClearTargetableComponent(targetableEntity);
                }

                isTarget.OfEntitys.Clear();

                _deadTargetEntity = BattleState.NULL_ENTITY;

                _isTargetPool.Value.Del(deadTargetEntity);
            }
        }

        private void ClearTargetableComponent(int entity)
        {
            ref var targetableComponent = ref _targetablePool.Value.Get(entity);
            targetableComponent.TargetEntity = BattleState.NULL_ENTITY;
            targetableComponent.TargetObject = null;
            targetableComponent.EntitysInDetectionZone?.Remove(_deadTargetEntity);
            targetableComponent.EntitysInMeleeZone?.Remove(_deadTargetEntity);
            targetableComponent.EntitysInRangeZone?.Remove(_deadTargetEntity);
        }
    }
}