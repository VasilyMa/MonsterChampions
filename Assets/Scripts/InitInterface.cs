using DG.Tweening;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Client {
    sealed class InitInterface : MonoBehaviour, IEcsInitSystem  
    {
        readonly EcsWorldInject _world = default;

        readonly EcsSharedInject<GameState> _state = default;

        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;

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
            interfaceComp.RewardPanelHolder = interfaceComp.RewardPanel.transform;
            interfaceComp.HolderCards = interfaceComp.BuyCard.transform;
            interfaceComp.LoseHolder = interfaceComp.LosePanel.transform.GetChild(0).transform;
            interfaceComp.RewardHolder = interfaceComp.Reward.transform;
            interfaceComp.BuyCard.Init(systems.GetWorld(), systems.GetShared<GameState>());
            interfaceComp.RewardPanel.Init(systems.GetWorld(), systems.GetShared<GameState>());
            interfaceComp.LosePanel.Init(systems.GetWorld(), systems.GetShared<GameState>());
            interfaceComp.Reward.Init(systems.GetWorld(), systems.GetShared<GameState>());

            interfaceComp.CollectionManager = FindObjectOfType<CollectionMB>();
            interfaceComp.CollectionMenu = interfaceComp.CollectionManager.transform;
            interfaceComp.CollectionHolder = interfaceComp.CollectionManager.transform.GetChild(1).transform.GetChild(0).transform;
            interfaceComp.DeckHolder = interfaceComp.CollectionManager.transform.GetChild(0).transform;
            interfaceComp.CollectionCards = new System.Collections.Generic.List<GameObject>();
            interfaceComp.DeckCards = new System.Collections.Generic.List<GameObject>();
            interfaceComp.CollectionManager.Init(systems.GetWorld(), systems.GetShared<GameState>());
            interfaceComp.MenuHolder = interfaceComp.MainMenu.transform;
            interfaceComp.MainMenu.Init(systems.GetWorld(), systems.GetShared<GameState>());

            interfaceComp.MainMenu.UpdateDeck();
            interfaceComp.HolderCards.gameObject.SetActive(false);
            interfaceComp.RewardPanelHolder.gameObject.SetActive(false);
            interfaceComp.LoseHolder.gameObject.SetActive(false);
            interfaceComp.RewardHolder.gameObject.SetActive(false);
            interfaceComp.DeckHolder.DOMove((GameObject.Find("TargetDeck").transform.position), 1f, false);
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                interfaceComp.MenuHolder.gameObject.SetActive(true);
                _state.Value.hubSystem = true;
                _state.Value.runSysytem = false;
            }
            else
            {
                interfaceComp.MenuHolder.gameObject.SetActive(false);
                interfaceComp.HolderCards.gameObject.SetActive(true);
            }
                
        }
    }
}