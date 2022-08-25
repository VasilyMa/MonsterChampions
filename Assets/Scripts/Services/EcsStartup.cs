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
        [SerializeField] MergeEffectsPool _mergeEffectsPool;
        public Collection collection;
        public Deck deck;
        private BattleState _battleState;
        private EcsWorld _world;
        private GameState _state;
        private EcsSystems _fixedTimeSystems, _globalInitSystem, _initSystems, _hubSystems, _fightSystems, _delhereEvents;
        
        void Start()
        {
            _world = new EcsWorld();
            _state = new GameState(_world, _monsterStorage, _mergeEffectsPool);
            _state.hubSystem = false;
            _state.runSysytem = true;

            _battleState = new BattleState(_world);

            _fixedTimeSystems = new EcsSystems(_world);
            _globalInitSystem = new EcsSystems(_world, _state);
            collection = _state.Collection;
            deck = _state.Deck;
            _initSystems = new EcsSystems (_world, _state);
            _hubSystems = new EcsSystems(_world, _state);
            _fightSystems = new EcsSystems(_world, _state);
            _delhereEvents = new EcsSystems(_world, _state);

            _fixedTimeSystems
                .Add(new UnitLookingSystem())
                ;

            _globalInitSystem
                .Add(new InitBaseDeck())
                .Add(new InitInterface())
                .Add(new InitInput())
                .Add(new InputSystem())
                .Add(new RewardSystem())
                .Add(new GetNewMonster())
                ;
            //_initSystems
                

                ;
            _hubSystems
               .Add(new DragAndDropCardSystem())
                ;

            _fightSystems
                .Add(new WinEventSystem())
                .Add(new LoseEventSystem())

                .Add(new InitBoard())
                .Add(new PlayableDeckSystem())
                .Add(new BuyUnitSystem())
                .Add(new InitBase())
                .Add(new InitUnits())

                .Add(new InitMergeEffectPool())

                .Add(new InitCamera())

                .Add(new DragAndDropUnitSystem())
                .Add(new MergeUnitSystem())
                .Add(new ActivateEnemyBaseEventSystem())

                .Add(new UnitMoveToTargetSystem())
                .Add(new StopUnitSystem())
                .Add(new OnOffRunAminationSystem())

                .Add(new TargetingSystem())
                .Add(new RetargetOnEnemyInDetectionZoneSystem())

                .Add(new OnOffAttackUnitSystem())
                .Add(new InOutFightUnitSystem())

                .Add(new CreateSlevDebuffAuraSystem())
                .Add(new WorkingSlevDebuffAuraSystem())

                .Add(new DamagingEventSystem())

                .Add(new RefreshHealthBarEventSystem())
                .Add(new HealthBarLookToCamera())

                .Add(new EnemySpawnerSystem())
                .Add(new DieEventSystem())
                .Add(new ResetIsTargetComponentAfterDeath())
                ;

            //_delhereEvents
                
                ;

#if UNITY_EDITOR
            _initSystems.Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem(/*events*/));
#endif

            InjectAllSystems(_fixedTimeSystems, _globalInitSystem, _initSystems, _hubSystems, _fightSystems, _delhereEvents);
            InitAllSystems(_fixedTimeSystems, _globalInitSystem, _initSystems, _hubSystems, _fightSystems, _delhereEvents);
        }

        void Update()
        {
            _globalInitSystem?.Run();

            _initSystems?.Run();

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
            OnDestroyAllSystems(_fixedTimeSystems, _globalInitSystem, _initSystems, _hubSystems, _fightSystems, _delhereEvents);

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