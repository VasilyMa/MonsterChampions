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
                for (int i = 0; i < deck.Length - 1; i++)
                {
                    deck[i].UnitID = storage[i].MonsterID;
                    deck[i].NameUnit = storage[i].NameUnit;
                    deck[i].Sprite = storage[i].Sprite;
                    deck[i].Cost = storage[i].Cost;
                    deck[i].Damage = storage[i].Damage;
                    deck[i].Health = storage[i].Health;
                    deck[i].MoveSpeed = storage[i].MoveSpeed;
                    deck[i].Elemental = storage[i].Elemental;
                    deck[i].Prefabs = storage[i].Prefabs;
                }
                _state.Value.Settings.MaxLevelRewardedCard = 3;
                _state.Value.Settings.BaseDeck = true;
                _state.Value.Save();
            } 
        }
    }
}