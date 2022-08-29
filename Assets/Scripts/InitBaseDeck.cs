using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
namespace Client 
{
    sealed class InitBaseDeck : IEcsInitSystem 
    {
        readonly EcsSharedInject<GameState> _state = default;
        public void Init (IEcsSystems systems) 
        {
            if (!_state.Value.Settings.BaseDeck)
            {
                var deck = _state.Value.Deck.DeckPlayer;
                var storage = _state.Value._monsterStorage.monster;
                for (int i = 0; i < deck.Length - 2; i++)
                {
                    deck[i].UniqueID = 1;
                    deck[i].MonsterID = storage[i].MonsterID;
                    deck[i].Sprite = storage[i].Sprite;
                    deck[i].Cost = storage[i].Cost;
                    deck[i].Damage = storage[i].Damage;
                    deck[i].Health = storage[i].Health;
                    deck[i].MoveSpeed = storage[i].MoveSpeed;
                    deck[i].Elemental = storage[i].Elemental;
                    deck[i].VisualAndAnimations = storage[i].VisualAndAnimations;
                }
                _state.Value.Settings.MaxLevelRewardedCard = 3;
                _state.Value.Settings.BaseDeck = true;
                _state.Value.Save();
            } 
        }
    }
}