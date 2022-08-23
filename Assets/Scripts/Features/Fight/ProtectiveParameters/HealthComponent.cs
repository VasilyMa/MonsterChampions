using UnityEngine;

namespace Client
{
    struct HealthComponent
    {
        public float MaxValue;
        public float CurrentValue;

        public GameObject HealthBar;
        public float HealthBarMaxWidth;
    }
}