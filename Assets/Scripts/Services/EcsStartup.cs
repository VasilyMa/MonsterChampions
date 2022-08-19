using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using Leopotam.EcsLite.ExtendedSystems;
using UnityEngine;

namespace Client
{
    sealed class EcsStartup : MonoBehaviour
    {
        [SerializeField] EcsUguiEmitter _uguiEmitter;
        [SerializeField] GetMonster _monsterStorage;

        private BattleState _battleState;
        private EcsWorld _world;
        private GameState _state;
        private EcsSystems _fixedTimeSystems, _globalInitSystem, _initSystems, _runSystems, _hubSystems, _fightSystems, _delhereEvents;

        void Start()
        {
            _world = new EcsWorld();
            _state = new GameState(_world, _monsterStorage);
            _battleState = new BattleState(_world);

            _fixedTimeSystems = new EcsSystems(_world);
            _globalInitSystem = new EcsSystems(_world, _state);
            _initSystems = new EcsSystems (_world, _state);
            _runSystems = new EcsSystems(_world, _state);
            _hubSystems = new EcsSystems(_world, _state);
            _fightSystems = new EcsSystems(_world, _battleState);
            _delhereEvents = new EcsSystems(_world, _state);

            _fixedTimeSystems
                .Add(new UnitLookingSystem())
                ;

            _globalInitSystem
                .Add(new InitInput())

                ;
            _initSystems
                .Add(new InitBoard())
                .Add(new InitPlayableDeck())

                ;
            _runSystems
                .Add(new ForcedStoppedEventSystem())
                .Add(new UnitMoveToTargetSystem())
                ;
            _hubSystems
                .Add(new BuyUnitSystem())
                ;

            _fightSystems
                .Add(new InitBase())
                .Add(new InitUnits())

                .Add(new ActivateEnemyBaseEventSystem())

                .Add(new TargetingSystem())
                .Add(new RetargetOnEnemyInDetectionZoneSystem())

                .Add(new InOutFightUnitSystem())
                .Add(new OnOffAttackUnitSystem())

                .Add(new DamagingEventSystem())

                .Add(new SpawnEnemyUnitsEventSystem())
                .Add(new DieEventSystem())
                .Add(new ResetIsTargetComponentAfterDeath())
                ;

            //_delhereEvents
                
                ;

#if UNITY_EDITOR
            _initSystems.Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem(/*events*/));
#endif

            InjectAllSystems(_fixedTimeSystems, _globalInitSystem, _initSystems, _runSystems, _hubSystems, _fightSystems, _delhereEvents);
            InitAllSystems(_fixedTimeSystems, _globalInitSystem, _initSystems, _runSystems, _hubSystems, _fightSystems, _delhereEvents);
        }

        void Update()
        {
            _globalInitSystem?.Run();
            _initSystems?.Run();
            _runSystems?.Run();
            _hubSystems?.Run();
            _fightSystems?.Run();

            _delhereEvents?.Run();
        }

        private void FixedUpdate()
        {
            _fixedTimeSystems?.Run();
        }

        void OnDestroy()
        {
            OnDestroyAllSystems(_fixedTimeSystems, _globalInitSystem, _initSystems, _runSystems, _hubSystems, _fightSystems, _delhereEvents);

            if (_world != null)
            {
                _world.Destroy();
                _world = null;
            }
        }

        private void OnDestroyAllSystems(params EcsSystems[] systems)
        {
            for (int i = 0; i < systems.Length; i++)
            {
                if (systems[i] != null)
                {
                    systems[i].Destroy();
                    systems[i] = null;
                }
            }
        }

        private void InjectAllSystems(params EcsSystems[] systems)
        {
            foreach (var system in systems)
            {
                system.Inject();
            }
        }

        private void InitAllSystems(params EcsSystems[] systems)
        {
            foreach (var system in systems)
            {
                system.Init();
            }
        }
    }
}