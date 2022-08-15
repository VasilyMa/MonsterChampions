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
        EcsWorld _world;
        GameState _state;
        private EcsSystems _globalInitSystem, _initSystems, _runSystems, _delhereEvents, _hubSystems, _fightSystems;

        void Start()
        {
            _world = new EcsWorld ();
            _initSystems = new EcsSystems (_world);
            _runSystems = new EcsSystems(_world);
            _delhereEvents = new EcsSystems(_world);

            _globalInitSystem
                .Add(new InitInput())

                ;
            _initSystems
                .Add(new InitEnemyBase())
                .Add(new InitUnits())
                .Add(new InitBoard())
                .Add(new InitPlayableDeck())

                ;
            _hubSystems
                .Add(new BuyUnitSystem())
                ;
            _runSystems
                .Add(new ForcedStoppedEventSystem())
                .Add(new UnitMoveToTargetSystem()) // to do ay del here UnitMoveToTargetSystem and write it in _fightSystems
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
    }
}