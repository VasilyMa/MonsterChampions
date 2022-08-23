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
        public Transform LoseHolder;
        public Transform RewardHolder;
        public Transform MenuHolder;

        public StartDeckForDevelop TempButton;

        public Canvas MainCanvas;
    }
}