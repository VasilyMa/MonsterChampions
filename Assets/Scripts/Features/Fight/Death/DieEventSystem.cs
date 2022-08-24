using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class DieEventSystem : IEcsRunSystem
    {
        readonly EcsWorldInject _world = default;

        readonly EcsFilterInject<Inc<DieEvent>> _dieEventFilter = default;

        readonly EcsPoolInject<DieEvent> _dieEventPool = default;

        readonly EcsPoolInject<DeadTag> _deadPool = default;
        readonly EcsPoolInject<BaseTag> _basePool = default;
        readonly EcsPoolInject<Targetable> _targetablePool = default;
        readonly EcsPoolInject<Animable> _animablePool = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        readonly EcsPoolInject<IsTarget> _isTargetPool = default;
        readonly EcsPoolInject<Movable> _movablePool = default;
        readonly EcsPoolInject<FractionComponent> _fractionPool = default;
        readonly EcsPoolInject<UnitTag> _unitPool = default;

        readonly EcsPoolInject<DropGoldComponent> _dropGoldPool = default;
        readonly EcsPoolInject<DropGoldEvent> _dropGoldEventPool = default;
        readonly EcsPoolInject<WinEvent> _winEventPool = default;
        readonly EcsPoolInject<LoseEvent> _loseEventPool = default;

        private int _dyingEntity = BattleState.NULL_ENTITY;

        public void Run (IEcsSystems systems)
        {
            foreach (var dieEventEntity in _dieEventFilter.Value)
            {
                ref var dieEvent = ref _dieEventPool.Value.Get(dieEventEntity);

                _dyingEntity = dieEvent.DyingEntity;

                if (_deadPool.Value.Has(_dyingEntity))
                {
                    DeleteEvent(dieEventEntity);
                    continue;
                }

                DisableTargetableComponent();

                DisableMovableComponent();

                ClearTargetableComponentInEnemyUnits();

                PlayDeadAnimation();

                DropGold();

                SetDeadLayer();

                _deadPool.Value.Add(_dyingEntity);

                if (_basePool.Value.Has(_dyingEntity))
                {
                    InvokeWinOrLoseEvent();
                }

                

                DestroyUnit();

                DeleteEvent(dieEventEntity);
            }
        }

        private void DeleteEvent(int dieEventEntity)
        {
            _dieEventPool.Value.Del(dieEventEntity);
            _dyingEntity = BattleState.NULL_ENTITY;
        }

        private void PlayDeadAnimation()
        {
            if (!_animablePool.Value.Has(_dyingEntity))
            {
                return;
            }

            ref var animableComponent = ref _animablePool.Value.Get(_dyingEntity);
            animableComponent.Animator.SetTrigger(nameof(animableComponent.Die));
        }

        private void DropGold()
        {
            if (!_dropGoldPool.Value.Has(_dyingEntity))
            {
                return;
            }

            _dropGoldEventPool.Value.Add(_world.Value.NewEntity());
        }

        private void InvokeWinOrLoseEvent()
        {
            ref var fractionComponent = ref _fractionPool.Value.Get(_dyingEntity);

            if (fractionComponent.isFriendly)
            {
                _loseEventPool.Value.Add(_world.Value.NewEntity());
            }
            else
            {
                _winEventPool.Value.Add(_world.Value.NewEntity());
            }
        }

        private void DestroyUnit()
        {
            if (!_unitPool.Value.Has(_dyingEntity))
            {
                return;
            }

            if (!_viewPool.Value.Has(_dyingEntity))
            {
                return;
            }

            ref var viewComponent = ref _viewPool.Value.Get(_dyingEntity);

            viewComponent.GameObject.SetActive(false);
        }

        private void SetDeadLayer()
        {
            if (!_viewPool.Value.Has(_dyingEntity))
            {
                return;
            }

            ref var viewComponent = ref _viewPool.Value.Get(_dyingEntity);

            viewComponent.LayerBeforeDeath = viewComponent.GameObject.layer;
            viewComponent.GameObject.layer = LayerMask.NameToLayer(nameof(viewComponent.DeadUnit));

            if (viewComponent.Model != null)
            {
                viewComponent.Model.layer = LayerMask.NameToLayer(nameof(viewComponent.DeadUnit));
            }
        }

        private void DisableTargetableComponent()
        {
            if (!_targetablePool.Value.Has(_dyingEntity))
            {
                return;
            }

            ref var targetableComponent = ref _targetablePool.Value.Get(_dyingEntity);
            targetableComponent.TargetEntity = BattleState.NULL_ENTITY;
            targetableComponent.TargetObject = null;
            targetableComponent.EntitysInDetectionZone?.Clear();
            targetableComponent.EntitysInMeleeZone?.Clear();
            targetableComponent.EntitysInRangeZone?.Clear();
        }

        private void DisableMovableComponent()
        {
            if (!_movablePool.Value.Has(_dyingEntity))
            {
                return;
            }

            ref var movableComponent = ref _movablePool.Value.Get(_dyingEntity);
            movableComponent.NavMeshAgent.isStopped = true;
        }

        private void ClearTargetableComponentInEnemyUnits()
        {
            if (!_isTargetPool.Value.Has(_dyingEntity))
            {
                return;
            }

            ref var isTarget = ref _isTargetPool.Value.Get(_dyingEntity);

            foreach (var enemyEntity in isTarget.OfEntitys)
            {
                ref var enemyTargetableComponent = ref _targetablePool.Value.Get(enemyEntity);

                enemyTargetableComponent.TargetEntity = BattleState.NULL_ENTITY;
                enemyTargetableComponent.TargetObject = null;
            }
        }
    }
}