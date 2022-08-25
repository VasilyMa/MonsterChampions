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
        public MergeEffectsPool _mergeEffectsPool;
        public Collection Collection = new Collection();
        public Deck Deck = new Deck();
        public GameSettings Settings = new GameSettings();
        public int StorageEntity;
        public int InputEntity;
        public int BoardEntity;
        public int CameraEntity;
        public int InterfaceEntity;
        public int CurrentLevel = 1;
        public bool runSysytem = false, hubSystem = true;


        public PlayableDeck PlayableDeck = new PlayableDeck();
        private int _playerBaseEntity = -1;
        private List<int> _enemyBaseEntity = new List<int>();
        private int _currentActiveEnemyBaseInArray = -1;
        public const int NULL_ENTITY = -1;

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

        public GameState(EcsWorld EcsWorld, GetMonster monsterStorage, MergeEffectsPool mergeEffectsPool)
        {
            _ecsWorld = EcsWorld;
            _monsterStorage = monsterStorage;
            _mergeEffectsPool = mergeEffectsPool;
            if (File.Exists(savePathDeck) && File.Exists(savePathCollection) && File.Exists(savePathSettings))
            {
                Load();
            }
            else
            {
                Save();
            }
        }
        public void Save()
        {
            SaveGameSetting();
            SaveCollection();
            SaveDeck();
        }
        public void Load()
        {
            LoadGameSettings();
            LoadCollection();
            LoadDeck();
        }


        #region Save/Load
        private void SaveGameSetting()
        {
            string saveDataSettings = JsonUtility.ToJson(Settings, true);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream file = File.Create(string.Concat(savePathSettings));
            binaryFormatter.Serialize(file, saveDataSettings);
            file.Close();
        }
        private void SaveDeck()
        {
            string saveDataDeck = JsonUtility.ToJson(Deck, true);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream file = File.Create(string.Concat(savePathDeck));
            binaryFormatter.Serialize(file, saveDataDeck);
            file.Close();
        }

        private void SaveCollection()
        {
            string saveDataCollection = JsonUtility.ToJson(Collection, true);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream file = File.Create(string.Concat(savePathCollection));
            binaryFormatter.Serialize(file, saveDataCollection);
            file.Close();
        }
        private void LoadGameSettings()
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream file = File.Open(string.Concat(savePathSettings), FileMode.Open, FileAccess.Read);
            JsonUtility.FromJsonOverwrite(binaryFormatter.Deserialize(file).ToString(), Settings);
            file.Close();
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
        #endregion Save/Load
    }
}
