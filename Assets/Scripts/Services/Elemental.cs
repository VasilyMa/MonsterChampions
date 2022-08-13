using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public static class Elemental
    {
        private static float _highResist = 2;
        private static float _mediumResist = 1;
        private static float _lowResist = 0.5f;
        private static float _defaultZero = 0;

        public static float GetDamageDivider(ElementalType damageElement, ElementalType healthElement)
        {
            switch (damageElement)
            {
                case ElementalType.Earth:
                    return GetEarthDivider(healthElement);
                case ElementalType.Air:
                    return GetAirDivider(healthElement);
                case ElementalType.Darkness:
                    return GetDarknessDivider(healthElement);
                case ElementalType.Fire:
                    return GetFireDivider(healthElement);
                case ElementalType.Water:
                    return GetWaterDivider(healthElement);
                default:
                    return _defaultZero;
            }
        }

        private static float GetEarthDivider(ElementalType healthElement)
        {
            switch (healthElement)
            {
                case ElementalType.Earth:
                    return _highResist;
                case ElementalType.Air:
                    return _lowResist;
                case ElementalType.Darkness:
                    return _lowResist;
                case ElementalType.Fire:
                    return _highResist;
                case ElementalType.Water:
                    return _mediumResist;
                default:
                    return _defaultZero;
            }
        }

        private static float GetAirDivider(ElementalType healthElement)
        {
            switch (healthElement)
            {
                case ElementalType.Earth:
                    return _highResist;
                case ElementalType.Air:
                    return _highResist;
                case ElementalType.Darkness:
                    return _lowResist;
                case ElementalType.Fire:
                    return _mediumResist;
                case ElementalType.Water:
                    return _lowResist;
                default:
                    return _defaultZero;
            }
        }

        private static float GetDarknessDivider(ElementalType healthElement)
        {
            switch (healthElement)
            {
                case ElementalType.Earth:
                    return _lowResist;
                case ElementalType.Air:
                    return _lowResist;
                case ElementalType.Darkness:
                    return _highResist;
                case ElementalType.Fire:
                    return _lowResist;
                case ElementalType.Water:
                    return _lowResist;
                default:
                    return _defaultZero;
            }
        }

        private static float GetFireDivider(ElementalType healthElement)
        {
            switch (healthElement)
            {
                case ElementalType.Earth:
                    return _lowResist;
                case ElementalType.Air:
                    return _mediumResist;
                case ElementalType.Darkness:
                    return _lowResist;
                case ElementalType.Fire:
                    return _highResist;
                case ElementalType.Water:
                    return _highResist;
                default:
                    return _defaultZero;
            }
        }

        private static float GetWaterDivider(ElementalType healthElement)
        {
            switch (healthElement)
            {
                case ElementalType.Earth:
                    return _mediumResist;
                case ElementalType.Air:
                    return _highResist;
                case ElementalType.Darkness:
                    return _lowResist;
                case ElementalType.Fire:
                    return _lowResist;
                case ElementalType.Water:
                    return _highResist;
                default:
                    return _defaultZero;
            }
        }
    }

    public enum ElementalType
    {
        Earth = 1,
        Air = 2,
        Darkness = 3,
        Fire = 4,
        Water = 5,
    }
}


