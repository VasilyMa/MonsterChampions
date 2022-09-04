using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class SpawnLogicEnemyPlayerSystem : IEcsRunSystem
    {
        readonly EcsWorldInject _world;

        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsFilterInject<Inc<MonsterSpawner>, Exc<DeadTag>> _monsterSpawnerFilter = default;

        readonly EcsPoolInject<MonsterSpawner> _monsterSpawnerPool = default;

        readonly EcsPoolInject<MonsterSpawnEvent> _monsterSpawnEventPool = default;

        private int _monsterSpawnerEntity = BattleState.NULL_ENTITY;
        private int _actualSquadToSpawn;
        private int _neededGoldForSpawn;

        private float _timeToSpawnMaxValue = 1;
        private float _timeToSpawnCurrentValue = 1;

        public void Run (IEcsSystems systems)
        {
            if (_timeToSpawnCurrentValue > 0)
            {
                _timeToSpawnCurrentValue -= Time.deltaTime;

                return;
            }

            _timeToSpawnCurrentValue = _timeToSpawnMaxValue;

            foreach (var monsterSpawnerEntity in _monsterSpawnerFilter.Value)
            {
                _monsterSpawnerEntity = monsterSpawnerEntity;

                ref var monsterSpawner = ref _monsterSpawnerPool.Value.Get(_monsterSpawnerEntity);
                _actualSquadToSpawn = monsterSpawner.ActualSquad;

                _neededGoldForSpawn = 0;

                foreach (var monster in monsterSpawner.MonsterSpawnerInfo.MonstersSquads[_actualSquadToSpawn].Monsters)
                {
                    _neededGoldForSpawn += monster.Cost;
                }

                if (_gameState.Value.GetEnemyGold() < _neededGoldForSpawn)
                {
                    continue;
                }

                _gameState.Value.RevomeEnemyGold(_neededGoldForSpawn);

                var monstersLevel = monsterSpawner.MonsterSpawnerInfo.MonstersSquads[_actualSquadToSpawn].MonstersLevel;

                float monsterCount = monsterSpawner.MonsterSpawnerInfo.MonstersSquads[_actualSquadToSpawn].Monsters.Count;

                float maxRangeX = monsterCount - 1;

                float maxLeftPositionX = 0 - (maxRangeX);

                int monsterIndex = 0;

                foreach (var monster in monsterSpawner.MonsterSpawnerInfo.MonstersSquads[_actualSquadToSpawn].Monsters)
                {
                    var monsterSpawnEventEntity = _world.Value.NewEntity();

                    ref var monsterSpawnEventComponent = ref _monsterSpawnEventPool.Value.Add(monsterSpawnEventEntity);

                    monsterSpawnEventComponent.SpawnPoint = new Vector3(maxLeftPositionX + (monsterIndex * 2),
                                                                        monsterSpawner.MonsterSpawnerInfo.SpawnPoint.position.y,
                                                                        monsterSpawner.MonsterSpawnerInfo.SpawnPoint.position.z);


                    monsterSpawnEventComponent.Direction = monsterSpawner.MonsterSpawnerInfo.SpawnPoint.rotation;
                    monsterSpawnEventComponent.Cost = monster.Cost;
                    monsterSpawnEventComponent.Damage = monster.Damage;
                    monsterSpawnEventComponent.Elemental = monster.Elemental;
                    monsterSpawnEventComponent.Health = monster.Health;
                    monsterSpawnEventComponent.MoveSpeed = monster.MoveSpeed;
                    monsterSpawnEventComponent.MonsterID = monster.MonsterID;
                    monsterSpawnEventComponent.Level = monstersLevel;
                    monsterSpawnEventComponent.VisualAndAnimations = monster.VisualAndAnimations;
                    monsterSpawnEventComponent.isFriendly = false;

                    monsterIndex++;
                }

                monsterSpawner.ActualSquad++;

                if (monsterSpawner.ActualSquad > monsterSpawner.MonsterSpawnerInfo.MonstersSquads.Count - 1)
                {
                    monsterSpawner.ActualSquad = 0;
                }
            }

            _monsterSpawnerEntity = BattleState.NULL_ENTITY;
        }
    }
}