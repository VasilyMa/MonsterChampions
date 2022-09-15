using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class GoldAddingTimerSystem : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsPoolInject<GoldAddingComponent> _goldAddingPool = default;
        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;

        private static float _timerMaxValue = 1;
        private static float _timerCurrentValue = _timerMaxValue;
        private static int _goldReward = 2;

        public void Run (IEcsSystems systems)
        {
            _timerCurrentValue -= Time.deltaTime;

            if (_timerCurrentValue > 0)
            {
                return;
            }

            _timerCurrentValue = _timerMaxValue;

            int friendlyBaseEntity = _gameState.Value.GetPlayerBaseEntity();
            int enemyBaseEntity = _gameState.Value.GetEnemyBaseEntity();

            ref var friendlyGoldAddingComponent = ref _goldAddingPool.Value.Get(friendlyBaseEntity);
            ref var enemyGoldAddingComponent = ref _goldAddingPool.Value.Get(enemyBaseEntity);

            // to do ay effect for added gold
            _gameState.Value.AddPlayerGold(_goldReward + friendlyGoldAddingComponent.Modifier);
            _gameState.Value.AddEnemyGold(_goldReward + enemyGoldAddingComponent.Modifier);

            _interfacePool.Value.Get(_gameState.Value.InterfaceEntity).BuyCard.CheckButtons();
        }
    }
}