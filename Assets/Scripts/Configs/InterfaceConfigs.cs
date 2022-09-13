using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[CreateAssetMenu(fileName = "InterfaceConfig", menuName = "Pools/InterfacePools")]
public class InterfaceConfigs : ScriptableObject
{
    public Sprite[] elementsShirt;
    public Sprite[] collectionBackground;

    public Sprite[] BiomsSprites;
    public BiomType[] StartBiomTypes;
    public int[] StartBiomLevels;
    public List<Biom> Bioms;
    public Color CurrentPointColor;
    public Color CompletePointColor;
    public Color AnCompletePointColor;
}
public class Biom
{
    public List<int> BiomLevels; 
    public int StartBiomLevel; 
    public BiomType BiomType; 
    public Sprite BiomSprite; 
    public Sprite NextBiomSprite;
    //public Sprite BlurSprite; // заблюренная картинка для магазина
}

public enum BiomType
{
    Forest = 1, Desert = 2, DarkForest = 3
}
