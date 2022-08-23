using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client {
    sealed class MergeUnitSystem : IEcsRunSystem {
        readonly EcsSharedInject<GameState> _state = default;
        readonly EcsFilterInject<Inc<MergeUnitEvent>> _mergeFilter = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        readonly EcsPoolInject<PhysicsComponent> _physicsPool = default;
        readonly EcsPoolInject<UnitTag> _unitPool = default;
        readonly EcsPoolInject<Movable> _movablePool = default;
        readonly EcsPoolInject<Targetable> _targetablePool = default;
        readonly EcsPoolInject<HealthComponent> _healthPool = default;
        readonly EcsPoolInject<ElementalComponent> _elementalPool = default;
        readonly EcsPoolInject<LevelComponent> _levelPool = default;
        readonly EcsPoolInject<DamageComponent> _damagePool = default;
        public void Run (IEcsSystems systems) {
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
                //to do ay write there leveling






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
                //
                _mergeFilter.Pools.Inc1.Del(entity);
            }
        }
    }
}