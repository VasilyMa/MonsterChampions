using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class InitBoard : IEcsInitSystem
    {
        public void Init (IEcsSystems systems)
        {
            var board = GameObject.FindObjectOfType<BoardMB>().gameObject;
        }
    }
}