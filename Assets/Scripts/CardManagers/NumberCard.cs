using UnityEngine;

public class NumberCard : CardManager
{

    private int value;
    private NumberCardData cardData;

    public override void Init(int newId, CardData newCardData) {
        base.Init(newId, newCardData);
        cardData = newCardData as NumberCardData;
        value = cardData.value;
    }
}
