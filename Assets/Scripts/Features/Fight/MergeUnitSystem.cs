using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client {
    sealed class MergeUnitSystem : IEcsRunSystem {
        readonly EcsSharedInject<GameState> _state = default;
        readonly EcsFilterInject<Inc<MergeUnitEvent>> _mergeFilter = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        readonly EcsPoolInject<PhysicsComponent> _physicsPool = default;
        readonly EcsPoolInject<Animable> _animablePool = default;
        readonly EcsPoolInject<UnitTag> _unitPool = default;
        readonly EcsPoolInject<Movable> _movablePool = default;
        readonly EcsPoolInject<Targetable> _targetablePool = default;
        readonly EcsPoolInject<HealthComponent> _healthPool = default;
        readonly EcsPoolInject<ElementalComponent> _elementalPool = default;
        readonly EcsPoolInject<LevelComponent> _levelPool = default;
        readonly EcsPoolInject<DamageComponent> _damagePool = default;
        readonly EcsPoolInject<RangeUnitComponent> _rangeUnitPool = default;
        readonly EcsPoolInject<DroppingGoldComponent> _droppingGoldPool = default;

        private Vector3 _ebenya = new Vector3(0, 100, 0);

        public void Run (IEcsSystems systems)
        {
            foreach (var entity in _mergeFilter.Value)
            {
                ref var mergeComp = ref _mergeFilter.Pools.Inc1.Get(entity);
                ref var viewCompMain = ref _viewPool.Value.Get(mergeComp.EntitysecondUnit);
                ref var physicComp = ref _physicsPool.Value.Get(mergeComp.EntitysecondUnit);
                ref var unitCompMain = ref _unitPool.Value.Get(mergeComp.EntitysecondUnit);
                ref var movableComp = ref _movablePool.Value.Get(mergeComp.EntitysecondUnit);
                ref var targetComp = ref _targetablePool.Value.Get(mergeComp.EntitysecondUnit);
                ref var healthComp = ref _healthPool.Value.Get(mergeComp.EntitysecondUnit);
                ref var elementComp = ref _elementalPool.Value.Get(mergeComp.EntitysecondUnit);
                ref var levelComp = ref _levelPool.Value.Get(mergeComp.EntitysecondUnit);
                ref var damageComp = ref _damagePool.Value.Get(mergeComp.EntitysecondUnit);
                ref var animableComp = ref _animablePool.Value.Get(mergeComp.EntitysecondUnit);
                ref var droppingGoldComponent = ref _droppingGoldPool.Value.Get(mergeComp.EntitysecondUnit);

                // to do ay create objectPool for reuse models
                viewCompMain.Model.transform.SetParent(null);
                viewCompMain.Model.transform.position = _ebenya;

                viewCompMain.Model = GameObject.Instantiate(viewCompMain.VisualAndAnimations[levelComp.Value].ModelPrefab, viewCompMain.GameObject.transform.position, Quaternion.identity);
                viewCompMain.Model.transform.SetParent(viewCompMain.Transform);

                if (_rangeUnitPool.Value.Has(entity))
                {
                    ref var rangeUnitComponent = ref _rangeUnitPool.Value.Get(entity);

                    rangeUnitComponent.FirePoint = viewCompMain.Model.GetComponent<FirePointMB>().GetFirePoint();
                }

                animableComp.Animator.runtimeAnimatorController = viewCompMain.VisualAndAnimations[levelComp.Value].RuntimeAnimatorController;
                animableComp.Animator.avatar = viewCompMain.VisualAndAnimations[levelComp.Value].Avatar;
                // end to do ay

                levelComp.Value++;
                healthComp.MaxValue *= 2;
                healthComp.CurrentValue = healthComp.MaxValue;
                damageComp.Value *= 2;
                droppingGoldComponent.GoldValue *= 2;

                viewCompMain.HealthBarMB.UpdateHealth(healthComp.CurrentValue);

                GameObject.Instantiate(_state.Value.EffectsPool.ElementalEffects.GetElementalEffect(elementComp.CurrentType), viewCompMain.GameObject.transform); // to do ay write normal'nii Quaternion

                //
                ref var viewCompFirst = ref _viewPool.Value.Get(mergeComp.EntityfirstUnit);//there delete another unit and entity
                GameObject.Destroy(viewCompFirst.GameObject);
                _damagePool.Value.Del(mergeComp.EntityfirstUnit);
                _levelPool.Value.Del(mergeComp.EntityfirstUnit);
                _elementalPool.Value.Del(mergeComp.EntityfirstUnit);
                _healthPool.Value.Del(mergeComp.EntityfirstUnit);
                _targetablePool.Value.Del(mergeComp.EntityfirstUnit);
                _unitPool.Value.Del(mergeComp.EntityfirstUnit);
                _movablePool.Value.Del(mergeComp.EntityfirstUnit);
                _physicsPool.Value.Del(mergeComp.EntityfirstUnit);
                _viewPool.Value.Del(mergeComp.EntityfirstUnit);
                _animablePool.Value.Del(mergeComp.EntityfirstUnit);
                //
                _mergeFilter.Pools.Inc1.Del(entity);

                if (Tutorial.CurrentStage == Tutorial.Stage.MergeMonsters)
                {
                    Tutorial.MergeMonsters.SetMonstersIsMerged();
                }
            }
        }
    }
}