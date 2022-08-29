using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Client;
using UnityEngine.UI;

public class CardInfo : MonoBehaviour
{
    public int UniqueID;
    public Sprite Sprite;
    public int Cost;
    public float Damage;
    public float Health;
    public float MoveSpeed;
    public GameObject[] Prefabs;
    public MonstersID.Value MonsterID;
    public ElementalType Elemental;
    public List<MonsterVisualAndAnimations> VisualAndAnimations = new List<MonsterVisualAndAnimations>();


    [SerializeField] private GameObject nameCard;
    [SerializeField] private GameObject unitImage;
    [SerializeField] private GameObject healthAmount;
    [SerializeField] private GameObject damageAmount;
    [SerializeField] private GameObject cost;
    [SerializeField] private GameObject[] elementalType;
    [SerializeField] private Image Background;

    [SerializeField] private Sprite[] sprites;

    public void UpdateCardInfo()
    {
        nameCard.GetComponentInChildren<Text>().text = MonsterID.ToString();
        unitImage.GetComponentInChildren<Image>().sprite = Sprite;
        healthAmount.GetComponentInChildren<Text>().text = Health.ToString();
        damageAmount.GetComponentInChildren<Text>().text = Damage.ToString();
        cost.GetComponentInChildren<Text>().text = Cost.ToString();
        switch (Elemental)
        {
            case ElementalType.Default:
                break;
            case ElementalType.Earth:
                Background.sprite = sprites[0];
                elementalType[0].gameObject.SetActive(true);
                elementalType[1].gameObject.SetActive(false);
                elementalType[2].gameObject.SetActive(false);
                elementalType[3].gameObject.SetActive(false);
                elementalType[4].gameObject.SetActive(false);
                break;
            case ElementalType.Air:
                Background.sprite = sprites[1];
                elementalType[0].gameObject.SetActive(false);
                elementalType[1].gameObject.SetActive(true);
                elementalType[2].gameObject.SetActive(false);
                elementalType[3].gameObject.SetActive(false);
                elementalType[4].gameObject.SetActive(false);
                break;
            case ElementalType.Darkness:
                Background.sprite = sprites[2];
                elementalType[0].gameObject.SetActive(false);
                elementalType[1].gameObject.SetActive(false);
                elementalType[2].gameObject.SetActive(true);
                elementalType[3].gameObject.SetActive(false);
                elementalType[4].gameObject.SetActive(false);
                break;
            case ElementalType.Fire:
                Background.sprite = sprites[3];
                elementalType[0].gameObject.SetActive(false);
                elementalType[1].gameObject.SetActive(false);
                elementalType[2].gameObject.SetActive(false);
                elementalType[3].gameObject.SetActive(true);
                elementalType[4].gameObject.SetActive(false);
                break;
            case ElementalType.Water:
                Background.sprite = sprites[4];
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
