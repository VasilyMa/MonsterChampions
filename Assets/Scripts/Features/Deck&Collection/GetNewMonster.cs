using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client {
    sealed class GetNewMonster : IEcsRunSystem {
        readonly EcsSharedInject<GameState> _state = default;
        readonly EcsFilterInject<Inc<NewMonster>> _monsterFilter = default;
        readonly EcsPoolInject<NewMonster> _newMonster = default;
        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;
        public void Run (IEcsSystems systems) {
            foreach (var entity in _monsterFilter.Value)
            {
                ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
                ref var monsterComp = ref _newMonster.Value.Get(entity);
                ref var monster = ref _state.Value._monsterStorage.monster[Random.Range(0,_state.Value._monsterStorage.monster.Length)];
                foreach (var item in _state.Value.Collection.CollectionUnits)
                {
                    if (monster.MonsterID != item.UnitID)
                    {
                        UnitData unitData = new UnitData();
                        unitData.UnitID = monster.MonsterID;
                        unitData.NameUnit = monster.NameUnit;
                        unitData.Damage = monster.Damage;
                        unitData.MoveSpeed = monster.MoveSpeed;
                        unitData.Health = monster.Health;
                        unitData.Prefabs = monster.Prefabs;
                        unitData.Elemental = monster.Elemental;
                        _state.Value.Collection.CollectionUnits.Add(unitData);
                        _state.Value.Save();
                        break;
                    }
                    else
                        break;
                }
                if (_state.Value.Collection.CollectionUnits.Count == 0)
                {
                    UnitData unitData = new UnitData();
                    unitData.UnitID = monster.MonsterID;
                    unitData.NameUnit = monster.NameUnit;
                    unitData.Damage = monster.Damage;
                    unitData.MoveSpeed = monster.MoveSpeed;
                    unitData.Health = monster.Health;
                    unitData.Prefabs = monster.Prefabs;
                    unitData.Elemental = monster.Elemental;
                    _state.Value.Collection.CollectionUnits.Add(unitData);
                    _state.Value.Save();
                }
                foreach (var card in _state.Value.Collection.CollectionUnits)
                {
                    var addedCard = (GameObject)GameObject.Instantiate(Resources.Load("CollectionCard"), interfaceComp.CollectionHolder);
                    var cardInfo = addedCard.GetComponent<CardInfo>();
                    cardInfo.unitID = card.UnitID;
                }
                _monsterFilter.Pools.Inc1.Del(entity);
            }
        }
    }
}