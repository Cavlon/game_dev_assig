using UnityEngine;
using System.Collections.Generic;

public class StaticData : MonoBehaviour
{
    public static GameObject nextOpponent = null;

    public static List<CardData> deck = new List<CardData>();

    public static Dictionary<CardData, int> deckDict = new Dictionary<CardData, int>();
    public static Dictionary<CardData, int> inventory = new Dictionary<CardData, int>();

    public static int bossesBeat = 0;

    public static int credits = 10;

    public static void AddCardToInventory(CardData card) {
        if (inventory.ContainsKey(card)) {
            inventory[card]++;
        } else {
            inventory.Add(card, 1);
        }
    }

    public static void AddCardToDeck(CardData card) {
        if (deckDict.ContainsKey(card)) {
            deckDict[card]++;
        } else {
            deckDict.Add(card, 1);
        }
    }

    public static int GetDeckCount(CardData card) {
        if (deckDict.ContainsKey(card)) {
            return deckDict[card];
        } else {
            return 0;
        }
    }
}
