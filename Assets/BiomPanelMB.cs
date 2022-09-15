using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.EcsLite;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Client 
{
    public class BiomPanelMB : MonoBehaviour
    {
        private EcsWorld _world;
        private GameState _state;
        private EcsPool<InterfaceComponent> _interfacePool;
        private Biom CurrentBiom = new Biom();
        [SerializeField] private Image _currentBiomImage;
        [SerializeField] private Image _nextBiomImage;
        [SerializeField] private Transform _biomPointHolder;
        [SerializeField] private GameObject _biomPointPrefab;
        [SerializeField] private Text _currentLevel;
        public void Init(EcsWorld world, GameState state)
        {
            _world = world;
            _state = state;
            _interfacePool = _world.GetPool<InterfaceComponent>();
            InitBiom();
            InitBiomPoints();
        }
        public void InitBiom()
        {
            var interfaceStorage = _state.InterfaceConfigs;
            for (int b = 0; b < interfaceStorage.StartBiomLevels.Length; b++)
            {
                var biom = new Biom();
                biom.StartBiomLevel = interfaceStorage.StartBiomLevels[b];
                biom.BiomType = interfaceStorage.StartBiomTypes[b];
                biom.BiomSprite = interfaceStorage.BiomsSprites[b];
                if (b + 1 < interfaceStorage.StartBiomLevels.Length)
                {
                    biom.NextBiomSprite = interfaceStorage.BiomsSprites[b + 1];
                }
                else
                {
                    biom.NextBiomSprite = interfaceStorage.BiomsSprites[0];
                }
                if (b == 0 || (b > 0 && b < interfaceStorage.StartBiomLevels.Length - 1))
                {
                    int length = interfaceStorage.StartBiomLevels[b + 1] - interfaceStorage.StartBiomLevels[b];
                    biom.BiomLevels = new List<int>();
                    for (int i = 0; i < length; i++) biom.BiomLevels.Add(interfaceStorage.StartBiomLevels[b] + i);
                }
                else if (b == interfaceStorage.StartBiomLevels.Length - 1)
                {
                    int length = (SceneManager.sceneCountInBuildSettings - 1) - (interfaceStorage.StartBiomLevels[b] - 1);
                    biom.BiomLevels = new List<int>();
                    for (int i = 0; i < length; i++) biom.BiomLevels.Add(interfaceStorage.StartBiomLevels[b] + i);
                }

                interfaceStorage.Bioms = new List<Biom>();
                interfaceStorage.Bioms.Add(biom);

                if (biom.BiomLevels.Contains(SceneManager.GetActiveScene().buildIndex))
                    CurrentBiom = biom;
            }
            _currentLevel.text = $"Level {_state.CurrentLevel}";
        }
        public void InitBiomPoints()
        {
            var interfaceStorage = _state.InterfaceConfigs;
            _currentBiomImage.sprite = CurrentBiom.BiomSprite;
            _nextBiomImage.sprite = CurrentBiom.NextBiomSprite;

            for (int i = 0; i < CurrentBiom.BiomLevels.Count; i++)
            {
                var lvl = CurrentBiom.BiomLevels[i];
                var currentScene = SceneManager.GetActiveScene().buildIndex;
                if (currentScene < lvl)
                {
                    var image = Instantiate(_biomPointPrefab, _biomPointHolder).GetComponent<Image>();
                    image.color = interfaceStorage.AnCompletePointColor;
                }
                else if (currentScene == lvl)
                {
                    var image = Instantiate(_biomPointPrefab, _biomPointHolder).GetComponent<Image>();
                    image.color = interfaceStorage.CurrentPointColor;
                    image.transform.localScale = new Vector3(1, 1.2f, 1);
                }
                else
                {
                    var image = Instantiate(_biomPointPrefab, _biomPointHolder).GetComponent<Image>();
                    image.color = interfaceStorage.CompletePointColor;
                }
            }
        }
    }
}
