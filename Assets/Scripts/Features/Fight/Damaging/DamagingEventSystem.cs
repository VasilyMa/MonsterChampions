using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class DamagingEventSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<DamagingEvent>> _damagingEventFilter = default;

        readonly EcsPoolInject<DamagingEvent> _damagingEventPool = default;
        readonly EcsPoolInject<DieEvent> _dieEventPool = default;

        readonly EcsPoolInject<HealthComponent> _healthPool = default;
        readonly EcsPoolInject<LevelComponent> _levelPool = default;
        readonly EcsPoolInject<ElementalComponent> _elementalPool = default;
        readonly EcsPoolInject<BaseTag> _baseTagPool = default;
        readonly EcsPoolInject<UnitTag> _unitTagPool = default;

        public void Run (IEcsSystems systems)
        {
            foreach (var damagingEventEntity in _damagingEventFilter.Value)
            {
                ref var damagingEvent = ref _damagingEventPool.Value.Get(damagingEventEntity);
                ref var healthComponent = ref _healthPool.Value.Get(damagingEvent.DamageEntity);

                if (_unitTagPool.Value.Has(damagingEvent.DamageEntity))
                {
                    DoDamageToUnit();
                }
                else if (_baseTagPool.Value.Has(damagingEvent.DamageEntity))
                {
                    DoDamageToBase(damagingEvent.DamageEntity, damagingEvent.WhoDoDamageEntity);
                }
                else
                {
                    Debug.LogError($"Error with «{damagingEvent.DamageEntity}» entity. It's don't have «BaseTag» or «UnitTag»");
                    DeleteEvent(damagingEventEntity);
                    continue;
                }

                if (healthComponent.CurrentValue <= 0)
                {
                    _dieEventPool.Value.Add(damagingEvent.DamageEntity);
                }

                DeleteEvent(damagingEventEntity);
            }
        }

#BaseDamage
        private void DoDamageToBase(int baseEntity, int unitEntity)
        {
            ref var baseHealthComponent = ref _healthPool.Value.Get(baseEntity);

            ref var unitLevelComponent = ref _levelPool.Value.Get(unitEntity);

            float damageToBase = CalculateDamageToBase(unitLevelComponent.Value);

            if (damageToBase > baseHealthComponent.CurrentValue)
            {
                damageToBase = baseHealthComponent.CurrentValue;
            }

            baseHealthComponent.CurrentValue -= damageToBase;
        }

        private float CalculateDamageToBase(int unitLevel)
        {
            return Mathf.Pow(2, unitLevel - 1);
        }
#BaseDamage
        private void DoDamageToUnit()
        {
            //to do ay unit damage
        }

        private void DeleteEvent(int damagingEventEntity)
        {
            _damagingEventPool.Value.Del(damagingEventEntity);
        }
    }
}