using UnityEngine;
using System.Collections.Generic;
using System;
using CavlonUtils;
using System.Collections;

public class HandManager : MonoBehaviour
{

    [SerializeField]
    private GameObject cardPrefab;

    [SerializeField]
    private Transform handTrans;
    [SerializeField]
    private SlotManager slotManager;

    private const float spread = 6f;
    private const float spacing = 90f;
    private const float vertOffset = 20f;
    private const int hoverDist = 300;

    private List<GameObject> cards = new List<GameObject>();
    private int hoverInd = -1;
    private int cardCount = 0;
    private int cardId = 0;
    private int selectedInd = -1;
    public int slotInd = -1;
    private Vector2 initHandPos;
    private Vector2 deckPos;

    private IEnumerator handAnimEnumerator;

    void Start() {
        initHandPos = handTrans.localPosition;
        deckPos = transform.parent.Find("Deck").position;
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedInd ==-1) {
            CheckClosestCard();
        } else {
            if (slotInd != -1 && slotManager.PlayCard(cards[selectedInd], slotInd)) {
                cards[selectedInd].GetComponent<CardManager>().OnClick = null;
                cards.RemoveAt(selectedInd);
                cardCount--;

                AnimateHand(AnimUtils.TweenPos(handTrans, initHandPos, 0.2f, AnimUtils.CubicOut));

                selectedInd = -1;
                UpdateHand();
            }
        }
    }

    public void AddCard(CardData cardData) {
        GameObject newCard = Instantiate(cardPrefab, handTrans.position, Quaternion.identity, handTrans);
        cards.Add(newCard);
        newCard.name = "Card " + cardId;
        cardCount++;

        newCard.GetComponent<UpdateCard>().cardData = cardData;

        CardManager cardManager;

        if (cardData is NumberCardData) {
            cardManager = newCard.AddComponent<NumberCard>();
        } else if (cardData is OperationCardData) {
            cardManager = newCard.AddComponent<OperationCard>();
        } else {
            cardManager = newCard.AddComponent<SpecialCard>();
        }
        newCard.transform.position = deckPos;

        cardManager.OnClick = CardSelected;
        cardManager.Init(cardId, cardData);
        cardId++;

        UpdateHand();
    }

    private void UpdateHand()
    {
        float midpoint = (cardCount - 1) / 2f;

        for (int i = 0; i < cardCount; i++) {
            float centralDist = midpoint - i;
            cards[i].transform.localRotation = Quaternion.Euler(0, 0, spread * centralDist);

            AnimateCard(cards[i], AnimUtils.TweenPos(cards[i].transform, new Vector2(centralDist * -spacing, Math.Abs(centralDist) * -vertOffset), 0.25f, AnimUtils.CubicOut));
        }
    }

    private void UpdateHoverIndex(int index) {
        if (index == hoverInd) return;
        HoverCard(index);
        hoverInd = index;
    }

    private void CheckClosestCard() {

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(handTrans.parent as RectTransform, new Vector2(Input.mousePosition.x, Input.mousePosition.y), null, out pos);

        if (Vector2.Distance(pos, handTrans.localPosition) < hoverDist + (cardCount * 20f)) {
            int prevDist = 1000;
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

    private void HoverCard(int index) {
        if (hoverInd != -1 && hoverInd < cardCount) {
            cards[hoverInd].transform.SetSiblingIndex(hoverInd);

            AnimateCardImage(cards[hoverInd], AnimUtils.TweenPos(cards[hoverInd].transform.GetChild(0), new Vector2(0, 0), 0.1f, AnimUtils.CubicOut));
        }

        if (index != -1) {
            cards[index].transform.SetAsLastSibling();

            AnimateCardImage(cards[index], AnimUtils.TweenPos(cards[index].transform.GetChild(0), new Vector2(0, 0.8f), 0.1f, AnimUtils.CubicOut));
        }
    }

    public void CardSelected(int id) {
        int ind = -1;
        for (int i = 0; i < cardCount; i++) {
            if (cards[i].GetComponent<CardManager>().id == id) {
                ind = i;
                break;
            }
        }
        if (selectedInd == ind) {
            cards[selectedInd].transform.SetSiblingIndex(selectedInd);

            AnimateCardImage(cards[selectedInd], AnimUtils.TweenPos(cards[ind].transform.GetChild(0), new Vector2(0, 0), 0.2f, AnimUtils.CubicOut));
            AnimateHand(AnimUtils.TweenPos(handTrans, initHandPos, 0.2f, AnimUtils.CubicOut));

            selectedInd = -1;
            return;
        }
        if (selectedInd != -1) return;
        selectedInd = ind;
        cards[ind].transform.SetAsLastSibling();

        AnimateCardImage(cards[ind], AnimUtils.TweenPos(cards[ind].transform.GetChild(0), new Vector2(0, 2.1f), 0.2f, AnimUtils.CubicOut));
        AnimateHand(AnimUtils.TweenPos(handTrans, new Vector2(initHandPos.x, initHandPos.y - 140f), 0.2f, AnimUtils.CubicOut));

        slotInd = -1;
    }

    private void AnimateCardImage(GameObject card, IEnumerator animation) {
        CardManager cardManager = card.GetComponent<CardManager>();
        if (cardManager.animImageEnumerator != null) {
            StopCoroutine(cardManager.animImageEnumerator);
        }
        cardManager.animImageEnumerator = animation;
        StartCoroutine(cardManager.animImageEnumerator);
    }

    private void AnimateCard(GameObject card, IEnumerator animation) {
        CardManager cardManager = card.GetComponent<CardManager>();
        if (cardManager.animEnumerator != null) {
            StopCoroutine(cardManager.animEnumerator);
        }
        cardManager.animEnumerator = animation;
        StartCoroutine(cardManager.animEnumerator);
    }

    private void AnimateHand(IEnumerator animation) {
        if (handAnimEnumerator != null) {
            StopCoroutine(handAnimEnumerator);
        }
        handAnimEnumerator = animation;
        StartCoroutine(handAnimEnumerator);
    }
}
