using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client {
    sealed class MergeUnitSystem : IEcsRunSystem {
        readonly EcsSharedInject<GameState> _state = default;
        readonly EcsFilterInject<Inc<MergeUnitEvent>> _mergeFilter = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        public void Run (IEcsSystems systems) {
            foreach (var entity in _mergeFilter.Value)
            {
                ref var mergeComp = ref _mergeFilter.Pools.Inc1.Get(entity);
                ref var viewCompFirst = ref _viewPool.Value.Get(mergeComp.EntityfirstUnit);
                ref var viewCompSecond = ref _viewPool.Value.Get(mergeComp.EntitysecondUnit);
            }
        }
    }
}