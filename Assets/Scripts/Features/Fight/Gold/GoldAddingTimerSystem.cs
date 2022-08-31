using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class GoldAddingTimerSystem : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _gameState;

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

            // to do ay effect for added gold
            _gameState.Value.AddPlayerGold(_goldReward);
        }
    }
}