using UnityEngine;

namespace Client
{
    struct ViewComponent
    {
        public int EntityNumber;
        public GameObject GameObject;
        public GameObject Model;
        public Transform Transform;
        public EcsInfoMB EcsInfoMB;
        public CardInfo CardInfo;
        public HealthbarMB HealthBarMB;

        public int LayerBeforeDeath;

        public string AliveUnit;
        public string InteractionZone;
        public string DeadUnit;
    }
}