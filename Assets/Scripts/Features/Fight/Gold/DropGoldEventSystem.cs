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

        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;

        public void Run (IEcsSystems systems)
        {
            foreach (var eventEntity in _dropGoldEventFilter.Value)
            {
                ref var dropGoldEvent = ref _dropGoldEventPool.Value.Get(eventEntity);

                _gameState.Value.AddPlayerGold(dropGoldEvent.GoldValue);
                _interfacePool.Value.Get(_gameState.Value.InterfaceEntity).Resources.UpdateCoin();
                GameObject.Instantiate(_gameState.Value._mergeEffectsPool.MergeEffectPrefab[0], dropGoldEvent.DropPoint, Quaternion.identity);
                Debug.Log($"Player Gold = {_gameState.Value.GetPlayerGold()}");
                DeleteEvent(eventEntity);
            }
        }

        private void DeleteEvent(int eventEntity)
        {
            _dropGoldEventPool.Value.Del(eventEntity);
        }
    }
}