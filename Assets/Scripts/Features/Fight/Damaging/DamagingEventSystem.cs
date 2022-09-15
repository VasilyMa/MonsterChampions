using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class DamagingEventSystem : IEcsRunSystem
    {
        readonly EcsWorldInject _world = default;
        readonly EcsSharedInject<GameState> _state = default;
        readonly EcsFilterInject<Inc<DamagingEvent>> _damagingEventFilter = default;

        readonly EcsPoolInject<DamagingEvent> _damagingEventPool = default;
        readonly EcsPoolInject<DieEvent> _dieEventPool = default;
        readonly EcsPoolInject<CreateSparkyExplosionEvent> _sparkyExplosionEventPool = default;
        readonly EcsPoolInject<CreateTinkiThunderboltEvent> _tinkiThunderboltEventPool = default;

        readonly EcsPoolInject<HealthComponent> _healthPool = default;
        readonly EcsPoolInject<BableProtectionComponent> _bableProtectionPool = default; 
        readonly EcsPoolInject<LevelComponent> _levelPool = default;
        readonly EcsPoolInject<ElementalComponent> _elementalPool = default;
        readonly EcsPoolInject<BaseTag> _baseTagPool = default;
        readonly EcsPoolInject<UnitTag> _unitTagPool = default;
        readonly EcsPoolInject<Animable> _animablePool = default;
        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;
        readonly EcsPoolInject<DeadTag> _deadPool = default;

        readonly EcsPoolInject<SparkyComponent> _sparkyPool = default;
        readonly EcsPoolInject<ExplosionComponent> _explosionPool = default;

        readonly EcsPoolInject<TinkiComponent> _tinkiPool = default;

        readonly EcsPoolInject<ViewComponent> _viewPool = default;

        private const float LEVELING_STANDART_VALUE = 1f;
        private const float LEVELING_INCREASER = 0.5f;
        private const float LEVELING_DECREASER = 0.25f;

        private ElementalType _bableProtectionElement = ElementalType.Water;

        private int _undergoEntity = BattleState.NULL_ENTITY;
        private int _damagingEntity = BattleState.NULL_ENTITY;
        private float _damageValue = 0;

        public void Run(IEcsSystems systems)
        {
            foreach (var damagingEventEntity in _damagingEventFilter.Value)
            {
                ref var damagingEvent = ref _damagingEventPool.Value.Get(damagingEventEntity);

                _undergoEntity = damagingEvent.UndergoEntity;
                _damagingEntity = damagingEvent.DamagingEntity;

                if (_undergoEntity == BattleState.NULL_ENTITY)
                {
                    DeleteEvent(damagingEventEntity);
                    continue;
                }

                if (_damagingEntity == BattleState.NULL_ENTITY)
                {
                    DeleteEvent(damagingEventEntity);
                    continue;
                }

                if (_deadPool.Value.Has(_undergoEntity))
                {
                    DeleteEvent(damagingEventEntity);
                    continue;
                }

                if (damagingEvent.DamageValue != 0)
                    _damageValue = damagingEvent.DamageValue;

                ref var healthComponent = ref _healthPool.Value.Get(_undergoEntity);


                if (_unitTagPool.Value.Has(_undergoEntity))
                {
                    UnitDamage();
                }
                else if (_baseTagPool.Value.Has(_undergoEntity))
                {
                    BaseDamage(_undergoEntity, _damagingEntity);
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
        private void BaseDamage(int baseEntity, int unitEntity)
        {
            ref var baseHealthComponent = ref _healthPool.Value.Get(baseEntity);
            ref var animableComponent = ref _animablePool.Value.Get(baseEntity);

            ref var unitLevelComponent = ref _levelPool.Value.Get(unitEntity);

            float damageToBase = CalculateDamageToBase(unitLevelComponent.Value);

            damageToBase = MatchDamageComparedHealth(damageToBase, baseHealthComponent.CurrentValue);

            baseHealthComponent.CurrentValue -= damageToBase;

            RefreshProgressBar();

            animableComponent.Animator.SetTrigger(nameof(animableComponent.isDamaged));

            _dieEventPool.Value.Add(_world.Value.NewEntity()).Invoke(_damagingEntity, withoutGold: true);
        }
        private void RefreshProgressBar()
        {
            ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
            ref var ourHealthComp = ref _healthPool.Value.Get(_state.Value.GetPlayerBaseEntity());
            ref var enemyHealthComp = ref _healthPool.Value.Get(_state.Value.GetEnemyBaseEntity());
            interfaceComp.Progress.UpdateHealth(ourHealthComp.CurrentValue, enemyHealthComp.CurrentValue);
        }

        private float CalculateDamageToBase(int unitLevel)
        {
            return Mathf.Pow(2, unitLevel - 1);
        }
        #endregion BaseDamage

        #region UnitDamage
        private void UnitDamage()
        {
            if (damagingEntityIsSparky())
            {
                InvokeSparkyExplosion();
            }

            if (damagingEntityIsTinki())
            {
                InvokeTinkiThunderbolt();
            }

            ref var damagingEntityElementalComponent = ref _elementalPool.Value.Get(_damagingEntity);

            float elementalDivider;

            if (BableProtectionIsZero())
            {
                ref var undergoEntityElementalComponent = ref _elementalPool.Value.Get(_undergoEntity);
                ref var undergoEntityHealthComponent = ref _healthPool.Value.Get(_undergoEntity);

                elementalDivider = Elemental.GetDamageDivider(damagingEntityElementalComponent.CurrentType, undergoEntityElementalComponent.CurrentType);

                DealDamage(ref undergoEntityHealthComponent.CurrentValue, elementalDivider);

                UpdateHealthbar(undergoEntityHealthComponent.CurrentValue);
            }
            else
            {
                ref var bableProtectionComponent = ref _bableProtectionPool.Value.Get(_undergoEntity);

                elementalDivider = Elemental.GetDamageDivider(damagingEntityElementalComponent.CurrentType, _bableProtectionElement);

                DealDamage(ref bableProtectionComponent.ProtectionValue, elementalDivider);

                UpdateShield(bableProtectionComponent.ProtectionValue);
            }

        }
        private void UpdateShield(float amount)
        {
            ref var viewComp = ref _viewPool.Value.Get(_undergoEntity);
            viewComp.HealthBarMB.ShieldUpdate(amount);
        }
        private void DealDamage(ref float health, float elementalDivider)
        {

            ref var damagingEntityLevelComponent = ref _levelPool.Value.Get(_damagingEntity);
            ref var undergoEntityLevelComponent = ref _levelPool.Value.Get(_undergoEntity);

            float levelingMultiply = CalculateLevelingMultiply(undergoEntityLevelComponent.Value, damagingEntityLevelComponent.Value);

            float damageToUnit = Mathf.Round(_damageValue * levelingMultiply / elementalDivider);

            health -= MatchDamageComparedHealth(damageToUnit, health);
        }
        private void UpdateHealthbar(float health)
        {
            ref var viewComp = ref _viewPool.Value.Get(_undergoEntity);
            viewComp.HealthBarMB.UpdateHealth(health);
        }

        private bool damagingEntityIsSparky()
        {
            return _sparkyPool.Value.Has(_damagingEntity);
        }

        private void InvokeSparkyExplosion()
        {
            _sparkyExplosionEventPool.Value.Add(_world.Value.NewEntity()).Invoke(sparkyEntity: _damagingEntity);
        }

        private bool damagingEntityIsTinki()
        {
            return _tinkiPool.Value.Has(_damagingEntity);
        }

        private void InvokeTinkiThunderbolt()
        {
            _tinkiThunderboltEventPool.Value.Add(_world.Value.NewEntity()).Invoke(_damagingEntity, _undergoEntity);
        }

        private bool BableProtectionIsZero()
        {
            if (!_bableProtectionPool.Value.Has(_undergoEntity))
            {
                return true;
            }

            ref var bableProtectionComponent = ref _bableProtectionPool.Value.Get(_undergoEntity);

            return bableProtectionComponent.ProtectionValue <= 0;
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

            bool typicalDeath = true;

            if (_explosionPool.Value.Has(_damagingEntity))
            {
                ref var explosionComponent = ref _explosionPool.Value.Get(_damagingEntity);

                if (_undergoEntity == explosionComponent.OwnerEntity)
                {
                    _dieEventPool.Value.Add(_world.Value.NewEntity()).Invoke(_undergoEntity, withoutGold: true);
                    typicalDeath = false;
                }
            }

            if (typicalDeath)
            {
                _dieEventPool.Value.Add(_world.Value.NewEntity()).Invoke(_undergoEntity);
            }
        }

        private void DeleteEvent(int damagingEventEntity)
        {
            _damagingEventPool.Value.Del(damagingEventEntity);

            _undergoEntity = BattleState.NULL_ENTITY;
            _damagingEntity = BattleState.NULL_ENTITY;
            _damageValue = 0;
        }
    }
}