using UnityEngine;

namespace Client
{
    struct SetPointToMoveEvent
    {
        public Vector3 DestinationPoint;
        public bool ToEnemyBase;
    }
}