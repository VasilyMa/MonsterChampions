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

        private const float LEVELING_STANDART_VALUE = 1f;
        private const float LEVELING_INCREASER = 0.5f;
        private const float LEVELING_DECREASER = 0.25f;

        public void Run (IEcsSystems systems)
        {
            foreach (var damagingEventEntity in _damagingEventFilter.Value)
            {
                ref var damagingEvent = ref _damagingEventPool.Value.Get(damagingEventEntity);
                ref var healthComponent = ref _healthPool.Value.Get(damagingEvent.UndergoEntity);

                if (_unitTagPool.Value.Has(damagingEvent.UndergoEntity))
                {
                    DoDamageToUnit(damagingEvent.UndergoEntity, damagingEvent.WhoDoDamageEntity, damagingEvent.DamageValue);
                }
                else if (_baseTagPool.Value.Has(damagingEvent.UndergoEntity))
                {
                    DoDamageToBase(damagingEvent.UndergoEntity, damagingEvent.WhoDoDamageEntity);
                }
                else
                {
                    Debug.LogError($"Error with «{damagingEvent.UndergoEntity}» entity. It's don't have «BaseTag» or «UnitTag»");
                    DeleteEvent(damagingEventEntity);
                    continue;
                }

                if (healthComponent.CurrentValue <= 0)
                {
                    _dieEventPool.Value.Add(damagingEvent.UndergoEntity);
                }

                DeleteEvent(damagingEventEntity);
            }
        }

#region BaseDamage
        private void DoDamageToBase(int baseEntity, int unitEntity)
        {
            ref var baseHealthComponent = ref _healthPool.Value.Get(baseEntity);

            ref var unitLevelComponent = ref _levelPool.Value.Get(unitEntity);

            float damageToBase = CalculateDamageToBase(unitLevelComponent.Value);

            damageToBase = MatchDamageComparedHealth(damageToBase, baseHealthComponent.CurrentValue);

            baseHealthComponent.CurrentValue -= damageToBase;
        }

        private float CalculateDamageToBase(int unitLevel)
        {
            return Mathf.Pow(2, unitLevel - 1);
        }
        #endregion BaseDamage

#region UnitDamage
        private void DoDamageToUnit(int undergoEntity, int whoDoDamageEntity, float damageValue)
        {
            ref var undergoUnitHealthComponent = ref _healthPool.Value.Get(undergoEntity);
            ref var undergoUnitLevelComponent = ref _levelPool.Value.Get(undergoEntity);
            ref var undergoUnitElementalComponent = ref _elementalPool.Value.Get(undergoEntity);

            ref var whoDoDamageLevelComponent = ref _levelPool.Value.Get(whoDoDamageEntity);
            ref var whoDoDamageElementalComponent = ref _elementalPool.Value.Get(whoDoDamageEntity);

            float levelingMultiply = CalculateLevelingMultiply(undergoUnitLevelComponent.Value, whoDoDamageLevelComponent.Value);
            float elementalDivider = Elemental.GetDamageDivider(whoDoDamageElementalComponent.CurrentType, undergoUnitElementalComponent.CurrentType);
            
            float damageToUnit = damageValue * levelingMultiply / elementalDivider;

            damageToUnit = MatchDamageComparedHealth(damageToUnit, undergoUnitHealthComponent.CurrentValue);

            undergoUnitHealthComponent.CurrentValue -= damageToUnit;
        }

        private float CalculateLevelingMultiply(int undergoUnitLevel, int whoDoDamageLevel)
        {
            float levelingOperand;
            if (undergoUnitLevel > whoDoDamageLevel)
            {
                levelingOperand = LEVELING_DECREASER;
            }
            else
            {
                levelingOperand = LEVELING_INCREASER;
            }

            return LEVELING_STANDART_VALUE + (levelingOperand * (whoDoDamageLevel - undergoUnitLevel));
        }
        #endregion UnitDamage

        private float MatchDamageComparedHealth(float damage, float health)
        {
            if (damage > health)
            {
                damage = health;
            }

            return damage;
        }

        private void DeleteEvent(int damagingEventEntity)
        {
            _damagingEventPool.Value.Del(damagingEventEntity);
        }
    }
}