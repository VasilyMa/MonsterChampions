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
    public int Cost;

    [SerializeField] private GameObject nameCard;
    [SerializeField] private GameObject unitImage;
    [SerializeField] private GameObject healthAmount;
    [SerializeField] private GameObject damageAmount;
    [SerializeField] private GameObject cost;
    [SerializeField] private GameObject[] elementalType;

    public void UpdateCardInfo()
    {
        nameCard.GetComponentInChildren<Text>().text = NameUnit;
        //unitImage.GetComponentInChildren<Image>().sprite = sprite;
        healthAmount.GetComponentInChildren<Text>().text = Health.ToString();
        damageAmount.GetComponentInChildren<Text>().text = Damage.ToString();
        cost.GetComponentInChildren<Text>().text = Cost.ToString();
        switch (Elemental)
        {
            case ElementalType.Default:
                break;
            case ElementalType.Earth:
                elementalType[0].gameObject.SetActive(true);
                elementalType[1].gameObject.SetActive(false);
                elementalType[2].gameObject.SetActive(false);
                elementalType[3].gameObject.SetActive(false);
                elementalType[4].gameObject.SetActive(false);
                break;
            case ElementalType.Air:
                elementalType[0].gameObject.SetActive(false);
                elementalType[1].gameObject.SetActive(true);
                elementalType[2].gameObject.SetActive(false);
                elementalType[3].gameObject.SetActive(false);
                elementalType[4].gameObject.SetActive(false);
                break;
            case ElementalType.Darkness:
                elementalType[0].gameObject.SetActive(false);
                elementalType[1].gameObject.SetActive(false);
                elementalType[2].gameObject.SetActive(true);
                elementalType[3].gameObject.SetActive(false);
                elementalType[4].gameObject.SetActive(false);
                break;
            case ElementalType.Fire:
                elementalType[0].gameObject.SetActive(false);
                elementalType[1].gameObject.SetActive(false);
                elementalType[2].gameObject.SetActive(false);
                elementalType[3].gameObject.SetActive(true);
                elementalType[4].gameObject.SetActive(false);
                break;
            case ElementalType.Water:
                elementalType[0].gameObject.SetActive(false);
                elementalType[1].gameObject.SetActive(false);
                elementalType[2].gameObject.SetActive(false);
                elementalType[3].gameObject.SetActive(false);
                elementalType[4].gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }
}
