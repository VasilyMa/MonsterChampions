using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    public class EcsInfoMB : MonoBehaviour
    {
        public int Entity;
        public int unitID;
        public MonstersID.Value monsterID;

        private EcsWorldInject _world;

        private EcsPool<Targetable> _targetablePool;
        private EcsPool<DamagingEvent> _damagingEventPool;
        private EcsPool<DamageComponent> _damageComponentPool;

        [SerializeField] private int _objectEntity;

        public void Init(EcsWorldInject world, int objectEntity)
        {
            _world = world;
            _objectEntity = objectEntity;
            Entity = objectEntity;
            _targetablePool = world.Value.GetPool<Targetable>();
            _damagingEventPool = world.Value.GetPool<DamagingEvent>();
            _damageComponentPool = world.Value.GetPool<DamageComponent>();
        }

        public EcsWorldInject GetWorld()
        {
            return _world;
        }

        public int GetEntity()
        {
            return _objectEntity;
        }

        public void DealMeleeDamage()
        {
            ref var damageComponent = ref _damageComponentPool.Get(_objectEntity);
            ref var targetableComponent = ref _targetablePool.Get(_objectEntity);
            foreach (var targetEntity in targetableComponent.EntitysInMeleeZone)
            {
                ref var damagingEventComponent = ref _damagingEventPool.Add(_world.Value.NewEntity());
                damagingEventComponent.Invoke(targetEntity, _objectEntity, damageComponent.Value);
            }
        }

        public void DealHitscanDamage()
        {
            ref var damageComponent = ref _damageComponentPool.Get(_objectEntity);
            ref var targetableComponent = ref _targetablePool.Get(_objectEntity);

            ref var damagingEventComponent = ref _damagingEventPool.Add(_world.Value.NewEntity());
            damagingEventComponent.Invoke(targetableComponent.TargetEntity, _objectEntity, damageComponent.Value);
        }
    }
}
