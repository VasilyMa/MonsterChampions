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
        [SerializeField] EffectsPool _effectsPool;
        [SerializeField] InterfaceConfigs _interfaceConfigs;
        public Collection collection;
        public Deck deck;
        private BattleState _battleState;
        private EcsWorld _world;
        private GameState _state;
        private EcsSystems _fixedTimeSystems, _globalInitSystem, _initSystems, _hubSystems, _preparedSystems, _fightSystems, _delhereEvents, _tutorialSystems;
        
        void Start()
        {
            _world = new EcsWorld();
            _state = new GameState(_world, _monsterStorage, _effectsPool, _interfaceConfigs);
            _state.HubSystems = false;
            _state.FightSystems = false;
            collection = _state.Collection;
            deck = _state.Deck;

            _battleState = new BattleState(_world);

            _fixedTimeSystems = new EcsSystems(_world);
            _globalInitSystem = new EcsSystems(_world, _state);

            _initSystems = new EcsSystems (_world, _state);
            _hubSystems = new EcsSystems(_world, _state);

            _preparedSystems = new EcsSystems(_world, _state);
            _fightSystems = new EcsSystems(_world, _state);

            _delhereEvents = new EcsSystems(_world, _state);

            _tutorialSystems = new EcsSystems(_world, _state);

            _tutorialSystems
                .Add(new TwoBuysMonsters())
                .Add(new MergeMonsters())
                .Add(new DragAndDropMonster()) // to do ay if player off game before overed first level
                .Add(new OpenCollection())
                .Add(new DragAndDropNewCardInDeck())
                ;

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
               .Add(new DragWaitSystem())
                ;

            _preparedSystems
                .Add(new InitCamera())

                .Add(new InitBoard())
                .Add(new InitBase())
                .Add(new InitUnits())

                .Add(new InitMergeEffectPool()) // to do ay

                .Add(new DragAndDropUnitSystem())
                .Add(new MergeUnitSystem())
                .Add(new ActivateEnemyBaseEventSystem())

                .Add(new MonsterSpawnEventSystem())

                .Add(new PlayableDeckSystem())

                .Add(new CameraMoveToBoardSystem())
                .Add(new BoardMoveToScreenSystem())
                .Add(new DoBaseSoSmall())
                ;

            _fightSystems
                .Add(new WinEventSystem())
                .Add(new LoseEventSystem())

                .Add(new UnitMoveToTargetSystem())
                .Add(new StopUnitSystem())
                .Add(new OnOffRunAminationSystem())

                .Add(new TargetingSystem())
                .Add(new RetargetOnEnemyInDetectionZoneSystem())

                .Add(new OnOffAttackUnitSystem())
                .Add(new InOutFightUnitSystem())

                .Add(new CreateSlevDebuffAuraSystem())
                .Add(new WorkingSlevDebuffAuraSystem())

                .Add(new CreateBableProtectionAuraSystem())
                .Add(new CheckAndDeleteBableProtectSystem())

                .Add(new DamagingEventSystem())

                .Add(new CreateSparkyExplosionEventSystem())

                .Add(new MoveTinkiThunderboltEffectSystem())
                .Add(new CreateTinkiThunderboltEventSystem())

                .Add(new SpawnLogicEnemyPlayerSystem())
                //.Add(new BuyUnitSystem())

                .Add(new DieEventSystem())
                .Add(new ResetIsTargetComponentAfterDeath())

                .Add(new DropGoldEventSystem())
                .Add(new GoldAddingTimerSystem())
                ;

            //_delhereEvents
                
                ;

#if UNITY_EDITOR
            _initSystems.Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem(/*events*/));
#endif

            InjectAllSystems(_fixedTimeSystems, _globalInitSystem, _initSystems, _hubSystems, _preparedSystems, _fightSystems, _delhereEvents, _tutorialSystems);
            InitAllSystems(_fixedTimeSystems, _globalInitSystem, _initSystems, _hubSystems, _preparedSystems, _fightSystems, _delhereEvents, _tutorialSystems);
        }

        void Update()
        {
            _globalInitSystem?.Run();

            _initSystems?.Run();

            if (!Tutorial.isOver())
            {
                _tutorialSystems?.Run();
            }

            if (_state.HubSystems) _hubSystems?.Run();
            if (_state.PreparedSystems) _preparedSystems?.Run();
            if (_state.FightSystems) _fightSystems?.Run();

            _delhereEvents?.Run();
        }

        private void FixedUpdate()
        {
            _fixedTimeSystems?.Run();
        }

        void OnDestroy()
        {
            OnDestroyAllSystems(_fixedTimeSystems, _globalInitSystem, _initSystems, _hubSystems, _preparedSystems, _fightSystems, _delhereEvents, _tutorialSystems);

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