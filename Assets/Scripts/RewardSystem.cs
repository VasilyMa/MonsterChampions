using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

namespace Client {
    sealed class RewardSystem : IEcsRunSystem {
        readonly EcsWorldInject _world = default;
        readonly EcsSharedInject<GameState> _state = default;
        readonly EcsFilterInject<Inc<RewardComponentEvent>> _rewardFilter = default;
        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;
        readonly EcsPoolInject<NewMonster> _newMonsterPool = default;
        private int newID;
        private int countTries;
        private bool isFullCollection;
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _rewardFilter.Value)
            {
                ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
                interfaceComp.RewardPanelHolder.gameObject.SetActive(true);
                interfaceComp.RewardHolder.gameObject.SetActive(true);
                if (Tutorial.CurrentStage == Tutorial.Stage.OpenCollection)
                {
                    ref var unlockedMonster = ref _state.Value._monsterStorage.monster[2];

                    var newCard = interfaceComp.RewardHolder.transform.GetChild(1).transform;
                    var infoNewCard = newCard.GetComponent<CardInfo>();
                    infoNewCard.LevelCard = 0;
                    infoNewCard.Cost = unlockedMonster.Cost;
                    infoNewCard.Sprite = unlockedMonster.Sprite;
                    infoNewCard.MonsterID = unlockedMonster.MonsterID;
                    infoNewCard.Damage = unlockedMonster.Damage;
                    infoNewCard.Health = unlockedMonster.Health;
                    infoNewCard.MoveSpeed = unlockedMonster.MoveSpeed;
                    infoNewCard.Prefabs = unlockedMonster.Prefabs;
                    infoNewCard.Elemental = unlockedMonster.Elemental;
                    infoNewCard.VisualAndAnimations = unlockedMonster.VisualAndAnimations;

                    newCard.gameObject.SetActive(false);
                    ref var monsterComp = ref _newMonsterPool.Value.Add(_world.Value.NewEntity());
                    monsterComp.cardInfo = infoNewCard;
                    _rewardFilter.Pools.Inc1.Del(entity);
                    break;
                }
                if (_state.Value.Settings.Level % 3 == 0 && _state.Value.Settings.Level <= _state.Value._monsterStorage.monster.Length * 3 - 9)
                {
                    ref var unlockedMonster = ref _state.Value._monsterStorage.monster[_state.Value.Settings.MaxLevelRewardedCard];

                    var newCard = interfaceComp.RewardHolder.transform.GetChild(1).transform;
                    var infoNewCard = newCard.GetComponent<CardInfo>();
                    infoNewCard.LevelCard = 0;
                    infoNewCard.Cost = unlockedMonster.Cost;
                    infoNewCard.Sprite = unlockedMonster.Sprite;
                    infoNewCard.MonsterID = unlockedMonster.MonsterID;
                    infoNewCard.Damage = unlockedMonster.Damage;
                    infoNewCard.Health = unlockedMonster.Health;
                    infoNewCard.MoveSpeed = unlockedMonster.MoveSpeed;
                    infoNewCard.Prefabs = unlockedMonster.Prefabs;
                    infoNewCard.Elemental = unlockedMonster.Elemental;
                    infoNewCard.VisualAndAnimations = unlockedMonster.VisualAndAnimations;

                    newCard.gameObject.SetActive(false);
                    ref var monsterComp = ref _newMonsterPool.Value.Add(_world.Value.NewEntity());
                    monsterComp.cardInfo = infoNewCard;
                }
                else
                {
                    var deck = _state.Value.Deck.DeckPlayer;
                    var collection = _state.Value.Collection.CollectionUnits;
                    var tempAllCards = new List<UnitData>();
                    foreach (var card in collection)
                    {
                        if (card.MonsterID != MonstersID.Value.Default)
                        {
                            tempAllCards.Add(card);
                        }
                    }
                    for (int i = 0; i < deck.Length; i++)
                    {
                        if (deck[i].MonsterID != MonstersID.Value.Default)
                        {
                            tempAllCards.Add(deck[i]);
                        }
                    }
                    //ref var mosnterInfo = ref _state.Value._monsterStorage.monster[Random.Range(1, _state.Value.Settings.MaxLevelRewardedCard)];

                    var tempCard = FindCardAndUpgrade(tempAllCards);

                    foreach (var card in collection)
                    {
                        if (card.MonsterID == tempCard.MonsterID)
                        {
                            card.LevelCard = tempCard.LevelCard;
                            card.Cost = tempCard.Cost;
                            card.Sprite = tempCard.Sprite;
                            card.MonsterID = tempCard.MonsterID;
                            card.Damage = tempCard.Damage;
                            card.Health = tempCard.Health;
                            card.MoveSpeed = tempCard.MoveSpeed;
                            card.Prefabs = tempCard.Prefabs;
                            card.Elemental = tempCard.Elemental;
                            card.VisualAndAnimations = tempCard.VisualAndAnimations; 
                            _state.Value.SaveCollection();
                            break;
                        }
                    }
                    for (int i = 0; i < deck.Length; i++)
                    {
                        if (deck[i].MonsterID == tempCard.MonsterID)
                        {
                            deck[i].LevelCard = tempCard.LevelCard;
                            deck[i].Cost = tempCard.Cost;
                            deck[i].Sprite = tempCard.Sprite;
                            deck[i].MonsterID = tempCard.MonsterID;
                            deck[i].Damage = tempCard.Damage;
                            deck[i].Health = tempCard.Health;
                            deck[i].MoveSpeed = tempCard.MoveSpeed;
                            deck[i].Prefabs = tempCard.Prefabs;
                            deck[i].Elemental = tempCard.Elemental;
                            deck[i].VisualAndAnimations = tempCard.VisualAndAnimations;
                            _state.Value.SaveDeck();
                            break;
                        }
                    }
                    var newCard = interfaceComp.RewardHolder.transform.GetChild(1).transform;
                    var infoNewCard = newCard.GetComponent<CardInfo>();
                    //infoNewCard.UniqueID = newID;
                    infoNewCard.LevelCard = tempCard.LevelCard;
                    infoNewCard.Cost = tempCard.Cost;
                    infoNewCard.Sprite = tempCard.Sprite;
                    infoNewCard.MonsterID = tempCard.MonsterID;
                    infoNewCard.Damage = tempCard.Damage;
                    infoNewCard.Health = tempCard.Health;
                    infoNewCard.MoveSpeed = tempCard.MoveSpeed;
                    infoNewCard.Prefabs = tempCard.Prefabs;
                    infoNewCard.Elemental = tempCard.Elemental;
                    infoNewCard.VisualAndAnimations = tempCard.VisualAndAnimations;
                    newCard.gameObject.SetActive(false);
                }
                _rewardFilter.Pools.Inc1.Del(entity);
            }
        }
        UnitData FindCardAndUpgrade(List<UnitData> list)
        {
            var deck = _state.Value.Deck.DeckPlayer;
            var collection = _state.Value.Collection.CollectionUnits;
            foreach (var item in list)
            {
                var idCard = Random.Range(0, list.Count);
                list[idCard].LevelCard++;
                if (list[idCard].LevelCard % 3 == 0)
                {
                    foreach (var card in collection)
                    {
                        if (card.MonsterID == list[idCard].MonsterID)
                        {
                            card.LevelCard = list[idCard].LevelCard;
                            break;
                        }
                    }
                    for (int i = 0; i < deck.Length; i++)
                    {
                        if (deck[i].MonsterID == list[idCard].MonsterID)
                        {
                            deck[i].LevelCard = list[idCard].LevelCard;
                            break;
                        }
                    }
                    _state.Value.SaveCollection();
                    _state.Value.SaveDeck();
                    list.Remove(list[idCard]);
                    break;
                }
                else if (list[idCard].LevelCard % 3 == 1)
                {
                    list[idCard].Health++;
                    return list[idCard];
                }
                else if (list[idCard].LevelCard % 3 == 2)
                {
                    list[idCard].Damage++;
                    return list[idCard];
                }
            }
            if (list.Count > 0)
            {
                return FindCardAndUpgrade(list);
            }
            return null;  
        }
        private void newUniqueID()
        {
            var newId = Random.Range(1, 101);
            var isCollection = FindEmptyInCollection(newId);
            var isDeck = FindEmptyInDeck(newId);
            if (isCollection | isDeck)
            {
                countTries++;
                if (countTries < 100)
                    newUniqueID();
                else 
                {
                    isFullCollection = true;
                    Debug.LogWarning("Your collection is full, please take it away");
                }
            }
            else
            {
                isFullCollection = false;
                newID = newId;
            }
        }
        private bool FindEmptyInCollection(int value)
        {
            var collection = _state.Value.Collection.CollectionUnits;
            var list = collection.FindAll(x => x.UniqueID == value);
            if (list.Count == 0)
                return false;
            else
                return true;
        }
        private bool FindEmptyInDeck(int value)
        {
            var deck = _state.Value.Deck.DeckPlayer;
            foreach (var card in deck)
            {
                if (card.UniqueID == value)
                {
                    return true;
                }
                else
                    return false;
            }
            return false;
        }
    }
}