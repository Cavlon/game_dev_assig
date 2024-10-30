using UnityEngine;

[CreateAssetMenu(fileName = "New Operation Card", menuName = "Card/OperationCard")]
public class OperationCardData : CardData
{
    public Operation operation;
    public int operand;

    public enum Operation {
        Add,
        Multiply,
        Exponentiate
    }
}
