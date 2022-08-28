using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client {
    sealed class GetNewMonster : IEcsRunSystem {
        readonly EcsWorldInject _world = default;
        readonly EcsSharedInject<GameState> _state = default;
        readonly EcsFilterInject<Inc<NewMonster>> _monsterFilter = default;
        readonly EcsPoolInject<NewMonster> _newMonster = default;
        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;
        public void Run (IEcsSystems systems) {
            foreach (var entity in _monsterFilter.Value)
            {
                ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
                ref var monsterComp = ref _newMonster.Value.Get(entity);
                var collection = _state.Value.Collection.CollectionUnits;
                //ref var firstMonster = ref _state.Value._monsterStorage.monster[Random.Range(0, _state.Value._monsterStorage.monster.Length)];
                ref var monster = ref monsterComp.cardInfo;
                UnitData newUnitData = new UnitData();
                newUnitData.UniqueID = monster.UniqueID;
                newUnitData.Sprite = monster.Sprite;
                newUnitData.Cost = monster.Cost;
                newUnitData.MonsterID = monster.MonsterID;
                newUnitData.Damage = monster.Damage;
                newUnitData.MoveSpeed = monster.MoveSpeed;
                newUnitData.Health = monster.Health;
                newUnitData.Prefabs = monster.Prefabs;
                newUnitData.Elemental = monster.Elemental;
                newUnitData.VisualAndAnimations = monster.VisualAndAnimations;
                _state.Value.Collection.CollectionUnits.Add(newUnitData);
                _state.Value.Save();
                _monsterFilter.Pools.Inc1.Del(entity);
                /*
                if (_state.Value.Collection.CollectionUnits.Count == 0)
                {
                    ref var firstMonster = ref _state.Value._monsterStorage.monster[Random.Range(0, _state.Value._monsterStorage.monster.Length)];
                    UnitData newUnitData = new UnitData();
                    newUnitData.UnitID = firstMonster.MonsterID;
                    newUnitData.NameUnit = firstMonster.NameUnit;
                    newUnitData.Damage = firstMonster.Damage;
                    newUnitData.MoveSpeed = firstMonster.MoveSpeed;
                    newUnitData.Health = firstMonster.Health;
                    newUnitData.Prefabs = firstMonster.Prefabs;
                    newUnitData.Elemental = firstMonster.Elemental;
                    _state.Value.Collection.CollectionUnits.Add(newUnitData);
                    _state.Value.Save();
                    _monsterFilter.Pools.Inc1.Del(entity);
                    break;
                }
                var newMonster = _state.Value._monsterStorage.monster[Random.Range(0, _state.Value._monsterStorage.monster.Length)];
                var list = collection.FindAll(x => x.UnitID == newMonster.MonsterID);
                if (list.Count == 0)
                {
                    UnitData UnitData = new UnitData();
                    UnitData.UnitID = newMonster.MonsterID;
                    UnitData.NameUnit = newMonster.NameUnit;
                    UnitData.Damage = newMonster.Damage;
                    UnitData.MoveSpeed = newMonster.MoveSpeed;
                    UnitData.Health = newMonster.Health;
                    UnitData.Prefabs = newMonster.Prefabs;
                    UnitData.Elemental = newMonster.Elemental;
                    _state.Value.Collection.CollectionUnits.Add(UnitData);
                    _state.Value.Save();
                    _monsterFilter.Pools.Inc1.Del(entity);
                }
                else
                {
                    if (collection.Count == _state.Value._monsterStorage.monster.Length)
                    {
                        Debug.LogWarning("Your collection is full");
                        _monsterFilter.Pools.Inc1.Del(entity);
                    }
                    else
                        break;
                }*/
            }
        }
    }
}