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
        private EcsSystems _initSystems, _runSystems, _delhereEvents;

        void Start()
        {
            _world = new EcsWorld ();
            _initSystems = new EcsSystems (_world);
            _runSystems = new EcsSystems(_world);
            _delhereEvents = new EcsSystems(_world);

            _initSystems
                .Add(new InitUnits())
                .Add(new InitBoard())
                ;

            _runSystems
                .Add(new MoveToPoint());

            _delhereEvents
                .DelHere<TestEvent>()
                ;

#if UNITY_EDITOR
            _initSystems.Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem(/*events*/));
#endif

            _initSystems.Inject().Init();
            _runSystems.Inject().Init();
            _delhereEvents.Inject().Init();
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