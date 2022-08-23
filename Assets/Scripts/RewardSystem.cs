using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client {
    sealed class RewardSystem : IEcsRunSystem {
        readonly EcsSharedInject<GameState> _state = default;
        readonly EcsFilterInject<Inc<RewardComponentEvent>> _rewardFilter = default;
        readonly EcsPoolInject<NewMonster> _newMonster = default;

        public void Run (IEcsSystems systems) {
            foreach (var entity in _rewardFilter.Value)
            {

            }
        }
    }
}