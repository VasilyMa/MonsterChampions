using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CreateSlevDebuffAuraSystem : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _gameState = default;

        readonly EcsFilterInject<Inc<SlevComponent, ViewComponent>, Exc<DeadTag, OnBoardUnitTag>> _slevFilter = default;

        readonly EcsPoolInject<SlevComponent> _slevPool = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        readonly EcsPoolInject<FractionComponent> _fractionPool = default;
        readonly EcsPoolInject<UnitTag> _unitPool = default;
        readonly EcsPoolInject<OnBoardUnitTag> _onBoardUnitPool = default;
        readonly EcsPoolInject<SlevAuraDebuff> _slevAuraDebuffPool = default;

        private float _timeToCreateAuraMaxValue = 1f;
        private float _timeToCreateAuraCurrentValue = 1f;
        private float _auraEffectRadius = 7f;
        private float _auraEffectMaxDuration = 5f;

        private int _aliveUnitLayer = LayerMask.GetMask(nameof(ViewComponent.AliveUnit));

        public void Run (IEcsSystems systems) // to do ay rewrite this system with methods. And check how OverlapSphere working with layers
        {
            if (_timeToCreateAuraCurrentValue > 0)
            {
                _timeToCreateAuraCurrentValue -= Time.deltaTime;
                return;
            }
            else
            {
                _timeToCreateAuraCurrentValue = _timeToCreateAuraMaxValue;
            }

            foreach (var slevEntity in _slevFilter.Value)
            {
                ref var viewComponent = ref _viewPool.Value.Get(slevEntity);
                ref var fractionComponent = ref _fractionPool.Value.Get(slevEntity);

                var _allUnitsInAura = Physics.OverlapSphere(viewComponent.Transform.position, _auraEffectRadius, _aliveUnitLayer);

                Debug.Log($"Всего найдено: {_allUnitsInAura.Length}");

                int collidersCount = 0;
                int enemyCount = 0;

                foreach (var unitInAura in _allUnitsInAura)
                {
                    collidersCount++;

                    var unitEcsInfoMB = unitInAura.GetComponent<EcsInfoMB>();
                    var unitInAuraEntity = unitEcsInfoMB.GetEntity();
                    Debug.Log("Заход");
                    if (!_unitPool.Value.Has(unitInAuraEntity))
                    {
                        continue;
                    }

                    if (_onBoardUnitPool.Value.Has(unitInAuraEntity))
                    {
                        continue;
                    }

                    ref var unitFractionComponent = ref _fractionPool.Value.Get(unitInAuraEntity);

                    if (unitFractionComponent.isFriendly == fractionComponent.isFriendly)
                    {
                        continue;
                    }

                    if (!_slevAuraDebuffPool.Value.Has(unitInAuraEntity))
                        _slevAuraDebuffPool.Value.Add(unitInAuraEntity);

                    ref var slevAuraDebuff = ref _slevAuraDebuffPool.Value.Get(unitInAuraEntity);

                    if (!slevAuraDebuff.isWork)
                    {
                        slevAuraDebuff.TimerToClearMaxValue = _auraEffectMaxDuration;

                        ref var debuffedMonsterViewComponent = ref _viewPool.Value.Get(unitInAuraEntity);

                        GameObject.Instantiate(_gameState.Value.EffectsPool.MonstersEffects.SlevDebuff, debuffedMonsterViewComponent.GameObject.transform);
                    }

                    slevAuraDebuff.TimerToClearCurrentValue = slevAuraDebuff.TimerToClearMaxValue;

                    enemyCount++;
                }

                Debug.Log($"Найдено коллайдеров: {collidersCount}. Найдено врагов: {enemyCount}");
            }
        }
    }
}