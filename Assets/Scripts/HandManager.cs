using UnityEngine;
using System.Collections.Generic;
using System;
using CavlonUtils;
using System.Collections;
using Unity.Mathematics;

public class HandManager : MonoBehaviour
{

    [SerializeField]
    private GameObject cardPrefab;      // Prefab of the generic card object which is then initialised

    [SerializeField]
    private Transform handTrans;
    [SerializeField]
    private SlotManager slotManager;
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private SoundManager soundManager;

    // Parameters for card fanning in the hand
    private const float spread = 6f;
    private const float spacing = 90f;
    private const float vertOffset = 20f;

    // The cards in the player's hand
    private List<GameObject> cards = new List<GameObject>();
    public int selectedInd = -1;
    public int slotInd = -1;
    private int hoverInd = -1;

    public int cardCount = 0;
    private int cardId = 0;     // Used to distinguish specific cards and identify them
    private Vector2 initHandPos;    // Initial hand position for animation
    private Vector2 deckPos;    // Deck position for card drawing

    private IEnumerator handAnimEnumerator;     // Used for interrupting hand animations

    void Start() {
        initHandPos = handTrans.localPosition;
        deckPos = transform.parent.Find("Deck").position;
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedInd == -1) {     // If no card is selected then perform card hover checks
            CheckClosestCard();
        } else {
            if (slotInd != -1) {    // Check if a slot has been selected
                CardManager cardManager = cards[selectedInd].GetComponent<CardManager>();
                if (cardManager.cardData.costType == CardData.CostType.Bytes) {     // Check if the player has enough bytes to play the card
                    if (gameManager.bytes >= cardManager.cardData.cost) {
                        gameManager.UpdateBytes(gameManager.bytes - cardManager.cardData.cost);
                        TryPlayCard();
                    } else {
                        if (slotManager.CheckSlot(cardManager, slotInd)) {
                            soundManager.PlaySound(8);
                            StartCoroutine(gameManager.ShakeBytes());
                            AnimateEntity(gameManager.Shake(cards[selectedInd].transform.GetChild(0), cardManager.animImageRotEnumerator, 2, spread * (((cardCount - 1) / 2f) - selectedInd)), ref cardManager.animImageRotEnumerator);
                        }
                    }
                    
                } else if (gameManager.variables == cardManager.cardData.cost) {    // Check if the player has enough variables to play the card
                    TryPlayCard();
                } else {
                    if (slotManager.CheckSlot(cardManager, slotInd)) {
                        soundManager.PlaySound(8);
                        AnimateEntity(gameManager.Shake(cards[selectedInd].transform.GetChild(0), cardManager.animImageRotEnumerator, 2, spread * (((cardCount - 1) / 2f) - selectedInd)), ref cardManager.animImageRotEnumerator);
                    }
                }
                slotInd = -1;
            }
        }
    }

    private void TryPlayCard() {    // Attempt to play the selected card in the chosen slot
        if (slotManager.CheckSlot(cards[selectedInd].GetComponent<CardManager>(), slotInd)) {   // Play the card if the slot is free
            slotManager.PlayerPlayCard(cards[selectedInd], slotInd);
            cards.RemoveAt(selectedInd);
            cardCount--;

            AnimateEntity(AnimUtils.TweenPos(handTrans, initHandPos, 0.2f, AnimUtils.CubicOut), ref handAnimEnumerator);

            selectedInd = -1;
            UpdateHand();
        }
    }

    public void AddCard(CardData cardData) {    // Draw a card from the deck
        GameObject newCard = Instantiate(cardPrefab, deckPos, Quaternion.identity, handTrans);  // Create a new card
        cards.Add(newCard);
        newCard.name = "Card " + cardId;
        cardCount++;

        newCard.GetComponent<UpdateCard>().InitValues(cardData);    // Update the visuals of the card with the default values

        CardManager cardManager;

        if (cardData is NumberCardData) {   // Determine what type the card is
            cardManager = newCard.AddComponent<NumberCard>();
        } else if (cardData is OperationCardData) {
            cardManager = newCard.AddComponent<OperationCard>();
        } else {
            cardManager = newCard.AddComponent<SpecialCard>();
        }

        // Initialise the card and its callback
        cardManager.OnClick = CardSelected;
        cardManager.Init(cardId, cardData);
        cardManager.AddClickFunction();
        cardId++;

        // Update the hand fanning
        UpdateHand();
    }

    private void UpdateHand()   // Calculates the hand fanning
    {
        float midpoint = (cardCount - 1) / 2f;

        for (int i = 0; i < cardCount; i++) {
            float centralDist = midpoint - i;
            cards[i].transform.localRotation = Quaternion.Euler(0, 0, spread * centralDist);

            AnimateEntity(AnimUtils.TweenPos(cards[i].transform, new Vector2(centralDist * -spacing, Math.Abs(centralDist) * -vertOffset), 0.25f, AnimUtils.CubicOut), ref cards[i].GetComponent<CardManager>().animEnumerator);
        }
    }

    private void UpdateHoverIndex(int index) {      // Manage what card is currently being hovered over
        if (index == hoverInd) return;
        HoverCard(index);
        hoverInd = index;
    }

    private void CheckClosestCard() {   // Checks which card the mouse is currently closest to

        // Get the mouse coordinates in local canvas coordinates
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(handTrans.parent as RectTransform, new Vector2(Input.mousePosition.x, Input.mousePosition.y), null, out pos);

        // Only check for hovering if the mouse is close enough to the cards
        if (Math.Abs(initHandPos.x-pos.x) < 150 + (cardCount-1) * 65 && pos.y < initHandPos.y + 240) {

            // Iterate through each card and find which is closest to the mouse
            // Set the closest index to be the one hovered over
            int prevDist = int.MaxValue;
            for (int i = 0; i < cards.Count; i++) {
                int dist = (int)Vector2.Distance(pos, handTrans.localPosition + cards[i].transform.localPosition);
                if (dist <= prevDist) {
                    prevDist = dist;
                } else {
                    UpdateHoverIndex(i-1);
                    return;
                }
            }
            UpdateHoverIndex(cardCount-1);
        } else {
            UpdateHoverIndex(-1);
        }
    }

    private void HoverCard(int index) {     // Animate card hovering
        if (hoverInd != -1 && hoverInd < cardCount) {   // Lower the previously hovered card
            cards[hoverInd].transform.SetSiblingIndex(hoverInd);

            AnimateEntity(AnimUtils.TweenPos(cards[hoverInd].transform.GetChild(0), new Vector2(0, 0), 0.1f, AnimUtils.CubicOut), ref cards[hoverInd].GetComponent<CardManager>().animImagePosEnumerator);
        }

        if (index != -1) {      // Raise the newly hovered card
            cards[index].transform.SetAsLastSibling();

            float centralDist = (cardCount - 1) / 2f - index;
            soundManager.PlaySound(5);
            AnimateEntity(AnimUtils.TweenPos(cards[index].transform.GetChild(0), new Vector2(0, 60f + Math.Abs(centralDist) * vertOffset), 0.1f, AnimUtils.CubicOut), ref cards[index].GetComponent<CardManager>().animImagePosEnumerator);
        }
    }

    public void ResetSelection() {      // Forcefully reset the currently selected card
        if (selectedInd == -1) return;
        cards[selectedInd].transform.SetSiblingIndex(selectedInd);

        AnimateEntity(AnimUtils.TweenPos(cards[selectedInd].transform.GetChild(0), new Vector2(0, 0), 0.2f, AnimUtils.CubicOut), ref cards[selectedInd].GetComponent<CardManager>().animImagePosEnumerator);
        RaiseHand();

        gameManager.varSearching = false;
        gameManager.requiredVars = 0;
        gameManager.variables = 0;
        slotManager.ResetSelection();
    }

    public void CardSelected(int id) {      // Select a card when it is clicked

        // Identify which card was clicked from its ID
        int ind = -1;
        for (int i = 0; i < cardCount; i++) {
            if (cards[i].GetComponent<CardManager>().id == id) {
                ind = i;
                break;
            }
        }

        // If the previously selected card was clicked again, lower and unselect it
        if (selectedInd == ind) {
            ResetSelection();
            selectedInd = -1;
            soundManager.PlaySound(0);
            return;
        }

        if (selectedInd != -1) return;  // If a card has already been selected but a different card is clicked, ignore the click

        // Raise the selected card
        selectedInd = ind;
        cards[ind].transform.SetAsLastSibling();

        // Enter variable searching mode if the card selected requires variables
        if (cards[ind].GetComponent<CardManager>().cardData.costType == CardData.CostType.Variables) {
            gameManager.varSearching = true;
            gameManager.requiredVars = cards[ind].GetComponent<CardManager>().cardData.cost;
        }

        soundManager.PlaySound(0);

        AnimateEntity(AnimUtils.TweenPos(cards[ind].transform.GetChild(0), new Vector2(0, 100f), 0.2f, AnimUtils.CubicOut), ref cards[ind].GetComponent<CardManager>().animImagePosEnumerator);
        LowerHand();

        // Start searching for a slot
        slotInd = -1;
    }

    public void LowerHand() {
        AnimateEntity(AnimUtils.TweenPos(handTrans, new Vector2(initHandPos.x, initHandPos.y - 140f), 0.2f, AnimUtils.CubicOut), ref handAnimEnumerator);
    }

    public void RaiseHand() {
        AnimateEntity(AnimUtils.TweenPos(handTrans, initHandPos, 0.2f, AnimUtils.CubicOut), ref handAnimEnumerator);
    }

    // Interrupts the currently playing animation and starts the new one
    private void AnimateEntity(IEnumerator animation, ref IEnumerator enumerator) {
        if (enumerator != null) {
            StopCoroutine(enumerator);
        }
        enumerator = animation;
        StartCoroutine(enumerator);
    }
}
