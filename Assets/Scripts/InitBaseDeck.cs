using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
namespace Client 
{
    sealed class InitBaseDeck : IEcsInitSystem 
    {
        readonly EcsSharedInject<GameState> _state = default;

        private int _startMonster = (int)MonstersID.Value.Stoon;

        public void Init (IEcsSystems systems) 
        {
            if (!_state.Value.Settings.BaseDeck)
            {
                var deck = _state.Value.Deck.DeckPlayer;
                var storage = _state.Value._monsterStorage.monster;
                deck[0].MonsterID = storage[_startMonster].MonsterID;
                deck[0].Sprite = storage[_startMonster].Sprite;
                deck[0].Cost = storage[_startMonster].Cost;
                deck[0].Damage = storage[_startMonster].Damage;
                deck[0].Health = storage[_startMonster].Health;
                deck[0].MoveSpeed = storage[_startMonster].MoveSpeed;
                deck[0].Elemental = storage[_startMonster].Elemental;
                deck[0].VisualAndAnimations = storage[_startMonster].VisualAndAnimations;

                _state.Value.Settings.MaxLevelRewardedCard = 3;
                _state.Value.Settings.BaseDeck = true;
                _state.Value.Save();
            } 
        }
    }
}