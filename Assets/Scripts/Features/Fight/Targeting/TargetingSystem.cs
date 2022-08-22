using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace Client
{
    sealed class TargetingSystem : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _battleState;

        readonly EcsFilterInject<Inc<Targetable, FractionComponent>, Exc<UntargetableTag, OnBoardUnitTag>> _targetableFilter = default;

        readonly EcsPoolInject<Targetable> _targetablePool = default;
        readonly EcsPoolInject<FractionComponent> _fractionPool = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        readonly EcsPoolInject<BaseTag> _basePool = default;
        readonly EcsPoolInject<UntargetableTag> _untargetablePool = default;
        readonly EcsPoolInject<IsTarget> _isTargetPool = default;
        private int targetEntity = BattleState.NULL_ENTITY;

        public void Run (IEcsSystems systems)
        {
            foreach (var targetabelEntity in _targetableFilter.Value)
            {
                ref var targetableComponent = ref _targetablePool.Value.Get(targetabelEntity);
                ref var fractionComponent = ref _fractionPool.Value.Get(targetabelEntity);

                if (CheckTargetIsExistAndIsNotBase(targetableComponent.TargetEntity))
                {
                    continue;
                }

                if (targetableComponent.EntitysInDetectionZone.Count > 0)
                {
                    targetEntity = targetableComponent.EntitysInDetectionZone[0];
                }

                if (BattleState.isNullableEntity(targetEntity))
                {
                    SetTargetEntityAsBase(targetabelEntity);
                }

                if (BattleState.isNullableEntity(targetEntity))
                {
                    _untargetablePool.Value.Add(targetabelEntity);
                    continue;
                }

                AddIsTargetComponent(targetEntity, targetabelEntity);
                
                targetableComponent.TargetEntity = targetEntity;
                targetableComponent.TargetObject = _viewPool.Value.Get(targetEntity).GameObject;

                ClearTargetEntityField();
            }
        }

        private void ClearTargetEntityField()
        {
            targetEntity = BattleState.NULL_ENTITY;
        }

        private void AddIsTargetComponent(int entity, int addedEntity)
        {
            if (!_isTargetPool.Value.Has(entity))
            {
                _isTargetPool.Value.Add(entity);
            }

            ref var isTarget = ref _isTargetPool.Value.Get(entity);

            if (isTarget.OfEntitys == null)
            {
                isTarget.OfEntitys = new List<int>();
            }

            isTarget.OfEntitys.Add(addedEntity);
        }

        private void SetTargetEntityAsBase(int targetabelEntity)
        {
            ref var fractionComponent = ref _fractionPool.Value.Get(targetabelEntity);

            if (fractionComponent.isFriendly)
            {
                targetEntity = _battleState.Value.GetEnemyBaseEntity();
            }
            else
            {
                targetEntity = _battleState.Value.GetPlayerBaseEntity();
            }
        }

        private bool CheckTargetIsExistAndIsNotBase(int entity)
        {
            return entity != BattleState.NULL_ENTITY && !_basePool.Value.Has(entity);
        }
    }
}