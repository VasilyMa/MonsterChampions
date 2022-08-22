using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class SpawnEnemyUnitsEventSystem : IEcsRunSystem
    {        
        readonly EcsWorldInject _world = default;

        readonly EcsFilterInject<Inc<SpawnEnemyUnitsEvent>> _spawnEnemyUnitsEventFilter = default;

        readonly EcsPoolInject<SpawnEnemyUnitsEvent> _spawnEnemyUnitsEventPool = default;

        public void Run (IEcsSystems systems)
        {
            foreach (var spawnEnemyUnitsEventEntity in _spawnEnemyUnitsEventFilter.Value)
            {

                // to do ay write enemy spawn

                DeleteEvent(spawnEnemyUnitsEventEntity);
            }
        }

        private void DeleteEvent(int spawnEnemyUnitsEventEntity)
        {
            _spawnEnemyUnitsEventPool.Value.Del(spawnEnemyUnitsEventEntity);
        }
    }
}