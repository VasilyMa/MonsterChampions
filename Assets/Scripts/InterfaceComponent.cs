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
        /// ������ ������ ��� ����������� � ��������� � �������� ���������
        /// </summary>
        public Transform RewardHolder;
        /// <summary>
        /// ������ ��������� ��� ����������� � ��������� � �������� ���������
        /// </summary>
        public Transform LoseHolder;
        public Transform RewardPanelHolder;
        public Transform MenuHolder;
        public Transform RemoveHolder;
        public Transform AttentionHolder;
        /// <summary>
        /// �����������, ��� �������� ��������� ��� �� ������
        /// </summary>
        public Transform TargetCollection;
        /// <summary>
        /// �����������, ��� �������� �� ������� ���� ��� �� ������
        /// </summary>
        public Transform TargetDeck;
        /// <summary>
        /// �����������, ��� �������� ������ "� ���������" ��� �� ������
        /// </summary>
        public Transform TargetCollectionButton;
        /// <summary>
        /// �����������, ��� �������� ������ "tap to play" ��� �� ������
        /// </summary>
        public Transform TargetPlayButton;
        /// <summary>
        /// �����������, ��� �������� ������� ���� ��� �� ������
        /// </summary>
        public Transform TargetCardPanel;
        /// <summary>
        /// �����������, ��� �������� ������ ��������� ��� ������ ��� �� ������
        /// </summary>
        public Transform TargetLoseWin;
        /// <summary>
        /// �����������, ��� �������� ��������� ��� ��� �� ������
        /// </summary>
        public Transform TargetProgressBar;
        
        /// <summary>
        /// ��������� ��� �������������
        /// </summary>
        public Vector3 defaultPosCardHolder;
        /// <summary>
        /// ��������� ��� �������������
        /// </summary>
        public Vector3 defaultPosProgressHolder;
        /// <summary>
        /// ��������� ��� �������������
        /// </summary>
        public Vector3 defaultPosRemoveButton;
        /// <summary>
        /// ��������� ��� �������������
        /// </summary>
        public Vector3 defaultPosCollectionButton;
        /// <summary>
        /// ��������� ��� �������������
        /// </summary>
        public Vector3 defaultPosPlayButton;
        /// <summary>
        /// ��������� ��� �������������
        /// </summary>
        public Vector3 defaultPosCollection;
        /// <summary>
        /// ��������� ��� �������������
        /// </summary>
        public Vector3 defaultPosCardPanel;
        /// <summary>
        /// ��������� ��� �������������
        /// </summary>
        public Vector3 deafaultPosDeck;



        public StartDeckForDevelop TempButton;

        public Canvas MainCanvas;
    }
}