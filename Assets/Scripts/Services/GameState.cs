using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Client
{
    public class GameState
    {
        private string savePathCollection = $"{Application.persistentDataPath}/Collection.Save";
        private string savePathDeck = $"{Application.persistentDataPath}/Deck.Save";
        private string savePathSettings = $"{Application.persistentDataPath}/Settings.Save";
        private EcsWorld _ecsWorld;
        public GetMonster _monsterStorage;
        public EffectsPool EffectsPool;
        public InterfaceConfigs InterfaceConfigs;
        public Collection Collection = new Collection();
        public Deck Deck = new Deck();
        public GameSettings Settings = new GameSettings();
        public int StorageEntity;
        public int InputEntity;
        public int BoardEntity;
        public int CameraEntity;
        public int InterfaceEntity;
        public int CurrentLevel = 1;
        public bool isDrag;
        public bool inCollection;

        public bool PreparedSystems { set; get; } = false;
        public bool FightSystems { set; get; } = false;
        public bool HubSystems { set; get; } = true;

        private int _playerBaseEntity = -1;
        private List<int> _enemyBaseEntity = new List<int>();
        private int _currentActiveEnemyBaseInArray = -1;

        private int PlayerGold;
        private int EnemyGold;

        public PlayableDeck PlayableDeck = new PlayableDeck();
        public const int NULL_ENTITY = -1;


        #region /// Gold Manipulations ///
        public void AddEnemyGold(int value)
        {
            if (value <= 0)
            {
                Debug.LogError("Trying to added minus value for EnemyGold!");
                return;
            }

            EnemyGold += value;

            UpdateEnemyCoinInterface();
        }

        public void RevomeEnemyGold(int value)
        {
            if (value <= 0)
            {
                Debug.LogError("Trying to remove minus value for EnemyGold!");
                return;
            }

            EnemyGold -= value;

            UpdateEnemyCoinInterface();
        }

        public int GetEnemyGold()
        {
            return EnemyGold;
        }

        private void UpdateEnemyCoinInterface()
        {
            EcsPool<InterfaceComponent> interfacePool = _ecsWorld.GetPool<InterfaceComponent>();

            ref var interfaceComp = ref interfacePool.Get(InterfaceEntity);

            interfaceComp.Resources.UpdateEnemyCoinAmount();
        }

        public void AddPlayerGold(int value)
        {
            if (value <= 0)
            {
                Debug.LogError("Trying to added minus value for PlayerGold!");
                return;
            }

            PlayerGold += value;

            UpdatePlayerCoinInterface();
        }

        public void RevomePlayerGold(int value)
        {
            if (value <= 0)
            {
                Debug.LogError("Trying to remove minus value for PlayerGold!");
                return;
            }

            PlayerGold -= value;

            UpdatePlayerCoinInterface();
        }

        public int GetPlayerGold()
        {
            return PlayerGold;
        }

        private void UpdatePlayerCoinInterface()
        {
            EcsPool<InterfaceComponent> interfacePool = _ecsWorld.GetPool<InterfaceComponent>();

            ref var interfaceComp = ref interfacePool.Get(InterfaceEntity);

            interfaceComp.Resources.UpdatePlayerCoinAmount();
        }

        #endregion

        public void SetPlayerBaseEntity(int baseEntity)
        {
            if (baseEntity > NULL_ENTITY)
            {
                _playerBaseEntity = baseEntity;
            }
            else
            {
                _playerBaseEntity = NULL_ENTITY;
                Debug.LogError("Write incorrect number for PlayerBaseEntity. Value was match to -1");
            }
        }

        public int GetPlayerBaseEntity()
        {
            if (_playerBaseEntity == NULL_ENTITY) Debug.LogError("Get nullable value (-1) for PlayerBaseEntity.");
            return _playerBaseEntity;
        }

        public void AddEnemyBaseEntity(int entity)
        {
            _enemyBaseEntity.Add(entity);
            Debug.Log($"Добавили базу в BattleState: {entity}");
        }

        public int GetEnemyBaseEntity()
        {
            if (_enemyBaseEntity.Count > 0)
            {
                return _enemyBaseEntity[0];
            }
            else
            {
                return NULL_ENTITY;
            }
        }

        public void ActivateNextEnemyBase()
        {
            _currentActiveEnemyBaseInArray++;
            EcsWorld.GetPool<ActivateEnemyBaseEvent>().Add(_enemyBaseEntity[_currentActiveEnemyBaseInArray]);
        }

        public static bool isNullableEntity(int entity)
        {
            return entity == NULL_ENTITY;
        }

        public EcsWorld EcsWorld
        {
            get
            {
                return _ecsWorld;
            }
            set
            {
                _ecsWorld = value;
            }
        }

        public GameState(EcsWorld EcsWorld, GetMonster monsterStorage, EffectsPool effectsPool, InterfaceConfigs interfaceConfigs)
        {
            _ecsWorld = EcsWorld;
            _monsterStorage = monsterStorage;
            EffectsPool = effectsPool;
            InterfaceConfigs = interfaceConfigs;
            if (File.Exists(savePathSettings))
            {
                LoadGame();
            }
            else
            {
                Settings.BaseDeck = false;
                Settings.TutorialStage = 0;
                Settings.SceneNumber = 0;
                Settings.GameVersion = Application.version;
                Settings.Level = 1;
                Settings.MaxLevelRewardedCard = 2;
                SaveGameSetting();
                if (File.Exists(savePathCollection)) SaveCollection();
                else SaveCollection();
                if(File.Exists(savePathDeck)) SaveDeck();
                else SaveDeck();
            }
        }
        public void Save()
        {
            SaveCollection();
            SaveDeck();
        }
        #region Save/Load
        public void SaveGameSetting()
        {
            File.Delete(savePathSettings);
            string saveDataSettings = JsonUtility.ToJson(Settings, true);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream file = File.Create(string.Concat(savePathSettings));
            binaryFormatter.Serialize(file, saveDataSettings);
            file.Close();
        }
        public void SaveDeck()
        {
            File.Delete(savePathDeck);
            string saveDataDeck = JsonUtility.ToJson(Deck, true);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream file = File.Create(string.Concat(savePathDeck));
            binaryFormatter.Serialize(file, saveDataDeck);
            file.Close();
        }

        public void SaveCollection()
        {
            File.Delete(savePathCollection);
            string saveDataCollection = JsonUtility.ToJson(Collection, true);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream file = File.Create(string.Concat(savePathCollection));
            binaryFormatter.Serialize(file, saveDataCollection);
            file.Close();
        }
        private void LoadGame()
        {
            var tmpSettigns = new GameSettings();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream file = File.Open(string.Concat(savePathSettings), FileMode.OpenOrCreate, FileAccess.ReadWrite);
            JsonUtility.FromJsonOverwrite(binaryFormatter.Deserialize(file).ToString(), tmpSettigns);
            if (tmpSettigns.GameVersion == Application.version)
            {
                Settings = tmpSettigns;
                CurrentLevel = tmpSettigns.Level;
                if(File.Exists(savePathDeck)) LoadDeck();
                else SaveDeck();
                if (File.Exists(savePathCollection)) LoadCollection();
                else SaveCollection();
                file.Close();
            }
            else
            {
                file.Close();
                File.Delete(savePathSettings);
                file = File.Create((string.Concat(savePathSettings)));
                Settings.BaseDeck = false;
                Settings.TutorialStage = 0;
                Settings.SceneNumber = 0;
                Settings.GameVersion = Application.version;
                Settings.Level = 1;
                Settings.MaxLevelRewardedCard = 2;

                if (File.Exists(savePathDeck))
                {
                    SaveDeck();
                }
                else
                {
                    SaveDeck();
                }
                if (File.Exists(savePathCollection))
                {
                    SaveCollection();
                }
                else
                {
                    SaveCollection();
                }
                string saveDataSettings = JsonUtility.ToJson(Settings, true);
                binaryFormatter.Serialize(file, saveDataSettings);
                file.Close();
            }
            Tutorial.LoadCurrentStage(Settings.TutorialStage);
        }
        private void LoadCollection()
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream file = File.Open(string.Concat(savePathCollection), FileMode.Open, FileAccess.Read);
            JsonUtility.FromJsonOverwrite(binaryFormatter.Deserialize(file).ToString(), Collection);
            file.Close();
        }
        private void LoadDeck()
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream file = File.Open(string.Concat(savePathDeck), FileMode.Open, FileAccess.Read);
            JsonUtility.FromJsonOverwrite(binaryFormatter.Deserialize(file).ToString(), Deck);
            file.Close();
        }
        private void RemoveData()
        {
            File.Delete(savePathDeck);
            File.Delete(savePathCollection);
        }
        #endregion Save/Load
    }
}
