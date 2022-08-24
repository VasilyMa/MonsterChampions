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

        public void Run (IEcsSystems systems)
        {
            foreach (var eventEntity in _dropGoldEventFilter.Value)
            {
                ref var dropGoldEvent = ref _dropGoldEventPool.Value.Get(eventEntity);

                _gameState.Value.AddPlayerGold(dropGoldEvent.GoldValue);

                GameObject.Instantiate(_gameState.Value._mergeEffectsPool.MergeEffectPrefab[0], dropGoldEvent.DropPoint, Quaternion.identity);

                DeleteEvent(eventEntity);
            }
        }

        private void DeleteEvent(int eventEntity)
        {
            _dropGoldEventPool.Value.Del(eventEntity);
        }
    }
}