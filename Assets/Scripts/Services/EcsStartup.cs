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

        private EcsWorld _world;
        private BattleState _battleState;
        private EcsSystems _initSystems, _runSystems, _fightSystems, _delhereEvents;

        void Start()
        {
            _world = new EcsWorld();
            _battleState = new BattleState(_world);
            _initSystems = new EcsSystems (_world, _battleState);
            _runSystems = new EcsSystems(_world, _battleState);
            _fightSystems = new EcsSystems(_world, _battleState);
            _delhereEvents = new EcsSystems(_world, _battleState);

            _initSystems
                .Add(new InitEnemyBase())
                .Add(new InitUnits())
                .Add(new InitBoard())
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

            _initSystems.Inject();
            _runSystems.Inject();
            _fightSystems.Inject();

            _delhereEvents.Inject();

            _initSystems.Init();
            _runSystems.Init();
            _fightSystems.Init();

            _delhereEvents.Init();
        }

        void Update()
        {
            _initSystems?.Run();
            _runSystems?.Run();
            _fightSystems?.Run();

            _delhereEvents?.Run();
        }

        void OnDestroy()
        {
            OnDestroySystem(_initSystems);
            OnDestroySystem(_runSystems);
            OnDestroySystem(_fightSystems);
            OnDestroySystem(_delhereEvents);

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