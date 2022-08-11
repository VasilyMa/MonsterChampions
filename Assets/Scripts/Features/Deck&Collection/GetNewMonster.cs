using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client {
    sealed class GetNewMonster : IEcsRunSystem {
        readonly EcsSharedInject<GameState> _state = default;
        readonly EcsFilterInject<Inc<NewMonster>> _monsterFilter = default;
        readonly EcsPoolInject<NewMonster> _newMonster = default;
        public void Run (IEcsSystems systems) {
            foreach (var entity in _monsterFilter.Value)
            {
                ref  var monsterComp = ref _newMonster.Value.Get(entity);
                ref var monster = ref _state.Value._monsterStorage.monster[Random.Range(0,2)];
                UnitData unitData = new UnitData();
                unitData.MonsterID = monster.MonsterID;
                unitData.NameUnit = monster.NameUnit;
                unitData.Damage = monster.Damage;
                unitData.MoveSpeed = monster.MoveSpeed;
                unitData.Health = monster.Health;
                unitData.Prefabs = monster.Prefabs;
                unitData.Elemental = monster.Elemental;
                _state.Value.Collection.CollectionUnits.Add(unitData);
                _state.Value.Save();
                _monsterFilter.Pools.Inc1.Del(entity);
            }
        }
    }
}