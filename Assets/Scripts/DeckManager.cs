using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [SerializeField]
    private List<CardData> deckCards = new List<CardData>();

    public void DrawCard(HandManager handManager) {
        if (deckCards.Count == 0) return;

        handManager.AddCard(deckCards[0]);
        deckCards.RemoveAt(0);
    }
}
