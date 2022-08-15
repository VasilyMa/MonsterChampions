using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    public class BaseDamagingZoneMB : MonoBehaviour
    {
        [SerializeField] private GameObject _mainGameObject;
        [SerializeField] private EcsInfoMB _ecsInfoMB;

        private EcsWorldInject _world;
        private int _objectEntity;

        private EcsPool<DamagingEvent> _damagingEventPool;

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

            ref var damagingEvent = ref _damagingEventPool.Add(_world.Value.NewEntity());
            damagingEvent.Invoke(_objectEntity, other.gameObject.GetComponent<EcsInfoMB>().GetEntity());
        }

        private void SetHash()
        {
            _world = _ecsInfoMB.GetWorld();
            _objectEntity = _ecsInfoMB.GetEntity();
            _damagingEventPool = _world.Value.GetPool<DamagingEvent>();

            _isHashed = true;
        }
    }
}
