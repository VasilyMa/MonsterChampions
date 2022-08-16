using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;
using UnityEngine;
using Client;
public class CollectionMB : MonoBehaviour
{
    private EcsWorld _world;
    private GameState _state;
    public void Init(EcsWorld world, GameState state)
    {
        _world = world;
        _state = state;
    }
    public void AddCardToDeck()
    {

    }
}
