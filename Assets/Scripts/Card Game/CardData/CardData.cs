using UnityEngine;

public class CardData : ScriptableObject
{
    public string faceValue;
    public CostType costType;
    public int health;
    public int damage;
    public int cost;

    public enum CostType 
    {
        Variables,
        Bytes
    }

}
