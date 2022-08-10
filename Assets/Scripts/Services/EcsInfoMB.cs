using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    public class EcsInfoMB : MonoBehaviour
    {
        private EcsWorldInject _world;

        private EcsPool<Targetable> _targetablePool;

        public void Init(EcsWorldInject world)
        {
            _world = world;
            _targetablePool = world.Value.GetPool<Targetable>();
        }

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
