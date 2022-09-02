using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class SpawnLogicEnemyPlayerSystem : IEcsRunSystem
    {
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


            }

            _monsterSpawnerEntity = BattleState.NULL_ENTITY;
        }
    }
}