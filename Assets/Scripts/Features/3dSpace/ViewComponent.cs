using UnityEngine;
using System.Collections.Generic;

namespace Client
{
    struct ViewComponent
    {
        public int EntityNumber;
        public GameObject GameObject;
        public GameObject Model;
        public Transform Transform;
        public EcsInfoMB EcsInfoMB;
        public List<MonsterVisualAndAnimations> VisualAndAnimations;
        public HealthbarMB HealthBarMB;

        public int LayerBeforeDeath;

        public string OnBoardUnit;
        public string AliveUnit;
        public string InteractionZone;
        public string DeadUnit;
    }
}