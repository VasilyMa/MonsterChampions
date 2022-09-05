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
            TapToPlay = 0,
            TwoBuysMonsters = 1,
            MergeMonsters = 2,
            DragAndDropMonster = 3,
            OpenCollection = 4,
            DragAndDropNewCardInDeck = 5,

            TutorialsOver = 6,
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

        public static bool isOver(Stage stage)
        {
            return ((int)stage >= (int)Stage.TutorialsOver);
        }
    }
}
