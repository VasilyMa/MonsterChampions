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
                var deck = _state.Value.Deck.DeckPlayer;
                var holder = interfaceComp.HolderCards;
                for (int i = 0; i < deck.Length; i++)
                {
                    if (deck[i].MonsterID == MonstersID.Value.Default)
                    {
                        holder.GetChild(i).gameObject.SetActive(false);
                    }
                    else
                    {
                        var newCard = (GameObject)GameObject.Instantiate(Resources.Load("PlayCard"), holder.GetChild(i).transform);
                        var newCardInfo = newCard.GetComponent<CardInfo>();
                        newCardInfo.UniqueID = _state.Value.Deck.DeckPlayer[i].UniqueID;
                        newCardInfo.Cost = _state.Value.Deck.DeckPlayer[i].Cost;
                        newCardInfo.Sprite = _state.Value.Deck.DeckPlayer[i].Sprite;
                        newCardInfo.MonsterID = _state.Value.Deck.DeckPlayer[i].MonsterID;
                        newCardInfo.Damage = _state.Value.Deck.DeckPlayer[i].Damage;
                        newCardInfo.Elemental = _state.Value.Deck.DeckPlayer[i].Elemental;
                        newCardInfo.Health = _state.Value.Deck.DeckPlayer[i].Health;
                        newCardInfo.Prefabs = _state.Value.Deck.DeckPlayer[i].Prefabs;
                        newCardInfo.MoveSpeed = _state.Value.Deck.DeckPlayer[i].MoveSpeed;
                        newCardInfo.VisualAndAnimations = _state.Value.Deck.DeckPlayer[i].VisualAndAnimations;
                        newCardInfo.UpdateCardInfo();
                    }
                }

                //for (int i = 0; i < deck.Length; i++)
                //{
                //    var newCard = (GameObject)GameObject.Instantiate(Resources.Load("PlayCard"), holder.GetChild(i).transform);
                //    var newCardInfo = newCard.GetComponent<CardInfo>();
                //    newCardInfo.Cost = _state.Value.Deck.DeckPlayer[i].Cost;
                //    newCardInfo.Sprite = _state.Value.Deck.DeckPlayer[i].Sprite;
                //    newCardInfo.MonsterID = _state.Value.Deck.DeckPlayer[i].MonsterID;
                //    newCardInfo.Damage = _state.Value.Deck.DeckPlayer[i].Damage;
                //    newCardInfo.Elemental = _state.Value.Deck.DeckPlayer[i].Elemental;
                //    newCardInfo.Health = _state.Value.Deck.DeckPlayer[i].Health;
                //    newCardInfo.Prefabs = _state.Value.Deck.DeckPlayer[i].Prefabs;
                //    newCardInfo.MoveSpeed = _state.Value.Deck.DeckPlayer[i].MoveSpeed;
                //    newCardInfo.VisualAndAnimations = _state.Value.Deck.DeckPlayer[i].VisualAndAnimations;
                //    newCardInfo.UpdateCardInfo();
                //}
                //for (int i = 0; i < holder.childCount; i++)
                //{
                //    if (holder.GetChild(i).transform.GetComponentInChildren<CardInfo>().MonsterID == MonstersID.Value.Default)
                //    {
                //        GameObject.Destroy(holder.GetChild(i).gameObject);
                //    }
                //}

                interfaceComp.BuyCard.CheckButtons();

                _playableDeckFilter.Pools.Inc1.Del(entity);
            }   
        }
    }
}