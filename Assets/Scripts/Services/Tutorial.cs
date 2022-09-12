using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    public static class Tutorial
    {
        public static Stage CurrentStage { get; private set; } = (Stage)0;

        public static bool StageIsEnable { get; set; } = false;

        public enum Stage
        {
            TwoBuysMonsters = 0,
            MergeMonsters = 1,
            DragAndDropMonster = 2,
            OpenCollection = 3,
            DragAndDropNewCardInDeck = 4,

            TutorialsOver = 5,
        }

        #region Servises
        public static void SetNextStage(EcsSharedInject<GameState> gameState, bool isSave = true)
        {
            int nextStage = (int)CurrentStage + 1;

            if (nextStage > (int)Stage.TutorialsOver)
            {
                Debug.LogWarning("You try set next tutorial stage on last stage.");
            }
            else
            {
                CurrentStage = (Stage)nextStage;
                gameState.Value.Settings.TutorialStage = nextStage;

                if (isSave)
                {
                    gameState.Value.SaveGameSetting();
                }

                Debug.Log("Change stage on "+ CurrentStage);
            }
        }

        public static bool isOver()
        {
            return ((int)CurrentStage >= (int)Stage.TutorialsOver);
        }

        public static bool StageIsTutorialOver(Stage stage)
        {
            return ((int)stage >= (int)Stage.TutorialsOver);
        }

        public static bool StageIsTutorialOver(int stage)
        {
            return (stage >= (int)Stage.TutorialsOver);
        }

        public static void LoadCurrentStage(int currentStage)
        {
            CurrentStage = (Stage)currentStage;
        }

        public static bool isStage(Stage stage)
        {
            return stage == CurrentStage;
        }
        #endregion

        #region Stages

        #region TwoBuysMonsters
        public static class TwoBuysMonsters
        {
            private static int _maxSpawnsValue = 2;
            private static int _SpawnsCurrentValue = 0;

            public static void AddSpawn()
            {
                if (_SpawnsCurrentValue < _maxSpawnsValue)
                {
                    _SpawnsCurrentValue++;
                }
            }

            public static int GetSpawnsValue()
            {
                return _SpawnsCurrentValue;
            }

            public static int GetMaxSpawnsValue()
            {
                return _maxSpawnsValue;
            }
        }
        #endregion

        #region MergeMonsters
        public static class MergeMonsters
        {
            private static bool _monstersIsMerged = false;

            public static void SetMonstersIsMerged()
            {
                _monstersIsMerged = true;
            }

            public static bool isMerged()
            {
                return _monstersIsMerged;
            }
        }
        #endregion

        #region DragAndDropMonster
        public static class DragAndDropMonster
        {
            private static bool _tryDropSoFar = false;
            private static bool _monsterIsDropped = false;

            public static void SetTryingFarDrop()
            {
                _tryDropSoFar = true;
            }

            public static bool isTryDropSoFar()
            {
                return _tryDropSoFar;
            }

            public static void SetMonsterIsDropped()
            {
                _monsterIsDropped = true;
            }

            public static bool isDropped()
            {
                return _monsterIsDropped;
            }
        }
        #endregion

        #region OpenCollection
        public static class OpenCollection
        {
            private static bool _collectionIsOpened = false;

            public static void SetCollectionIsOpened()
            {
                _collectionIsOpened = true;
            }

            public static bool isOpened()
            {
                return _collectionIsOpened;
            }
        }
        #endregion

        #region DragAndDropNewCardInDeck
        public static class DragAndDropNewCardInDeck
        {
            private static bool _panelIsOpened = false;

            private static bool _cardIsDragged = false;
            private static bool _cardIsDroppedBack = false;
            private static bool _cardIsDroppedInDeck = false;

            public static void SetPanelIsOpened()
            {
                _panelIsOpened = true;
            }

            public static bool PanelIsOpened()
            {
                return _panelIsOpened;
            }

            public static void SetCardIsDragged()
            {
                _cardIsDragged = true;

                _cardIsDroppedBack = false;
            }

            public static bool isDragged()
            {
                return _cardIsDragged;
            }

            public static void SetCardIsDroppedBack()
            {
                _cardIsDroppedBack = true;

                _cardIsDragged = false;
            }

            public static bool isDroppedBack()
            {
                return _cardIsDroppedBack;
            }

            public static void SetCardIsDroppedInDeck()
            {
                _cardIsDroppedInDeck = true;

                _cardIsDragged = false;
            }

            public static bool isDroppedInDeck()
            {
                return _cardIsDroppedInDeck;
            }
        }

        #endregion

        #endregion
    }
}
