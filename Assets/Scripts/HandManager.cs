using UnityEngine;
using System.Collections.Generic;
using System;
using CavlonUtils;
using System.Collections;
using Unity.Mathematics;

public class HandManager : MonoBehaviour
{

    [SerializeField]
    private GameObject cardPrefab;

    [SerializeField]
    private Transform handTrans;
    [SerializeField]
    private SlotManager slotManager;
    [SerializeField]
    private GameManager gameManager;

    private const float spread = 6f;
    private const float spacing = 90f;
    private const float vertOffset = 20f;

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
            if (slotInd != -1) {
                CardManager cardManager = cards[selectedInd].GetComponent<CardManager>();
                if (cardManager.cardData.costType == CardData.CostType.Bytes) {
                    if (gameManager.bytes >= cardManager.cardData.cost) {
                        gameManager.UpdateBytes(gameManager.bytes - cardManager.cardData.cost);
                        TryPlayCard();
                    } else {
                        if (slotManager.CheckSlot(cardManager, slotInd)) {
                            StartCoroutine(gameManager.ShakeBytes());
                        }
                    }
                    
                } else if (gameManager.variables == cardManager.cardData.cost) {
                    TryPlayCard();
                } else {
                    if (slotManager.CheckSlot(cardManager, slotInd)) {
                        AnimateEntity(gameManager.Shake(cards[selectedInd].transform.GetChild(0), cardManager.animImageRotEnumerator, 2, spread * (((cardCount - 1) / 2f) - selectedInd)), ref cardManager.animImageRotEnumerator);
                    }
                }
                slotInd = -1;
            }
        }
    }

    private void TryPlayCard() {
        if (slotManager.CheckSlot(cards[selectedInd].GetComponent<CardManager>(), slotInd)) {
            slotManager.PlayCard(cards[selectedInd], slotInd);
            cards.RemoveAt(selectedInd);
            cardCount--;

            AnimateEntity(AnimUtils.TweenPos(handTrans, initHandPos, 0.2f, AnimUtils.CubicOut), ref handAnimEnumerator);

            selectedInd = -1;
            UpdateHand();
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

            AnimateEntity(AnimUtils.TweenPos(cards[i].transform, new Vector2(centralDist * -spacing, Math.Abs(centralDist) * -vertOffset), 0.25f, AnimUtils.CubicOut), ref cards[i].GetComponent<CardManager>().animEnumerator);
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

        if (Math.Abs(initHandPos.x-pos.x) < 150 + (cardCount-1) * 65 && pos.y < initHandPos.y + 240) {
            // Debug.Log(Math.Abs(initHandPos.x-pos.x));

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

    private void HoverCard(int index) {
        if (hoverInd != -1 && hoverInd < cardCount) {
            cards[hoverInd].transform.SetSiblingIndex(hoverInd);

            AnimateEntity(AnimUtils.TweenPos(cards[hoverInd].transform.GetChild(0), new Vector2(0, 0), 0.1f, AnimUtils.CubicOut), ref cards[hoverInd].GetComponent<CardManager>().animImagePosEnumerator);
        }

        if (index != -1) {
            cards[index].transform.SetAsLastSibling();

            float centralDist = (cardCount - 1) / 2f - index;
            AnimateEntity(AnimUtils.TweenPos(cards[index].transform.GetChild(0), new Vector2(0, 60f + Math.Abs(centralDist) * vertOffset), 0.1f, AnimUtils.CubicOut), ref cards[index].GetComponent<CardManager>().animImagePosEnumerator);
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

            AnimateEntity(AnimUtils.TweenPos(cards[ind].transform.GetChild(0), new Vector2(0, 0), 0.2f, AnimUtils.CubicOut), ref cards[ind].GetComponent<CardManager>().animImagePosEnumerator);
            AnimateEntity(AnimUtils.TweenPos(handTrans, initHandPos, 0.2f, AnimUtils.CubicOut), ref handAnimEnumerator);

            gameManager.varSearching = false;
            gameManager.requiredVars = 0;
            gameManager.variables = 0;
            slotManager.ResetSelection();

            selectedInd = -1;
            return;
        }
        if (selectedInd != -1) return;
        selectedInd = ind;
        cards[ind].transform.SetAsLastSibling();

        if (cards[ind].GetComponent<CardManager>().cardData.costType == CardData.CostType.Variables) {
            gameManager.varSearching = true;
            gameManager.requiredVars = cards[ind].GetComponent<CardManager>().cardData.cost;
        }

        AnimateEntity(AnimUtils.TweenPos(cards[ind].transform.GetChild(0), new Vector2(0, 100f), 0.2f, AnimUtils.CubicOut), ref cards[ind].GetComponent<CardManager>().animImagePosEnumerator);
        AnimateEntity(AnimUtils.TweenPos(handTrans, new Vector2(initHandPos.x, initHandPos.y - 140f), 0.2f, AnimUtils.CubicOut), ref handAnimEnumerator);

        slotInd = -1;
    }

    private void AnimateEntity(IEnumerator animation, ref IEnumerator enumerator) {
        if (enumerator != null) {
            StopCoroutine(enumerator);
        }
        enumerator = animation;
        StartCoroutine(enumerator);
    }
}
