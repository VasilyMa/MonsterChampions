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
                var monster = _state.Value._monsterStorage.monster[Random.Range(0, _state.Value._monsterStorage.monster.Length)];
                var list = collection.FindAll(x => x.UnitID == monster.MonsterID);
                if (list.Count == 0)
                {
                    UnitData UnitData = new UnitData();
                    UnitData.UnitID = monster.MonsterID;
                    UnitData.NameUnit = monster.NameUnit;
                    UnitData.Damage = monster.Damage;
                    UnitData.MoveSpeed = monster.MoveSpeed;
                    UnitData.Health = monster.Health;
                    UnitData.Prefabs = monster.Prefabs;
                    UnitData.Elemental = monster.Elemental;
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
                }
            }
        }
    }
}