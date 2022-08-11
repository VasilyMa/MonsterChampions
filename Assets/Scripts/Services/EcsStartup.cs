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
        private EcsSystems _initSystems, _runSystems, _delhereEvents, _hubSystems;

        void Start()
        {
            _world = new EcsWorld ();
            _state = new GameState(_world, _monsterStorage);
            _initSystems = new EcsSystems (_world, _state);
            _runSystems = new EcsSystems(_world, _state);
            _hubSystems = new EcsSystems(_world, _state);
            _delhereEvents = new EcsSystems(_world, _state);
            

            _initSystems
                .Add(new InitUnits())
                .Add(new InitBoard())
                .Add(new InitMonsterStorage())
                ;

            _runSystems
                .Add(new SetPointToMoveEventSystem())
                .Add(new TargetingSystem())
                .Add(new InputPlayerSystem())
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

            _initSystems.Inject();
            _runSystems.Inject();
            _hubSystems.Inject();
            _delhereEvents.Inject();

            _initSystems.Init();
            _runSystems.Init();
            _hubSystems.Init();
            _delhereEvents.Init();
        }

        void Update()
        {
            _initSystems?.Run();
            _runSystems?.Run();
            _hubSystems?.Run();

            _delhereEvents?.Run();
        }

        void OnDestroy()
        {
            OnDestroySystem(_initSystems);
            OnDestroySystem(_runSystems);
            OnDestroySystem(_delhereEvents);
            OnDestroySystem(_hubSystems);
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