using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class UpdateCard : MonoBehaviour
{

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

    public void InitValues(CardData cardData) {
        healthText.text = cardData.health.ToString();
        
        if (cardData is SpecialCardData specCard && specCard.overrideDamageString != "") {
            damageText.text = specCard.overrideDamageString;
        } else {
            damageText.text = cardData.damage.ToString();
        }
        
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

    public void UpdateHealth(int health) {
        healthText.text = health.ToString();
    }
}
