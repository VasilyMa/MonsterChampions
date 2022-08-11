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

        [SerializeField] private int _objectEntity;

        public void Init(EcsWorldInject world, int objectEntity)
        {
            _world = world;
            _objectEntity = objectEntity;
            _targetablePool = world.Value.GetPool<Targetable>();
        }

        public EcsWorldInject GetWorld()
        {
            return _world;
        }

        public int GetEntity()
        {
            return _objectEntity;
        }
    }
}
