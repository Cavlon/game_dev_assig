using UnityEngine;
using System.Collections.Generic;

public class StaticData : MonoBehaviour
{
    public static GameObject nextOpponent = null;
    public static int enemyVal = 0;

    public static List<CardData> deck = new List<CardData>();

    public static Dictionary<CardData, int> deckDict = new Dictionary<CardData, int>();
    public static Dictionary<CardData, int> inventory = new Dictionary<CardData, int>();

    public static int bossesBeat = 0;
    public static int credits = 0;

    public static bool battleWon = false;
    public static bool firstLoad = true;
    public static bool shopIntroduced = false;

    public static Vector3 playerPos = new Vector3(0, 1.07f, 0);

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
