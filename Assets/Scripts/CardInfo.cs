using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Client;
using UnityEngine.UI;

public class CardInfo : MonoBehaviour
{
    [HideInInspector] public Sprite sprite;
    [HideInInspector] public int unitID;
    [HideInInspector] public string NameUnit;
    [HideInInspector] public float Damage;
    [HideInInspector] public float Health;
    [HideInInspector] public float MoveSpeed;
    [HideInInspector] public GameObject[] Prefabs;
    [HideInInspector] public ElementalType Elemental;

    [SerializeField] private GameObject nameCard;
    [SerializeField] private GameObject unitImage;
    [SerializeField] private GameObject healthAmount;
    [SerializeField] private GameObject damageAmount;
    [SerializeField] private GameObject cost;
    [SerializeField] private GameObject elementalType;

    public void UpdateCardInfo()
    {
        nameCard.GetComponentInChildren<Text>().text = NameUnit;
    }
}
