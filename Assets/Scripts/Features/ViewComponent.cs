using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    struct ViewComponent
    {
        public int EntityNumber;
        public GameObject GameObject;
        public Transform Transform;
        public Rigidbody Rigidbody;
    }
}