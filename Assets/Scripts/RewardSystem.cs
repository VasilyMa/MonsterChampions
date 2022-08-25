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
        public void Run (IEcsSystems systems)
        {
            foreach (var entity in _rewardFilter.Value)
            {
                ref var mosnterInfo = ref _state.Value._monsterStorage.monster[Random.Range(0, _state.Value.Settings.MaxLevelRewardedCard)];
                ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
                interfaceComp.RewardPanelHolder.gameObject.SetActive(true);
                interfaceComp.RewardHolder.gameObject.SetActive(true);
                var newCard = interfaceComp.RewardHolder.transform.GetChild(1).transform;
                var infoNewCard = newCard.GetComponent<CardInfo>();
                infoNewCard.Cost = (int)Random.Range(mosnterInfo.Cost - mosnterInfo.Cost * 0.25f, mosnterInfo.Cost + mosnterInfo.Cost * 0.25f);
                infoNewCard.Sprite = mosnterInfo.Sprite;
                infoNewCard.NameUnit = mosnterInfo.NameUnit;
                infoNewCard.unitID = mosnterInfo.MonsterID;
                infoNewCard.Damage = Random.Range(mosnterInfo.Damage - mosnterInfo.Damage * 0.25f, mosnterInfo.Damage + mosnterInfo.Damage * 0.25f);
                infoNewCard.Health = Random.Range(mosnterInfo.Health - mosnterInfo.Health * 0.25f, mosnterInfo.Health + mosnterInfo.Health * 0.25f);
                infoNewCard.MoveSpeed = Random.Range(mosnterInfo.MoveSpeed - mosnterInfo.MoveSpeed * 0.25f, mosnterInfo.MoveSpeed + mosnterInfo.MoveSpeed * 0.25f);
                infoNewCard.Prefabs = mosnterInfo.Prefabs;
                infoNewCard.Elemental = mosnterInfo.Elemental;
                infoNewCard.VisualAndAnimations = mosnterInfo.VisualAndAnimations;

                newCard.gameObject.SetActive(false);
                ref var monsterComp = ref _newMonsterPool.Value.Add(_world.Value.NewEntity());
                monsterComp.cardInfo = infoNewCard;
                _rewardFilter.Pools.Inc1.Del(entity);
            }
        }
    }
}