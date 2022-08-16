using System.Collections.Generic;
using UnityEngine;
namespace Client {
    struct InterfaceComponent {
        public BuyCardMB BuyCard;
        public Transform HolderCards;
        public List<GameObject> cards;

        public List<GameObject> CollectionCards;
        public List<GameObject> DeckCards;
        public Transform CollectionHolder;
        public Transform DeckHolder;
        public CollectionMB CollectionManager;

        public Canvas MainCanvas;
    }
}