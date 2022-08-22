using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using Leopotam.EcsLite.ExtendedSystems;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Client
{
    sealed class EcsStartup : MonoBehaviour
    {
        [SerializeField] EcsUguiEmitter _uguiEmitter;
        [SerializeField] GetMonster _monsterStorage;
        public Collection collection;
        public Deck deck;
        private BattleState _battleState;
        private EcsWorld _world;
        private GameState _state;
        private EcsSystems _fixedTimeSystems, _globalInitSystem, _initSystems, _runSystems, _hubSystems, _fightSystems, _delhereEvents;
        
        void Start()
        {
            _world = new EcsWorld();
            _state = new GameState(_world, _monsterStorage);
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                _state.hubSystem = true;
                _state.runSysytem = false;
            }
            else
            {
                _state.hubSystem = false;
                _state.runSysytem = true;
            }

            _battleState = new BattleState(_world);

            _fixedTimeSystems = new EcsSystems(_world);
            _globalInitSystem = new EcsSystems(_world, _state);
            collection = _state.Collection;
            deck = _state.Deck;
            _initSystems = new EcsSystems (_world, _state);
            _runSystems = new EcsSystems(_world, _state);
            _hubSystems = new EcsSystems(_world, _state);
            _fightSystems = new EcsSystems(_world, _state);
            _delhereEvents = new EcsSystems(_world, _state);

            _fixedTimeSystems
                .Add(new UnitLookingSystem())
                ;

            _globalInitSystem
                .Add(new InitInterface())
                .Add(new InitInput())
                .Add(new InputSystem())
                ;
            //_initSystems
                

                ;
            _hubSystems
                .Add(new DragAndDropCardSystem())
                .Add(new GetNewMonster())
               
                ;
            _runSystems
                .Add(new UnitMoveToTargetSystem())
                .Add(new StopUnitSystem())
                .Add(new OnOffRunAminationSystem())// to do ay del here UnitMoveToTargetSystem and write it in _fightSystems
                ;

            _fightSystems
                .Add(new InitBoard())
                .Add(new InitPlayableDeck())
                .Add(new BuyUnitSystem())
                .Add(new InitBase())
                .Add(new InitUnits())

                .Add(new DragAndDropUnitSystem())
                .Add(new ActivateEnemyBaseEventSystem())

                .Add(new TargetingSystem())
                .Add(new RetargetOnEnemyInDetectionZoneSystem())

                .Add(new OnOffAttackUnitSystem())
                .Add(new InOutFightUnitSystem())

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

            if(_state.runSysytem) _runSystems?.Run();
            if(_state.hubSystem) _hubSystems?.Run();
            if(_state.runSysytem) _fightSystems?.Run();

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
        private void OnApplicationQuit()
        {
            _state.Save();
        }
    }
}