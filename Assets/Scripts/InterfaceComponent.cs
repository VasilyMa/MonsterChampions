using System.Collections.Generic;
using UnityEngine;
namespace Client {
    struct InterfaceComponent {
        public BuyCardMB BuyCard;
        public Transform HolderCards;
        public List<GameObject> cards;

        public List<GameObject> CollectionCards;
        public List<GameObject> DeckCards;
        public Transform CollectionMenu;
        public Transform CollectionHolder;
        public Transform DeckHolder;
        public CollectionMB CollectionManager;
        public MenuMB MainMenu;
        public RewardPanelMB RewardPanel;
        public LosePanelMB LosePanel;
        public RewardMB Reward;
        public ResourcesMB Resources;
        public ProgressMB Progress;

        public Transform RewardHolder;
        public Transform LoseHolder;
        public Transform RewardPanelHolder;
        public Transform MenuHolder;

        public Vector3 defaultPosCardHolder;
        public Vector3 defaultPosProgressHolder;

        public StartDeckForDevelop TempButton;

        public Canvas MainCanvas;
    }
}