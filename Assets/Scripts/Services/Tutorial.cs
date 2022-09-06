using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public static class Tutorial
    {
        public static Stage CurrentStage { get; private set; } = (Stage)0;

        public enum Stage
        {
            TwoBuysMonsters = 0,
            MergeMonsters = 1,
            DragAndDropMonster = 2,
            OpenCollection = 3,
            DragAndDropNewCardInDeck = 4,

            TutorialsOver = 5,
        }

        public static void SetNextStage()
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
    }
}
