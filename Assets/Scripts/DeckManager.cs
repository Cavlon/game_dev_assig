using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CavlonUtils;
using System.Data.Common;

public class DeckManager : MonoBehaviour
{
    [SerializeField]
    private List<CardData> deckCards = new List<CardData>();
    [SerializeField]
    private Transform deck;
    [SerializeField]
    private Sprite[] deckIcons = new Sprite[3];
    [SerializeField]
    private Image deckIcon;
    [SerializeField]
    private Image deckCard;
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private HandManager handManager;

    private IEnumerator animShakeEnumerator;
    private IEnumerator animEnumerator;
    public bool canDraw = true;

    public void DrawCard(bool ignore) {
        if (deckCards.Count == 0) return;
        if (!canDraw || handManager.cardCount > 9) {
            StartCoroutine(gameManager.Shake(deck, animShakeEnumerator, 2, 0));
            return;
        }

        Debug.Log("Draw");

        handManager.AddCard(deckCards[0]);
        deckCards.RemoveAt(0);
        if (!ignore) canDraw = false;
        
        if (deckCards.Count == 0) {
            deckIcon.sprite = deckIcons[1];
            deckCard.color = new Color(deckCard.color.r, deckCard.color.g, deckCard.color.b, 50/255f);
        }
    }

    public void DeckClicked() {
        DrawCard(false);
    }

    public void OnHover() {
        Debug.Log("Deck Hover");
        if (deckCards.Count == 0) return;
        if (animEnumerator != null) {
            StopCoroutine(animEnumerator);
        }
        animEnumerator = AnimUtils.TweenScale(deck, new Vector3(1.1f, 1.1f, 1f), 0.2f, AnimUtils.CubicOut);
        StartCoroutine(animEnumerator);
    }

    public void OnUnHover() {
        Debug.Log("Deck Unhover");
        if (deckCards.Count == 0) return;
        if (animEnumerator != null) {
            StopCoroutine(animEnumerator);
        }
        animEnumerator = AnimUtils.TweenScale(deck, new Vector3(1f, 1f, 1f), 0.2f, AnimUtils.CubicOut);
        StartCoroutine(animEnumerator);
    }

    public void Shuffle() {
        deckCards = ListUtils.Shuffle<CardData>(deckCards);
    }
}
