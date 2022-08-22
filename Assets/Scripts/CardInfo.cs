using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Client;
using UnityEngine.UI;

public class CardInfo : MonoBehaviour
{
    public Sprite sprite;
    public int unitID;
    public string NameUnit;
    public float Damage;
    public float Health;
    public float MoveSpeed;
    public GameObject[] Prefabs;
    public ElementalType Elemental;

    [SerializeField] private GameObject nameCard;
    [SerializeField] private GameObject unitImage;
    [SerializeField] private GameObject healthAmount;
    [SerializeField] private GameObject damageAmount;
    [SerializeField] private GameObject cost;
    [SerializeField] private GameObject elementalType;

    public void UpdateCardInfo()
    {
        nameCard.GetComponentInChildren<Text>().text = NameUnit;
        unitImage.GetComponentInChildren<Image>().sprite = sprite;
    }
}
