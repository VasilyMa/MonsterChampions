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

        public static void SetNextStage(EcsSharedInject<GameState> gameState)
        {
            int nextStage = (int)CurrentStage + 1;
            if (nextStage >= (int)Stage.TutorialsOver)
            {
                CurrentStage = Stage.TutorialsOver;
                Debug.LogWarning("You try set next tutorial stage on last stage.");
            }
            else
            {
                CurrentStage = (Stage)nextStage;
                gameState.Value.Settings.TutorialStage++;
                gameState.Value.SaveGameSetting();
            }
        }

        public static bool isOver()
        {
            return ((int)CurrentStage >= (int)Stage.TutorialsOver);
        }

        public static bool StageisOver(Stage stage)
        {
            return ((int)stage >= (int)Stage.TutorialsOver);
        }

        public static bool StageisOver(int stage)
        {
            return (stage >= (int)Stage.TutorialsOver);
        }

        public static void LoadCurrentStage(int currentStage)
        {
            CurrentStage = (Stage)currentStage;
        }

        #region Stages

        #region TwoBuysMonsters
        public static class TwoBuysMonsters
        {
            private static int _maxBuysValue = 2;
            private static int _buysCurrentValue = 0;

            public static void AddBuys()
            {
                if (_buysCurrentValue < _maxBuysValue)
                {
                    _buysCurrentValue++;
                }
            }

            public static int GetBuysValue()
            {
                return _buysCurrentValue;
            }

            public static int GetMaxBuysValue()
            {
                return _maxBuysValue;
            }
        }
        #endregion

        #endregion
    }
}
