using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
namespace Client {
    sealed class BuyUnitSystem : IEcsRunSystem {
        readonly EcsFilterInject<Inc<BuyUnitEvent>> _buyFilter = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        readonly EcsPoolInject<Movable> _movablePool = default;
        readonly EcsPoolInject<UnitTag> _unitPool = default;
        readonly EcsPoolInject<OnBoardUnitTag> _onboardUnit = default;
        readonly EcsPoolInject<HealthComponent> _healthPool = default;
        readonly EcsPoolInject<ElementalComponent> _elementPool = default;

        public void Run (IEcsSystems systems) {
            foreach (var entity in _buyFilter.Value)
            {

            }
        }
    }
}