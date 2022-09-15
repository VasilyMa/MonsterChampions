using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class DropGoldEventSystem : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsFilterInject<Inc<DropGoldEvent>> _dropGoldEventFilter = default;

        readonly EcsPoolInject<DropGoldEvent> _dropGoldEventPool = default;

        readonly EcsPoolInject<DroppingGoldComponent> _droppingGoldPool = default;
        readonly EcsPoolInject<FractionComponent> _fractionPool = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;

        private int _dropGoldEntity = BattleState.NULL_ENTITY;

        public void Run (IEcsSystems systems)
        {
            foreach (var eventEntity in _dropGoldEventFilter.Value)
            {
                ref var dropGoldEvent = ref _dropGoldEventPool.Value.Get(eventEntity);

                _dropGoldEntity = dropGoldEvent.DropGoldEntity;

                ref var droppingGoldComponent = ref _droppingGoldPool.Value.Get(_dropGoldEntity);
                ref var fractionComponent = ref _fractionPool.Value.Get(_dropGoldEntity);

                if (_viewPool.Value.Has(_dropGoldEntity))
                {
                    ref var viewComponent = ref _viewPool.Value.Get(_dropGoldEntity);

                    GameObject.Instantiate(_gameState.Value.EffectsPool.OtherEffects.DroppingGold, viewComponent.Transform.position, Quaternion.identity);
                }

                if (fractionComponent.isFriendly)
                {
                    _gameState.Value.AddEnemyGold(droppingGoldComponent.GoldValue);
                }
                else
                {
                    _gameState.Value.AddPlayerGold(droppingGoldComponent.GoldValue);
                    _interfacePool.Value.Get(_gameState.Value.InterfaceEntity).Resources.UpdatePlayerCoinAmount();
                }
                _interfacePool.Value.Get(_gameState.Value.InterfaceEntity).BuyCard.CheckButtons();

                DeleteEvent(eventEntity);
            }
        }

        private void DeleteEvent(int eventEntity)
        {
            _dropGoldEventPool.Value.Del(eventEntity);

            _dropGoldEntity = BattleState.NULL_ENTITY;
        }
    }
}