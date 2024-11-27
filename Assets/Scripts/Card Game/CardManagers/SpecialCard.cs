using UnityEngine;

public class SpecialCard : CardManager
{
    public string specialKey;
    public int[] abilityArgs;

    private new SpecialCardData cardData;

    public override void Init(int newId, CardData newCardData) {
        base.Init(newId, newCardData);
        cardData = newCardData as SpecialCardData;
        specialKey = cardData.specialKey;
        abilityArgs = cardData.abilityArgs;
    }
}
