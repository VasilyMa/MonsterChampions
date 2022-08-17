using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;
using UnityEngine;
using Client;
public class CollectionMB : MonoBehaviour
{
    private EcsWorld _world;
    private GameState _state;
    private EcsPool<InterfaceComponent> _interfacePool;
    public void Init(EcsWorld world, GameState state)
    {
        _world = world;
        _state = state;
        _interfacePool = _world.GetPool<InterfaceComponent>();
    }
    public void ToMenu()
    {
        _state.Save();
        ref var interfaceComp = ref _interfacePool.Get(_state.InterfaceEntity);
        interfaceComp.MenuHolder.gameObject.SetActive(true);
        interfaceComp.CollectionMenu.gameObject.SetActive(false);
    }


    public void AddCardToDeck()
    {

    }
}
