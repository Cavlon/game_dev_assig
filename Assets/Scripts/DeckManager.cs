using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CavlonUtils;

public class DeckManager : MonoBehaviour
{
    [SerializeField]
    private List<CardData> deckCards = new List<CardData>();
    [SerializeField]
    private Transform deck;

    private IEnumerator animEnumerator;

    public void DrawCard(HandManager handManager) {
        if (deckCards.Count == 0) return;

        handManager.AddCard(deckCards[0]);
        deckCards.RemoveAt(0);
    }

    public void OnHover() {
        Debug.Log("Deck Hover");
        if (animEnumerator != null) {
            StopCoroutine(animEnumerator);
        }
        animEnumerator = AnimUtils.TweenScale(deck, new Vector3(1.1f, 1.1f, 1f), 0.2f, AnimUtils.CubicOut);
        StartCoroutine(animEnumerator);
    }

    public void OnUnHover() {
        Debug.Log("Deck Unhover");
        if (animEnumerator != null) {
            StopCoroutine(animEnumerator);
        }
        animEnumerator = AnimUtils.TweenScale(deck, new Vector3(1f, 1f, 1f), 0.2f, AnimUtils.CubicOut);
        StartCoroutine(animEnumerator);
    }
}
