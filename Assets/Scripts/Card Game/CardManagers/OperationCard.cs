using UnityEngine;

public class OperationCard : CardManager
{
    public string equation;
    private new OperationCardData cardData;

    public override void Init(int newId, CardData newCardData) {
        base.Init(newId, newCardData);
        cardData = newCardData as OperationCardData;
        equation = cardData.equation;
    }
}
