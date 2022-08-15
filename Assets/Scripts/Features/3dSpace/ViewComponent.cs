using UnityEngine;

namespace Client
{
    struct ViewComponent
    {
        public int EntityNumber;
        public GameObject GameObject;
        public Transform Transform;
        public EcsInfoMB EcsInfoMB;
    }
}