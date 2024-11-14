using UnityEngine;

public class NumberCard : CardManager
{

    public ulong value;
    private new NumberCardData cardData;

    public override void Init(int newId, CardData newCardData) {
        base.Init(newId, newCardData);
        cardData = newCardData as NumberCardData;
        value = (ulong)cardData.value;
    }
}
