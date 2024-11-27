using UnityEngine;

[CreateAssetMenu(fileName = "New Special Card", menuName = "Card/SpecialCard")]
public class SpecialCardData : CardData
{
    public string specialKey;
    public int[] abilityArgs;
    public string overrideDamageString;
}
