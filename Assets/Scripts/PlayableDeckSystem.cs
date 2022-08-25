using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client 
{
    sealed class PlayableDeckSystem : IEcsRunSystem 
    {
        readonly EcsSharedInject<GameState> _state = default;
        readonly EcsFilterInject<Inc<PlayableDeckEvent>> _playableDeckFilter = default;
        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;
        public void Run (IEcsSystems systems) 
        {
            foreach (var entity in _playableDeckFilter.Value)
            {
                ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
                var holder = interfaceComp.HolderCards;
                var cards = interfaceComp.cards;
                cards = new System.Collections.Generic.List<GameObject>();
                for (int i = 0; i < _state.Value.Deck.DeckPlayer.Length; i++)
                {
                    if (_state.Value.Deck.DeckPlayer[i].MonsterID == 0)
                    {
                        GameObject.Destroy(holder.GetChild(i).gameObject);
                    }
                }
                for (int card = 0; card < _state.Value.Deck.DeckPlayer.Length; card++)
                {
                    if (_state.Value.Deck.DeckPlayer[card].MonsterID == 0)
                        continue;
                    _state.Value.PlayableDeck.PlayerDeck.Add(_state.Value.Deck.DeckPlayer[card]);
                    for (int i = 0; i < holder.childCount; i++)
                    {
                        if (holder.GetChild(i).transform.childCount >= 1)
                            continue;
                        else
                        {
                            var newCard = (GameObject)GameObject.Instantiate(Resources.Load("PlayCard"), holder.GetChild(i).transform);
                            var newCardInfo = newCard.GetComponent<CardInfo>();
                            newCardInfo.Cost = _state.Value.Deck.DeckPlayer[card].Cost;
                            newCardInfo.Sprite = _state.Value.Deck.DeckPlayer[card].Sprite;
                            newCardInfo.MonsterID = _state.Value.Deck.DeckPlayer[card].MonsterID;
                            newCardInfo.Damage = _state.Value.Deck.DeckPlayer[card].Damage;
                            newCardInfo.Elemental = _state.Value.Deck.DeckPlayer[card].Elemental;
                            newCardInfo.Health = _state.Value.Deck.DeckPlayer[card].Health;
                            newCardInfo.Prefabs = _state.Value.Deck.DeckPlayer[card].Prefabs;
                            newCardInfo.MoveSpeed = _state.Value.Deck.DeckPlayer[card].MoveSpeed;
                            newCardInfo.UpdateCardInfo();
                            cards.Add(newCard);
                            break;
                        }
                    }
                }
                _playableDeckFilter.Pools.Inc1.Del(entity);
            }   
        }
    }
}