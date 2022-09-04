using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Client;

[CreateAssetMenu(fileName = "EffectsPool", menuName = "Pools/EffectsPool", order = 0)]
public class EffectsPool : ScriptableObject
{
    public ElementalEffects ElementalEffects;

    public MonstersEffects MonstersEffects;

    public OtherEffects OtherEffects;
}

[System.Serializable]
public class ElementalEffects
{
    [SerializeField] private GameObject DefaultElemental;
    [SerializeField] private GameObject EarthElemental;
    [SerializeField] private GameObject AirElemental;
    [SerializeField] private GameObject DarknessElemental;
    [SerializeField] private GameObject FireElemental;
    [SerializeField] private GameObject WaterElemental;

    public GameObject GetElementalEffect(ElementalType elementalType)
    {
        switch (elementalType)
        {
            case ElementalType.Default:
                return DefaultElemental;
            case ElementalType.Earth:
                return EarthElemental;
            case ElementalType.Air:
                return AirElemental;
            case ElementalType.Darkness:
                return DarknessElemental;
            case ElementalType.Fire:
                return FireElemental;
            case ElementalType.Water:
                return WaterElemental;
            default:
                return DefaultElemental;
        }
    }
}

[System.Serializable]
public class MonstersEffects
{
    public GameObject SparkyExplosion;
    public GameObject TinkiThunderbolt;
    public GameObject SlevDebuff;
    public GameObject BableProtectionBuff;
}

[System.Serializable]
public class OtherEffects
{
    public GameObject DroppingGold;
    public GameObject GoldShower;
}