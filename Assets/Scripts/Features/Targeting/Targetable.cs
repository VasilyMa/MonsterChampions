using UnityEngine;
using System.Collections.Generic;

namespace Client
{
    struct Targetable
    {
        public GameObject TargetObject;
        public int TargetEntity;
        public List<int> AllEntityInDetectionZone;
        public List<int> EntitysInMeleeZone;
        public List<int> EntitysInRangeZone;
    }
}