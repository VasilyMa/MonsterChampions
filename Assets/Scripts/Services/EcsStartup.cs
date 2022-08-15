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

        EcsWorld _world;
        GameState _state;
        private EcsSystems _globalInitSystem, _initSystems, _runSystems, _delhereEvents, _hubSystems;

        void Start()
        {
            _world = new EcsWorld ();
            _state = new GameState(_world, _monsterStorage);
            _initSystems = new EcsSystems (_world, _state);
            _runSystems = new EcsSystems(_world, _state);
            _hubSystems = new EcsSystems(_world, _state);
            _globalInitSystem = new EcsSystems(_world, _state);
            _delhereEvents = new EcsSystems(_world, _state);

            _globalInitSystem
                .Add(new InitInput())

                ;
            _initSystems
                .Add(new InitUnits())
                .Add(new InitBoard())
                .Add(new InitPlayableDeck())

                ;

            _runSystems
                .Add(new SetPointToMoveEventSystem())
                .Add(new TargetingSystem())
                .Add(new InputSystem())
                .Add(new DragAndDropUnitSystem())
                ;

            _hubSystems
                .Add(new GetNewMonster())
                
                ;

            _delhereEvents
                .DelHere<SetPointToMoveEvent>()
                ;

#if UNITY_EDITOR
            _initSystems.Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem(/*events*/));
#endif
            _globalInitSystem.Inject();
            _initSystems.Inject();
            _runSystems.Inject();
            _hubSystems.Inject();
            _delhereEvents.Inject();

            _globalInitSystem.Init();
            _initSystems.Init();
            _runSystems.Init();
            _hubSystems.Init();
            _delhereEvents.Init();
        }

        void Update()
        {
            _globalInitSystem?.Run();

            if(_state.runSysytem) _initSystems?.Run();
            if(_state.runSysytem) _runSystems?.Run();
            if(_state.hubSystem) _hubSystems?.Run();

            _delhereEvents?.Run();
        }

        void OnDestroy()
        {
            OnDestroySystem(_initSystems);
            OnDestroySystem(_runSystems);
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
    }
}