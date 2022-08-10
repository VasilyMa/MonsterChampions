using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    public class GameState
    {
        private EcsWorld _ecsWorld;

        //to do ay array of EnemyBases for some quantity bases

        public EcsWorld EcsWorld
        {
            get
            {
                return _ecsWorld;
            }
            set
            {
                _ecsWorld = value;
            }
        }

        public GameState(EcsWorld EcsWorld)
        {

        }
    }
}
