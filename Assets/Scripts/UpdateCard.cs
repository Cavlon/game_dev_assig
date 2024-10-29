using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpdateCard : MonoBehaviour
{
    public CardData cardData;


    [SerializeField]
    private Image cardImage;

    [SerializeField]
    private Image costImage;

    [SerializeField]
    private TMP_Text healthText;

    [SerializeField]
    private TMP_Text damageText;

    [SerializeField]
    private TMP_Text costText;

    [SerializeField]
    private TMP_Text faceText;

    [SerializeField]
    private Sprite[] costSprites = new Sprite[2];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateValues();
    }

    public void UpdateValues() {
        healthText.text = cardData.health.ToString();
        damageText.text = cardData.damage.ToString();
        faceText.text = cardData.faceValue;
        if (cardData.cost == 0) {
            costText.gameObject.SetActive(false);
            costImage.gameObject.SetActive(false);
        } else {
            costText.text = cardData.cost.ToString();
            costImage.sprite = costSprites[(int)cardData.costType];
        }
    }

    public void UpdateFaceText(string newText) {
        faceText.text = newText;
    }
}
