using UnityEngine;
using System.Collections.Generic;

namespace Client
{
    struct Targetable
    {
        public GameObject TargetObject;
        public int TargetEntity;
        public List<int> EntitysInDetectionZone;
        public List<int> EntitysInMeleeZone;
        public List<int> EntitysInRangeZone;

        public GameObject MeleeZone;
        public GameObject RangeZone;
    }
}