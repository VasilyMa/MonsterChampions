using UnityEngine;

namespace Client
{
    struct SetPointToMoveEvent
    {
        public Vector3 NewDestination;
        public Vector3 OldDestination;
        public bool ToEnemyBase;
    }
}