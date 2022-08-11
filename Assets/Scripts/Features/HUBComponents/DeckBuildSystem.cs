using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine; 
using UnityEngine.UI;
namespace Client {
    sealed class DeckBuildSystem : IEcsRunSystem {
        readonly EcsSharedInject<GameState> _state = default;
        readonly EcsFilterInject<Inc<DeckBuildComponent>> _buildFilter = default;
        public void Run (IEcsSystems systems) {
            foreach (var entity in _buildFilter.Value)
            {
                var collection =  _state.Value.Collection;
                var deck = _state.Value.Deck;
            }
        }
    }
}