using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public static class Elemental
    {
        private const float HIGH_RESIST = 2;
        private const float MEDIUM_RESIST = 1;
        private const float LOW_RESIST = 0.5f;
        private const float DEFAULT_MEDIUM_RESIST = 1;

        public static float GetDamageDivider(ElementalType damageElemental, ElementalType healthElemental)
        {
            switch (damageElemental)
            {
                case ElementalType.Earth:
                    return GetEarthDivider(healthElemental);
                case ElementalType.Air:
                    return GetAirDivider(healthElemental);
                case ElementalType.Darkness:
                    return GetDarknessDivider(healthElemental);
                case ElementalType.Fire:
                    return GetFireDivider(healthElemental);
                case ElementalType.Water:
                    return GetWaterDivider(healthElemental);
                default:
                    return DEFAULT_MEDIUM_RESIST;
            }
        }

        private static float GetEarthDivider(ElementalType healthElemental)
        {
            switch (healthElemental)
            {
                case ElementalType.Earth:
                    return HIGH_RESIST;
                case ElementalType.Air:
                    return LOW_RESIST;
                case ElementalType.Darkness:
                    return LOW_RESIST;
                case ElementalType.Fire:
                    return MEDIUM_RESIST;
                case ElementalType.Water:
                    return MEDIUM_RESIST;
                default:
                    return DEFAULT_MEDIUM_RESIST;
            }
        }

        private static float GetAirDivider(ElementalType healthElemental)
        {
            switch (healthElemental)
            {
                case ElementalType.Earth:
                    return LOW_RESIST;
                case ElementalType.Air:
                    return HIGH_RESIST;
                case ElementalType.Darkness:
                    return LOW_RESIST;
                case ElementalType.Fire:
                    return MEDIUM_RESIST;
                case ElementalType.Water:
                    return MEDIUM_RESIST;
                default:
                    return DEFAULT_MEDIUM_RESIST;
            }
        }

        private static float GetDarknessDivider(ElementalType healthElemental)
        {
            switch (healthElemental)
            {
                case ElementalType.Earth:
                    return LOW_RESIST;
                case ElementalType.Air:
                    return LOW_RESIST;
                case ElementalType.Darkness:
                    return HIGH_RESIST;
                case ElementalType.Fire:
                    return LOW_RESIST;
                case ElementalType.Water:
                    return LOW_RESIST;
                default:
                    return DEFAULT_MEDIUM_RESIST;
            }
        }

        private static float GetFireDivider(ElementalType healthElemental)
        {
            switch (healthElemental)
            {
                case ElementalType.Earth:
                    return MEDIUM_RESIST;
                case ElementalType.Air:
                    return MEDIUM_RESIST;
                case ElementalType.Darkness:
                    return LOW_RESIST;
                case ElementalType.Fire:
                    return HIGH_RESIST;
                case ElementalType.Water:
                    return LOW_RESIST;
                default:
                    return DEFAULT_MEDIUM_RESIST;
            }
        }

        private static float GetWaterDivider(ElementalType healthElemental)
        {
            switch (healthElemental)
            {
                case ElementalType.Earth:
                    return MEDIUM_RESIST;
                case ElementalType.Air:
                    return MEDIUM_RESIST;
                case ElementalType.Darkness:
                    return LOW_RESIST;
                case ElementalType.Fire:
                    return LOW_RESIST;
                case ElementalType.Water:
                    return HIGH_RESIST;
                default:
                    return DEFAULT_MEDIUM_RESIST;
            }
        }
    }

    public enum ElementalType
    {
        Default = 0,
        Earth = 1,
        Air = 2,
        Darkness = 3,
        Fire = 4,
        Water = 5,
    }
}


