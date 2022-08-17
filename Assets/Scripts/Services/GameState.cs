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
        private string savePathCollection =  $"{Application.persistentDataPath}/Collection.Save";
        private string savePathDeck = $"{Application.persistentDataPath}/Deck.Save";
        private EcsWorld _ecsWorld;
        public GetMonster _monsterStorage;
        public Collection Collection = new Collection();
        public Deck Deck = new Deck();
        public PlayableDeck PlayableDeck = new PlayableDeck();
        public int StorageEntity;
        public int InputEntity;
        public int BoardEntity;
        public int InterfaceEntity;
        public int CurrentLevel;
        public bool runSysytem = true, hubSystem;
        //to do ay array of EnemyBases for some quantity bases

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

        public GameState(EcsWorld EcsWorld, GetMonster monsterStorage)
        {
            _ecsWorld = EcsWorld;
            _monsterStorage = monsterStorage;
            if (File.Exists(savePathDeck)&&File.Exists(savePathCollection))
            {
                Load();
            }
            // Load(); to do
        }
        public void InitSotrages()
        {
            
        }
        public void Save()
        {
            SaveCollection();
            SaveDeck();
        }
        public void Load()
        {
            LoadCollection();
            LoadDeck();
        }
        

        #region Save/Load
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
