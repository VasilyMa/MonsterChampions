using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class DamagingEventSystem : IEcsRunSystem
    {
        readonly EcsWorldInject _world = default;

        readonly EcsFilterInject<Inc<DamagingEvent>> _damagingEventFilter = default;

        readonly EcsPoolInject<DamagingEvent> _damagingEventPool = default;
        readonly EcsPoolInject<DieEvent> _dieEventPool = default;

        readonly EcsPoolInject<HealthComponent> _healthPool = default;
        readonly EcsPoolInject<LevelComponent> _levelPool = default;
        readonly EcsPoolInject<ElementalComponent> _elementalPool = default;
        readonly EcsPoolInject<BaseTag> _baseTagPool = default;
        readonly EcsPoolInject<UnitTag> _unitTagPool = default;
        readonly EcsPoolInject<Animable> _animablePool = default;

        private const float LEVELING_STANDART_VALUE = 1f;
        private const float LEVELING_INCREASER = 0.5f;
        private const float LEVELING_DECREASER = 0.25f;

        private int _undergoEntity = BattleState.NULL_ENTITY;
        private int _damagingEntity = BattleState.NULL_ENTITY;
        private float _damageValue = 0;

        public void Run (IEcsSystems systems)
        {
            foreach (var damagingEventEntity in _damagingEventFilter.Value)
            {
                ref var damagingEvent = ref _damagingEventPool.Value.Get(damagingEventEntity);

                _undergoEntity = damagingEvent.UndergoEntity;
                _damagingEntity = damagingEvent.DamagingEntity;
                if (damagingEvent.DamageValue != 0)
                    _damageValue = damagingEvent.DamageValue;

                ref var healthComponent = ref _healthPool.Value.Get(_undergoEntity);


                if (_unitTagPool.Value.Has(_undergoEntity))
                {
                    DoDamageToUnit(_undergoEntity, _damagingEntity, damagingEvent.DamageValue);
                }
                else if (_baseTagPool.Value.Has(damagingEvent.UndergoEntity))
                {
                    DoDamageToBase(_undergoEntity, _damagingEntity);
                }
                else
                {
                    Debug.LogError($"Error with «{_undergoEntity}» entity. It's don't have «BaseTag» or «UnitTag»");
                    DeleteEvent(damagingEventEntity);
                    continue;
                }

                InvokeDieEnent();

                DeleteEvent(damagingEventEntity);
            }
        }

#region BaseDamage
        private void DoDamageToBase(int baseEntity, int unitEntity)
        {
            ref var baseHealthComponent = ref _healthPool.Value.Get(baseEntity);
            ref var animableComponent = ref _animablePool.Value.Get(baseEntity);

            ref var unitLevelComponent = ref _levelPool.Value.Get(unitEntity);

            float damageToBase = CalculateDamageToBase(unitLevelComponent.Value);

            damageToBase = MatchDamageComparedHealth(damageToBase, baseHealthComponent.CurrentValue);

            baseHealthComponent.CurrentValue -= damageToBase;

            animableComponent.Animator.SetTrigger(nameof(animableComponent.isDamaged));

            _dieEventPool.Value.Add(_world.Value.NewEntity()).Invoke(_damagingEntity);
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

        private void InvokeDieEnent()
        {
            if (_healthPool.Value.Get(_undergoEntity).CurrentValue > 0)
            {
                return;
            }

            if (_dieEventPool.Value.Has(_undergoEntity))
            {
                return;
            }

            _dieEventPool.Value.Add(_world.Value.NewEntity()).Invoke(_undergoEntity);
        }

        private void DeleteEvent(int damagingEventEntity)
        {
            _damagingEventPool.Value.Del(damagingEventEntity);

            _undergoEntity = BattleState.NULL_ENTITY;
            _damagingEntity = BattleState.NULL_ENTITY;
        }
    }
}