using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Client;
using UnityEngine.UI;

public class CardInfo : MonoBehaviour
{
    public int LevelCard;
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
    [SerializeField] private Image Element;
    //[SerializeField] private Image Shirt;

    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Sprite[] backgroundSprite;


    public void UpdateCardInfo()
    {
        nameCard.GetComponentInChildren<Text>().text = MonsterID.ToString();
        unitImage.GetComponentInChildren<Image>().sprite = Sprite;
        healthAmount.GetComponentInChildren<Text>().text = Health.ToString();
        damageAmount.GetComponentInChildren<Text>().text = Damage.ToString();
        cost.GetComponentInChildren<Text>().text = Cost.ToString();

        //var cardShirts = spritesShirt;

        switch (Elemental)
        {
            case ElementalType.Default:
                break;
            case ElementalType.Earth:
                Background.sprite = backgroundSprite[0];
                Element.sprite = sprites[0];
                //Shirt.sprite = cardShirts[0];
                elementalType[0].gameObject.SetActive(true);
                elementalType[1].gameObject.SetActive(false);
                elementalType[2].gameObject.SetActive(false);
                elementalType[3].gameObject.SetActive(false);
                elementalType[4].gameObject.SetActive(false);
                break;
            case ElementalType.Air:
                Background.sprite = backgroundSprite[1];
                Element.sprite = sprites[1];
                //Shirt.sprite = cardShirts[1];
                elementalType[0].gameObject.SetActive(false);
                elementalType[1].gameObject.SetActive(true);
                elementalType[2].gameObject.SetActive(false);
                elementalType[3].gameObject.SetActive(false);
                elementalType[4].gameObject.SetActive(false);
                break;
            case ElementalType.Darkness:
                Background.sprite = backgroundSprite[2];
                Element.sprite = sprites[2];
                //Shirt.sprite = cardShirts[2];
                elementalType[0].gameObject.SetActive(false);
                elementalType[1].gameObject.SetActive(false);
                elementalType[2].gameObject.SetActive(true);
                elementalType[3].gameObject.SetActive(false);
                elementalType[4].gameObject.SetActive(false);
                break;
            case ElementalType.Fire:
                Background.sprite = backgroundSprite[3];
                Element.sprite = sprites[3];
                //Shirt.sprite = cardShirts[3];
                elementalType[0].gameObject.SetActive(false);
                elementalType[1].gameObject.SetActive(false);
                elementalType[2].gameObject.SetActive(false);
                elementalType[3].gameObject.SetActive(true);
                elementalType[4].gameObject.SetActive(false);
                break;
            case ElementalType.Water:
                Background.sprite = backgroundSprite[4];
                Element.sprite = sprites[4];
                //Shirt.sprite = cardShirts[4];
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
