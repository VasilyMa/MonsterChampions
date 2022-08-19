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
        public Collection collection;
        public Deck deck;
        private BattleState _battleState;
        private EcsWorld _world;
        private GameState _state;
        private EcsSystems _globalInitSystem, _initSystems, _runSystems, _delhereEvents, _hubSystems, _fightSystems;

        void Start()
        {
            _world = new EcsWorld();
            _state = new GameState(_world, _monsterStorage);
            _battleState = new BattleState(_world);
            collection = _state.Collection;
            deck = _state.Deck;
            _initSystems = new EcsSystems (_world, _state);
            _runSystems = new EcsSystems(_world, _state);
            _globalInitSystem = new EcsSystems(_world, _state);
            _hubSystems = new EcsSystems(_world, _state);
            _fightSystems = new EcsSystems(_world, _state);
            _delhereEvents = new EcsSystems(_world, _state);

            _globalInitSystem
                .Add(new InitInterface())
                .Add(new InitInput())
                ;
            _initSystems
                .Add(new InitEnemyBase())
                .Add(new InitUnits())
                .Add(new InitBoard())
                //.Add(new InitPlayableDeck())

                ;
            /*_hubSystems
                
                
                ;*/
            _runSystems
                .Add(new InputSystem())
                .Add(new ForcedStoppedEventSystem())
                .Add(new DragAndDropUnitSystem())
                .Add(new MergeUnitSystem())
                .Add(new BuyUnitSystem())
                .Add(new GetNewMonster())
                .Add(new DragAndDropCardSystem());
            //.Add(new UnitMoveToTargetSystem()) // to do ay del here UnitMoveToTargetSystem and write it in _fightSystems
            ;

            _fightSystems
                .Add(new TargetingSystem())
                .Add(new DieEventSystem())
                .Add(new DamagingEventSystem())
                ;

            //_delhereEvents
                
                ;

#if UNITY_EDITOR
            _initSystems.Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem(/*events*/));
#endif
            _globalInitSystem.Inject();
            _initSystems.Inject();
            _runSystems.Inject();
            _delhereEvents.Inject();

            _globalInitSystem.Init();
            _initSystems.Init();
            _runSystems.Init();
            _delhereEvents.Init();
        }

        void Update()
        {
            _initSystems?.Run();
            _runSystems?.Run();

            _delhereEvents?.Run();
        }

        void OnDestroy()
        {
            OnDestroySystem(_initSystems);
            OnDestroySystem(_runSystems);
            OnDestroySystem(_fightSystems);
            OnDestroySystem(_delhereEvents);
            OnDestroySystem(_hubSystems);
            OnDestroySystem(_globalInitSystem);
            if (_world != null)
            {
                _world.Destroy ();
                _world = null;
            }
        }

        private void OnDestroySystem(EcsSystems system)
        {
            if (system != null)
            {
                system.Destroy();
                system = null;
            }
        }
        private void OnApplicationQuit()
        {
            _state.Save();
        }
    }
}