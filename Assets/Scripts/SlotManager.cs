using UnityEngine;
using UnityEngine.UI;
using CavlonUtils;
using System.Collections;
using System;
using Unity.Mathematics;

public class SlotManager : MonoBehaviour
{
    [SerializeField]
    private HandManager handManager;
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private GameObject cardPrefab;

    private Transform[] playerSlots = new Transform[5];
    public Transform[] opponentSlots = new Transform[5];
    private GameObject[] playedCards = new GameObject[5];
    public GameObject[] opponentCards = new GameObject[5];
    private bool[] selectedCards = new bool[4];

    void Start() {
        Transform operationSlots = transform.parent.Find("PlayerSlots").GetChild(0);
        for (int i = 0; i < 4; i++) {
            playerSlots[i] = operationSlots.GetChild(i);
        }
        playerSlots[4] = transform.parent.Find("PlayerSlots").GetChild(1);

        Transform opponentOperationSlots = transform.parent.Find("OpponentSlots").GetChild(0);
        for (int i = 0; i < 4; i++) {
            opponentSlots[i] = opponentOperationSlots.GetChild(i);
        }
        opponentSlots[4] = transform.parent.Find("OpponentSlots").GetChild(1);
    }

    public void SlotClicked(int ind) {
        handManager.slotInd = ind;
        Debug.Log("Slot " + ind + " Clicked");
    }

    public void CardClicked(int id) {
        if (!gameManager.varSearching) return;
        int ind = 0;
        for (int i = 0; i < 4; i++) {
            if (playedCards[i] != null && playedCards[i].GetComponent<CardManager>().id == id) {
                ind = i;
                break;
            }
        }
        if (!selectedCards[ind]) {
            if (gameManager.variables == gameManager.requiredVars) return;
            selectedCards[ind] = true;
            gameManager.variables++;
            playedCards[ind].transform.GetChild(0).Find("Generalise").gameObject.SetActive(true);
            StartCoroutine(gameManager.Shake(playedCards[ind].transform, playedCards[ind].GetComponent<CardManager>().animEnumerator, 1, 0));
        } else {
            selectedCards[ind] = false;
            gameManager.variables--;
            playedCards[ind].transform.GetChild(0).Find("Generalise").gameObject.SetActive(false);
            StartCoroutine(gameManager.Shake(playedCards[ind].transform, playedCards[ind].GetComponent<CardManager>().animEnumerator, 1, 0));
        }
    }

    public void ResetSelection() {
        for (int i = 0; i < 4; i++) {
            selectedCards[i] = false;
            if (playedCards[i] != null) playedCards[i].transform.GetChild(0).Find("Generalise").gameObject.SetActive(false);
        }
    }

    public bool CheckSlot(CardManager card, int index) {
        if (index == 4 && card.GetComponent<CardManager>() is not NumberCard) {
            return false;
        }
        return true;
    }

    public void PlayerPlayCard(GameObject card, int index) {
        playedCards[index] = card;
        card.transform.SetParent(playerSlots[index]);

        CardManager cardManager = card.GetComponent<CardManager>();
        if (cardManager.animEnumerator != null) {
            StopCoroutine(cardManager.animEnumerator);
        }
        cardManager.animEnumerator = AnimUtils.TweenPos(card.transform, new Vector2(0, 0), 0.25f, AnimUtils.CubicOut);
        StartCoroutine(cardManager.animEnumerator);

        if (index < 4) cardManager.OnClick = CardClicked;
        else cardManager.OnClick = null;

        card.transform.localRotation = Quaternion.Euler(0, 0, 0);
        card.transform.GetChild(0).localPosition = new Vector2(0, 0f);
        card.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        card.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(85/255f, 168/255f, 212/255f);
        card.transform.localScale = playerSlots[index].localScale;

        gameManager.variables = 0;
        gameManager.requiredVars = 0;
        gameManager.varSearching = false;
        for (int i = 0; i < 4; i++) {
            if (selectedCards[i]) {
                StartCoroutine(DestroyCard(playedCards[i]));
                selectedCards[i] = false;
            }
        }
    }

    public IEnumerator OpponentPlayCard(CardData cardData, Vector2 oppPos, int index) {
        GameObject card = Instantiate(cardPrefab, oppPos, Quaternion.identity, opponentSlots[index]);
        opponentCards[index] = card;

        CardManager cardManager;

        if (cardData is NumberCardData) {
            cardManager = card.AddComponent<NumberCard>();
        } else if (cardData is OperationCardData) {
            cardManager = card.AddComponent<OperationCard>();
        } else {
            cardManager = card.AddComponent<SpecialCard>();
        }

        cardManager.Init(-1, cardData);
        card.GetComponent<UpdateCard>().InitValues(cardData);
        if (cardManager.animEnumerator != null) {
            StopCoroutine(cardManager.animEnumerator);
        }
        cardManager.animEnumerator = AnimUtils.TweenPos(card.transform, new Vector2(0, 0), 0.25f, AnimUtils.CubicOut);
        card.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        card.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(209/255f, 114/255f, 108/255f);
        card.transform.localScale = opponentSlots[index].localScale;
        yield return StartCoroutine(cardManager.animEnumerator);
    }

