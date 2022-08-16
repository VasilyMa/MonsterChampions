using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

namespace Client
{
    sealed class InitInput : MonoBehaviour, IEcsInitSystem
    {
        readonly EcsWorldInject _world = default;

        readonly EcsSharedInject<GameState> _state = default;

        readonly EcsPoolInject<InputComponent> _inputPool = default;
        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;

        public void Init (IEcsSystems systems)
        {
            var entity = _world.Value.NewEntity();
            _state.Value.InputEntity = entity;
            ref var interfaceComp = ref _interfacePool.Value.Get(_state.Value.InterfaceEntity);
            ref var inputComp = ref _inputPool.Value.Add(entity);
            inputComp.Raycaster = interfaceComp.MainCanvas.GetComponent<GraphicRaycaster>();
            inputComp.EventSystem = FindObjectOfType<EventSystem>();
        }
    }
}