using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    public class MeleeZoneMB : MonoBehaviour
    {
        [SerializeField] private GameObject _mainGameObject; // to do ay rewrite it on overlapsphere
        [SerializeField] private EcsInfoMB _ecsInfoMB;

        private EcsWorldInject _world;
        private int _objectEntity;

        private EcsPool<Targetable> _targetablePool;

        private bool _isHashed = false;

        private string _enemyTag = "Enemy";
        private string _friendlyTag = "Friendly";
        private string _targetTag;

        void Start()
        {
            if (_mainGameObject == null) _mainGameObject = transform.parent.gameObject;
            if (_ecsInfoMB == null) _ecsInfoMB = _mainGameObject.GetComponent<EcsInfoMB>();

            if (_mainGameObject.CompareTag(_enemyTag))
            {
                _targetTag = _friendlyTag;
            }
            else
            {
                _targetTag = _enemyTag;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.isTrigger)
            {
                return;
            }

            if (!other.gameObject.CompareTag(_targetTag))
            {
                return;
            }

            if (!_isHashed)
            {
                SetHash();
            }

            ref var targetableComponent = ref _targetablePool.Get(_objectEntity);
            targetableComponent.EntitysInMeleeZone.Add(other.GetComponent<EcsInfoMB>().GetEntity());
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.isTrigger)
            {
                return;
            }

            if (!other.gameObject.CompareTag(_targetTag))
            {
                return;
            }

            ref var targetableComponent = ref _targetablePool.Get(_ecsInfoMB.GetEntity());
            targetableComponent.EntitysInMeleeZone.Remove(other.GetComponent<EcsInfoMB>().GetEntity());
        }

        private void SetHash()
        {
            _world = _ecsInfoMB.GetWorld();
            _objectEntity = _ecsInfoMB.GetEntity();
            _targetablePool = _world.Value.GetPool<Targetable>();

            _isHashed = true;
        }
    }
}
