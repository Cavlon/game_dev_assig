using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CavlonUtils;
using System.Data.Common;

public class DeckManager : MonoBehaviour
{
    // The cards currently in the deck
    [SerializeField]
    private List<CardData> deckCards = new List<CardData>();

    [SerializeField]
    private Transform deck;

    // Icons denoting the possible states of the deck
    [SerializeField]
    private Sprite[] deckIcons = new Sprite[3];

    // The deck visuals
    [SerializeField]
    private Image deckIcon;
    [SerializeField]
    private Image deckCard;

    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private HandManager handManager;

    // For deck animations
    private IEnumerator animShakeEnumerator;
    private IEnumerator animEnumerator;

    public bool canDraw = true;

    public void DrawCard(bool ignore) {     // Checks if a card can be drawn and draws it
        if (deckCards.Count == 0) return;   // Empty deck

        if (!canDraw || handManager.cardCount > 9) {    // Too many cards in the hand
            StartCoroutine(gameManager.Shake(deck, animShakeEnumerator, 2, 0));
            return;
        }

        Debug.Log("Draw");

        // Draws a card from the deck to the hand
        handManager.AddCard(deckCards[0]);
        deckCards.RemoveAt(0);

        if (!ignore) {  // Sometimes more than one card can be drawn (e.g. game start)
            canDraw = false;
            gameManager.CardDawn();
        }
        
        // Change deck states once empty
        if (deckCards.Count == 0) {
            deckIcon.sprite = deckIcons[1];
            deckCard.color = new Color(deckCard.color.r, deckCard.color.g, deckCard.color.b, 50/255f);
        }
    }

    public void DeckClicked() {     // If the deck is clicked, try to draw a card
        DrawCard(false);
    }

    public void OnHover() {     // Animates the deck when it is hovered over
        Debug.Log("Deck Hover");
        if (deckCards.Count == 0) return;
        if (animEnumerator != null) {
            StopCoroutine(animEnumerator);
        }
        animEnumerator = AnimUtils.TweenScale(deck, new Vector3(1.1f, 1.1f, 1f), 0.2f, AnimUtils.CubicOut);
        StartCoroutine(animEnumerator);
    }

    public void OnUnHover() {   // Animates the deck when it is no longer hovered over
        Debug.Log("Deck Unhover");
        if (deckCards.Count == 0) return;
        if (animEnumerator != null) {
            StopCoroutine(animEnumerator);
        }
        animEnumerator = AnimUtils.TweenScale(deck, new Vector3(1f, 1f, 1f), 0.2f, AnimUtils.CubicOut);
        StartCoroutine(animEnumerator);
    }

    public void Shuffle() {     // Randomly shuffles the deck
        deckCards = ListUtils.Shuffle<CardData>(deckCards);
    }
}
