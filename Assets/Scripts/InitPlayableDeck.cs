using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
namespace Client {
    sealed class InitPlayableDeck : IEcsInitSystem {
        readonly EcsSharedInject<GameState> _state = default;
        public void Init (IEcsSystems systems) {
            PlayableDeck playableDeck = new PlayableDeck();
            for (int card = 0; card < _state.Value.Deck.DeckPlayer.Length; card++)
            {
                playableDeck.PlayerDeck.Add(_state.Value.Deck.DeckPlayer[card]);
            }
        }
    }
}