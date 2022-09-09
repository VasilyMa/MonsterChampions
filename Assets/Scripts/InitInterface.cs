using DG.Tweening;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Client {
    sealed class InitInterface : MonoBehaviour, IEcsInitSystem  
    {
        readonly EcsWorldInject _world = default;

        readonly EcsSharedInject<GameState> _state = default;

        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;
        readonly EcsPoolInject<PlayableDeckEvent> _playDeckPool = default;

        private int _startGoldValue = 20;

        public void Init(IEcsSystems systems)
        {
            var entity = _world.Value.NewEntity();
            _state.Value.InterfaceEntity = entity;
            ref var interfaceComp = ref _interfacePool.Value.Add(entity);
            interfaceComp.MainCanvas = FindObjectOfType<Canvas>();
            interfaceComp.BuyCard = FindObjectOfType<BuyCardMB>();
            interfaceComp.MainMenu = FindObjectOfType<MenuMB>();
            interfaceComp.RewardPanel = FindObjectOfType<RewardPanelMB>();
            interfaceComp.LosePanel = FindObjectOfType<LosePanelMB>();
            interfaceComp.Reward = FindObjectOfType<RewardMB>();
            interfaceComp.Resources = FindObjectOfType<ResourcesMB>();
            interfaceComp.Progress = FindObjectOfType<ProgressMB>();

            interfaceComp.TargetCardPanel = GameObject.Find("TargetCardPanel").transform;
            interfaceComp.TargetCollection = GameObject.Find("TargetCollection").transform;
            interfaceComp.TargetCollectionButton = GameObject.Find("TargetCollectionName").transform;
            interfaceComp.TargetDeck = GameObject.Find("TargetDeck").transform;
            interfaceComp.TargetLoseWin = GameObject.Find("TargetLoseWin").transform;
            interfaceComp.TargetProgressBar = GameObject.Find("TargetProgress").transform;
            interfaceComp.TargetPlayButton = GameObject.Find("TargetPlayButton").transform;

            interfaceComp.RewardPanelHolder = interfaceComp.RewardPanel.transform;
            interfaceComp.HolderCards = interfaceComp.BuyCard.transform;
            interfaceComp.LoseHolder = interfaceComp.LosePanel.transform.GetChild(0).transform;
            interfaceComp.RewardHolder = interfaceComp.Reward.transform;
            interfaceComp.AttentionHolder = interfaceComp.MainMenu.transform.GetChild(2).transform;
            interfaceComp.BuyCard.Init(systems.GetWorld(), systems.GetShared<GameState>());
            interfaceComp.RewardPanel.Init(systems.GetWorld(), systems.GetShared<GameState>());
            interfaceComp.LosePanel.Init(systems.GetWorld(), systems.GetShared<GameState>());
            interfaceComp.Reward.Init(systems.GetWorld(), systems.GetShared<GameState>());
            interfaceComp.Resources.Init(systems.GetWorld(), systems.GetShared<GameState>());
            interfaceComp.Progress.Init(systems.GetWorld(), systems.GetShared<GameState>());

            interfaceComp.CollectionManager = FindObjectOfType<CollectionMB>();
            interfaceComp.CollectionMenu = interfaceComp.CollectionManager.transform;
            interfaceComp.CollectionHolder = interfaceComp.CollectionManager.transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform;
            interfaceComp.DeckHolder = interfaceComp.CollectionManager.transform.GetChild(0).transform;
            interfaceComp.RemoveHolder = interfaceComp.CollectionMenu.transform.GetChild(2).transform;
            interfaceComp.CollectionCards = new System.Collections.Generic.List<GameObject>();
            interfaceComp.DeckCards = new System.Collections.Generic.List<GameObject>();
            interfaceComp.CollectionManager.Init(systems.GetWorld(), systems.GetShared<GameState>());
            interfaceComp.MenuHolder = interfaceComp.MainMenu.transform;
            interfaceComp.MainMenu.Init(systems.GetWorld(), systems.GetShared<GameState>());


            interfaceComp.defaultPosCollectionButton = interfaceComp.MenuHolder.transform.GetChild(1).transform.position;
            interfaceComp.defaultPosPlayButton = interfaceComp.MenuHolder.transform.GetChild(0).transform.GetChild(0).transform.position;
            interfaceComp.defaultPosCollection = interfaceComp.CollectionMenu.transform.GetChild(1).transform.position;
            interfaceComp.defaultPosCardPanel = interfaceComp.HolderCards.transform.position;
            interfaceComp.deafaultPosDeck = interfaceComp.DeckHolder.transform.position;
            interfaceComp.defaultPosProgressHolder = interfaceComp.Progress.transform.GetChild(0).transform.position;
            interfaceComp.defaultPosCardHolder = interfaceComp.HolderCards.transform.position;
            interfaceComp.defaultPosRemoveButton = interfaceComp.CollectionMenu.transform.GetChild(2).transform.position;

            interfaceComp.MainMenu.UpdateDeck();
            interfaceComp.HolderCards.gameObject.SetActive(false);
            interfaceComp.RewardPanelHolder.gameObject.SetActive(false);
            interfaceComp.LoseHolder.gameObject.SetActive(false);
            interfaceComp.RewardHolder.gameObject.SetActive(false);
            interfaceComp.Resources.gameObject.SetActive(false);
            interfaceComp.AttentionHolder.gameObject.SetActive(false);

            _state.Value.AddPlayerGold(_startGoldValue); // to do ay write it in another system

            if (_state.Value.Settings.TutorialStage == 0)
            {
                _state.Value.hubSystem = false;
                _state.Value.runSysytem = true;
                _playDeckPool.Value.Add(_world.Value.NewEntity());
                interfaceComp.HolderCards.gameObject.SetActive(true);
                interfaceComp.MenuHolder.gameObject.SetActive(false);
                interfaceComp.Resources.gameObject.SetActive(true);
                interfaceComp.DeckHolder.gameObject.SetActive(false);
                interfaceComp.Resources.UpdatePlayerCoinAmount();
                interfaceComp.HolderCards.transform.DOMove(GameObject.Find("TargetCardPanel").transform.position, 1f, false);
                interfaceComp.Progress.transform.GetChild(0).transform.DOMove(GameObject.Find("TargetProgress").transform.position, 1f, false);
            }
            else 
            {
                interfaceComp.MenuHolder.gameObject.SetActive(true);
                _state.Value.hubSystem = true;
                _state.Value.runSysytem = false;
                interfaceComp.Resources.UpdatePlayerCoinAmount();
            }
        }
    }
}