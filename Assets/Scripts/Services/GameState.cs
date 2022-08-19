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
        private string savePathCollection = "/Collection.Save";
        private string savePathDeck = "/Deck.Save";
        private EcsWorld _ecsWorld;
        public GetMonster _monsterStorage;
        public Collection Collection = new Collection();
        public Deck Deck = new Deck();
        //public PlayableDeck PlayableDeck = new PlayableDeck();
        public int StorageEntity;
        public int InputEntity;
        public bool runSysytem = true, hubSystem;

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
            FileStream file = File.Create(string.Concat(Application.persistentDataPath, savePathDeck));
            binaryFormatter.Serialize(file, saveDataDeck);
            file.Close();
        }

        private void SaveCollection()
        {
            string saveDataCollection = JsonUtility.ToJson(Collection, true);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream file = File.Create(string.Concat(Application.persistentDataPath, savePathCollection));
            binaryFormatter.Serialize(file, saveDataCollection);
            file.Close();
        }
        private void LoadCollection()
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream file = File.Open(string.Concat(Application.persistentDataPath, savePathCollection), FileMode.Open, FileAccess.Read);
            JsonUtility.FromJsonOverwrite(binaryFormatter.Deserialize(file).ToString(), Collection);
            file.Close();
        }
        private void LoadDeck()
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream file = File.Open(string.Concat(Application.persistentDataPath, savePathDeck), FileMode.Open, FileAccess.Read);
            JsonUtility.FromJsonOverwrite(binaryFormatter.Deserialize(file).ToString(), Deck);
            file.Close();
        }
        #endregion Save/Load
    }
}