    public IEnumerator ApplyOperationsPlayer() {
        if (playedCards[4] != null) {
            Transform alphaCardTrans = playedCards[4].transform;
            NumberCard alphaCard = alphaCardTrans.GetComponent<NumberCard>();
            for (int i = 0; i < 4; i++) {
                if (playedCards[i] != null) {
                    if (playedCards[i].GetComponent<CardManager>() is OperationCard opCard) {
                        switch (opCard.operation) {
                            case OperationCardData.Operation.Add:
                                alphaCard.value += (ulong)opCard.operand;
                                break;
                            case OperationCardData.Operation.Multiply:
                                alphaCard.value *= (ulong)opCard.operand;
                                break;
                            case OperationCardData.Operation.Exponentiate:
                                alphaCard.value = (ulong)Math.Pow(alphaCard.value, opCard.operand);
                                break;
                        }
                        alphaCardTrans.GetComponent<UpdateCard>().UpdateFaceText(alphaCard.value.ToString());
                        StartCoroutine(gameManager.Shake(alphaCardTrans, alphaCard.animImageRotEnumerator, 2, 0));
                        yield return gameManager.Shake(playedCards[i].transform, opCard.animImageRotEnumerator, 2, 0);
                    }
                }
            }
        }
    }

    public IEnumerator ApplyOperationsOpponent() {
        if (opponentCards[4] != null) {
            Transform alphaCardTrans = opponentCards[4].transform;
            NumberCard alphaCard = alphaCardTrans.GetComponent<NumberCard>();
            for (int i = 0; i < 4; i++) {
                if (opponentCards[i] != null) {
                    if (opponentCards[i].GetComponent<CardManager>() is OperationCard opCard) {
                        switch (opCard.operation) {
                            case OperationCardData.Operation.Add:
                                alphaCard.value += (ulong)opCard.operand;
                                break;
                            case OperationCardData.Operation.Multiply:
                                alphaCard.value *= (ulong)opCard.operand;
                                break;
                            case OperationCardData.Operation.Exponentiate:
                                alphaCard.value = (ulong)Math.Pow(alphaCard.value, opCard.operand);
                                break;
                        }
                        alphaCardTrans.GetComponent<UpdateCard>().UpdateFaceText(alphaCard.value.ToString());
                        StartCoroutine(gameManager.Shake(alphaCardTrans, alphaCard.animImageRotEnumerator, 2, 0));
                        yield return gameManager.Shake(opponentCards[i].transform, opCard.animImageRotEnumerator, 2, 0);
                    }
                }
            }
        }
    }

    public IEnumerator PlayerAttack() {
        for (int i = 0; i < 4; i++) {
            if (playedCards[i] != null && opponentCards[i] != null) {
                CardManager cardManager = playedCards[i].GetComponent<CardManager>();
                CardManager oppCardManager = opponentCards[i].GetComponent<CardManager>();
                if (cardManager.damage != 0) {
                    yield return AnimUtils.TweenPos(playedCards[i].transform, new Vector2(playedCards[i].transform.localPosition.x, 530), 0.4f, AnimUtils.ElasticInOut);
                    oppCardManager.health = Math.Max(0, oppCardManager.health - cardManager.damage);
                    opponentCards[i].GetComponent<UpdateCard>().UpdateHealth(oppCardManager.health);
                    if (oppCardManager.health <= 0) {
                        StartCoroutine(DestroyCard(opponentCards[i]));
                    } else {
                        StartCoroutine(gameManager.Shake(opponentCards[i].transform, opponentCards[i].GetComponent<CardManager>().animEnumerator, 2, 0));
                    }
                    yield return AnimUtils.TweenPos(playedCards[i].transform, new Vector2(playedCards[i].transform.localPosition.x, 0), 0.7f, AnimUtils.ElasticInOut);
                }
            }
        }
    }

    public IEnumerator OpponentAttack() {
        for (int i = 0; i < 4; i++) {
            if (playedCards[i] != null && opponentCards[i] != null) {
                CardManager cardManager = playedCards[i].GetComponent<CardManager>();
                CardManager oppCardManager = opponentCards[i].GetComponent<CardManager>();
                if (oppCardManager.damage != 0) {
                    yield return AnimUtils.TweenPos(opponentCards[i].transform, new Vector2(opponentCards[i].transform.localPosition.x, 530), 0.4f, AnimUtils.ElasticInOut);
                    cardManager.health = Math.Max(0, cardManager.health - oppCardManager.damage);
                    playedCards[i].GetComponent<UpdateCard>().UpdateHealth(cardManager.health);
                    if (cardManager.health <= 0) {
                        StartCoroutine(DestroyCard(playedCards[i]));
                    } else {
                        StartCoroutine(gameManager.Shake(playedCards[i].transform, playedCards[i].GetComponent<CardManager>().animEnumerator, 2, 0));
                    }
                    yield return AnimUtils.TweenPos(opponentCards[i].transform, new Vector2(opponentCards[i].transform.localPosition.x, 0), 0.7f, AnimUtils.ElasticInOut);
                }
            }
        }
    }

    private IEnumerator DestroyCard(GameObject card) {
        StartCoroutine(gameManager.Shake(card.transform, card.GetComponent<CardManager>().animEnumerator, 2, 0));
        yield return StartCoroutine(AnimUtils.TweenScale(card.transform, new Vector2(0.01f, 0.01f), 0.7f, AnimUtils.ElasticInOut));;
        StopCoroutine(card.GetComponent<CardManager>().animEnumerator);
        Destroy(card);
    }

}
