using UnityEngine;

public class OperationCard : CardManager
{
    private int operand;
    private OperationCardData.Operation operation;
    private new OperationCardData cardData;

    public override void Init(int newId, CardData newCardData) {
        base.Init(newId, newCardData);
        cardData = newCardData as OperationCardData;
        operand = cardData.operand;
        operation = cardData.operation;
    }
}
