using UnityEngine;

public class CardData : ScriptableObject
{
    public string faceValue;
    public CardPack cardPack;
    public CostType costType;
    public int health;
    public int damage;
    public int cost;

    public enum CardPack 
    {
        Arithmetic,
        Calculus,
        LinearAlgebra,
        SmallNumbers,
        LargeNumbers
    }

    public enum CostType 
    {
        Variables,
        Bytes
    }

}
