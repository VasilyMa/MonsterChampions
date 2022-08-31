using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using DG.Tweening;
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
                ref var mosnterInfo = ref _state.Value._monsterStorage.monster[Random.Range(1, _state.Value.Settings.MaxLevelRewardedCard)];
                ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
                interfaceComp.RewardPanelHolder.gameObject.SetActive(true);
                interfaceComp.RewardHolder.gameObject.SetActive(true);
                newUniqueID();
                if (isFullCollection)
                {
                    _rewardFilter.Pools.Inc1.Del(entity);
                    return;
                }
                var newCard = interfaceComp.RewardHolder.transform.GetChild(1).transform;
                var infoNewCard = newCard.GetComponent<CardInfo>();
                infoNewCard.UniqueID = newID;
                infoNewCard.Cost = mosnterInfo.Cost;
                infoNewCard.Sprite = mosnterInfo.Sprite;
                infoNewCard.MonsterID = mosnterInfo.MonsterID;
                infoNewCard.Damage = Mathf.Round(Random.Range(mosnterInfo.Damage - mosnterInfo.Damage * 0.5f, mosnterInfo.Damage + mosnterInfo.Damage * 0.5f));
                infoNewCard.Health = Mathf.Round(Random.Range(mosnterInfo.Health - mosnterInfo.Health * 0.5f, mosnterInfo.Health + mosnterInfo.Health * 0.5f));
                infoNewCard.MoveSpeed = mosnterInfo.MoveSpeed;
                infoNewCard.Prefabs = mosnterInfo.Prefabs;
                infoNewCard.Elemental = mosnterInfo.Elemental;
                infoNewCard.VisualAndAnimations = mosnterInfo.VisualAndAnimations;

                newCard.gameObject.SetActive(false);
                ref var monsterComp = ref _newMonsterPool.Value.Add(_world.Value.NewEntity());
                monsterComp.cardInfo = infoNewCard;
                _rewardFilter.Pools.Inc1.Del(entity);
            }
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