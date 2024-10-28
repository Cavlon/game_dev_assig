using System.Collections.Generic;
using Global;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    [SerializeField]
    private List<Card> deckCards = new List<Card>();

    public void DrawCard(HandManager handManager) {
        if (deckCards.Count == 0) return;

        handManager.AddCard(deckCards[0]);
        deckCards.RemoveAt(0);
    }
}
