using System.Collections.Generic;
using UnityEngine;
namespace Client {
    struct InterfaceComponent {
        public BuyCardMB BuyCard;
        public Transform HolderCards;
        public List<GameObject> cards;

        public List<GameObject> CollectionCards;
        public List<GameObject> DeckCards;
        public Transform Hide;
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
        /// <summary>
        /// Панель наград для перемещения и обращеиня к дочерним элементам
        /// </summary>
        public Transform RewardHolder;
        /// <summary>
        /// Панель поражения для перемещения и обращеиня к дочерним элементам
        /// </summary>
        public Transform LoseHolder;
        public Transform RewardPanelHolder;
        public Transform MenuHolder;
        public Transform RemoveHolder;
        public Transform AttentionHolder;
        /// <summary>
        /// Коородинаты, где окажется коллекция при ее вызове
        /// </summary>
        public Transform TargetCollection;
        /// <summary>
        /// Коородинаты, где окажется НЕ игровая рука при ее вызове
        /// </summary>
        public Transform TargetDeck;
        /// <summary>
        /// Коородинаты, где окажется кнопка "в коллекцию" при ее вызове
        /// </summary>
        public Transform TargetCollectionButton;
        /// <summary>
        /// Коородинаты, где окажется кнопка "tap to play" при ее вызове
        /// </summary>
        public Transform TargetPlayButton;
        /// <summary>
        /// Коородинаты, где окажется игровая рука при ее вызове
        /// </summary>
        public Transform TargetCardPanel;
        /// <summary>
        /// Коородинаты, где окажется панель поражения или победы при ее вызове
        /// </summary>
        public Transform TargetLoseWin;
        /// <summary>
        /// Коородинаты, где окажется прогрресс бар при ее вызове
        /// </summary>
        public Transform TargetProgressBar;
        
        /// <summary>
        /// Положение при инициализации
        /// </summary>
        public Vector3 defaultPosCardHolder;
        /// <summary>
        /// Положение при инициализации
        /// </summary>
        public Vector3 defaultPosProgressHolder;
        /// <summary>
        /// Положение при инициализации
        /// </summary>
        public Vector3 defaultPosRemoveButton;
        /// <summary>
        /// Положение при инициализации
        /// </summary>
        public Vector3 defaultPosCollectionButton;
        /// <summary>
        /// Положение при инициализации
        /// </summary>
        public Vector3 defaultPosPlayButton;
        /// <summary>
        /// Положение при инициализации
        /// </summary>
        public Vector3 defaultPosCollection;
        /// <summary>
        /// Положение при инициализации
        /// </summary>
        public Vector3 defaultPosCardPanel;
        /// <summary>
        /// Положение при инициализации
        /// </summary>
        public Vector3 deafaultPosDeck;



        public StartDeckForDevelop TempButton;

        public Canvas MainCanvas;
    }
}