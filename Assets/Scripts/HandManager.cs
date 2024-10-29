using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using UnityEngine.SocialPlatforms;

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
                handTrans.localPosition = new Vector2(handTrans.localPosition.x, handTrans.localPosition.y + 140f);
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

            cards[i].transform.localPosition = new Vector3(centralDist * -spacing, Math.Abs(centralDist) * -vertOffset, 0);
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
            int dist = 0;
            for (int i = 0; i < cards.Count; i++) {
                dist = (int)Vector2.Distance(pos, handTrans.localPosition + cards[i].transform.localPosition);
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
            cards[hoverInd].transform.GetChild(0).localPosition = new Vector2(0, 0);
        }

        if (index != -1) {
            cards[index].transform.SetAsLastSibling();
            cards[index].transform.GetChild(0).localPosition = new Vector2(0, 0.8f);
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
            cards[selectedInd].transform.GetChild(0).localPosition = new Vector2(0, 0);
            handTrans.localPosition = new Vector2(handTrans.localPosition.x, handTrans.localPosition.y + 140f);
            selectedInd = -1;
            return;
        }
        if (selectedInd != -1) return;
        selectedInd = ind;
        cards[ind].transform.SetAsLastSibling();
        cards[ind].transform.GetChild(0).localPosition = new Vector2(0, 2f);
        handTrans.localPosition = new Vector2(handTrans.localPosition.x, handTrans.localPosition.y - 140f);
        slotInd = -1;
    }
}
