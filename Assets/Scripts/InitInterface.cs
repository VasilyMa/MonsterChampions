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
            interfaceComp.TempButton = FindObjectOfType<StartDeckForDevelop>();
            interfaceComp.RewardPanel = FindObjectOfType<RewardPanelMB>();
            interfaceComp.LosePanel = FindObjectOfType<LosePanelMB>();
            interfaceComp.RewardHolder = interfaceComp.RewardPanel.transform;
            interfaceComp.HolderCards = interfaceComp.BuyCard.transform;
            interfaceComp.LoseHolder = interfaceComp.LosePanel.transform;
            interfaceComp.BuyCard.Init(systems.GetWorld(), systems.GetShared<GameState>());
            interfaceComp.TempButton.Init(systems.GetWorld(), systems.GetShared<GameState>());
            interfaceComp.RewardPanel.Init(systems.GetWorld(), systems.GetShared<GameState>());
            interfaceComp.LosePanel.Init(systems.GetWorld(), systems.GetShared<GameState>());

            interfaceComp.CollectionManager = FindObjectOfType<CollectionMB>();
            interfaceComp.CollectionMenu = interfaceComp.CollectionManager.transform;
            interfaceComp.CollectionHolder = interfaceComp.CollectionManager.transform.GetChild(1).transform.GetChild(0).transform;
            interfaceComp.DeckHolder = interfaceComp.CollectionManager.transform.GetChild(0).transform;
            interfaceComp.CollectionCards = new System.Collections.Generic.List<GameObject>();
            interfaceComp.DeckCards = new System.Collections.Generic.List<GameObject>();
            interfaceComp.CollectionManager.Init(systems.GetWorld(), systems.GetShared<GameState>());
            interfaceComp.MenuHolder = interfaceComp.MainMenu.transform;
            interfaceComp.MainMenu.Init(systems.GetWorld(), systems.GetShared<GameState>());
            

            
            /*if (_state.Value.Deck.DeckPlayer.LongLength > 0)
            {
                foreach (var card in _state.Value.Deck.DeckPlayer)
                {
                    if (card.UnitID != 0)
                    {
                        var addedCard = (GameObject)GameObject.Instantiate(Resources.Load("CollectionCard"), interfaceComp.DeckHolder);
                        var cardInfo = addedCard.GetComponent<CardInfo>();
                        cardInfo.unitID = card.UnitID;
                    }
                }
            }*/
            interfaceComp.CollectionMenu.gameObject.SetActive(false);
            interfaceComp.HolderCards.gameObject.SetActive(false);
            interfaceComp.RewardHolder.gameObject.SetActive(false);
            interfaceComp.LoseHolder.gameObject.SetActive(false);
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                interfaceComp.MenuHolder.gameObject.SetActive(true);
            }
            else
            {
                interfaceComp.TempButton.gameObject.SetActive(false);
                interfaceComp.MenuHolder.gameObject.SetActive(false);
                interfaceComp.HolderCards.gameObject.SetActive(true);
            }
                
        }
    }
}