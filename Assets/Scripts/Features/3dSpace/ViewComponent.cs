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

        public int LayerBeforeDeath;

        public string DeadUnit;
    }
}