using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class RefreshHealthBarEventSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<RefreshHealthBarEvent>> _refreshEventFilter = default;

        readonly EcsPoolInject<RefreshHealthBarEvent> _refreshHealthBarEventPool = default;
        readonly EcsPoolInject<HealthComponent> _healthComponent = default;

        public void Run (IEcsSystems systems)
        {
            foreach (var eventEntity in _refreshEventFilter.Value)
            {
                ref var refreshHealthBarEvent = ref _refreshHealthBarEventPool.Value.Get(eventEntity);

                ref var healthComponent = ref _healthComponent.Value.Get(refreshHealthBarEvent.UnitEntity);

                if (healthComponent.HealthBar == null)
                {
                    continue;
                }

                if (!healthComponent.HealthBar.activeSelf)
                {
                    healthComponent.HealthBar.SetActive(true);
                }

                healthComponent.HealthBar.transform.localScale =  new Vector3(
                    healthComponent.HealthBarMaxWidth * (healthComponent.CurrentValue / healthComponent.MaxValue),
                    healthComponent.HealthBar.transform.localScale.y, 
                    healthComponent.HealthBar.transform.localScale.z
                    );

                DeleteEvent(eventEntity);
            }
        }

        private void DeleteEvent(int eventEntity)
        {
            _refreshHealthBarEventPool.Value.Del(eventEntity);
        }
    }
}