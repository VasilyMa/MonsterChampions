using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    struct ThunderboltComponent
    {
        public bool isCausedDamage;
        public List<TinkiThunderboltEffect> ThunderboltEffects;
    }

    public class TinkiThunderboltEffect
    {
        public bool isMoved;
        public GameObject Object;
        public Vector3 Destination;
    }
}