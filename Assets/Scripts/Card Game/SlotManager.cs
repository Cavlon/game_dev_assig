using UnityEngine;
using UnityEngine.UI;
using CavlonUtils;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Data;
using Unity.Mathematics;

public class SlotManager : MonoBehaviour
{

    private delegate IEnumerator SpecialAbility(int cardInd, CardManager cardManager, GameObject[] attackingCards, GameObject[] victimCards, int[] args);


    private Dictionary<string, SpecialAbility> abilityDict = new Dictionary<string, SpecialAbility>();

    [SerializeField]
    private HandManager handManager;
    [SerializeField]
    private DeckManager deckManager;
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private SoundManager soundManager;

    [SerializeField]
    private GameObject cardPrefab;

    // The different slots on the page
    private Transform[] playerSlots = new Transform[5];
    public Transform[] opponentSlots = new Transform[5];
    private GameObject[] playerCards = new GameObject[5];
    public GameObject[] opponentCards = new GameObject[5];

    // Which cards have been selected for sacrifice
    private bool[] selectedCards = new bool[4];

    private bool skipCard = false;

    void Start() {

        abilityDict.Add("DoubleAttack", DoubleAttack);
        abilityDict.Add("TripleAttack", TripleAttack);
        abilityDict.Add("Move", Move);
        abilityDict.Add("Push", Push);

        // Initialise all the slots
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

    public void SlotClicked(int ind) {      // Inform the hand that a slot has been clicked
        handManager.slotInd = ind;
        Debug.Log("Slot " + ind + " Clicked");
    }

    public void CardClicked(int id) {       // Label a card as a sacrifice target if variables are requires and it has been clicked
        if (!gameManager.varSearching) return;

        // Identify which card has been clicked
        int ind = 0;
        for (int i = 0; i < 4; i++) {
            if (playerCards[i] != null && playerCards[i].GetComponent<CardManager>().id == id) {
                ind = i;
                break;
            }
        }

        // Select or unselect the clicked card
        if (!selectedCards[ind]) {
            if (gameManager.variables == gameManager.requiredVars) return;  // Don't sacrifice more cards than needed
            selectedCards[ind] = true;
            soundManager.PlaySound(1);
            gameManager.variables++;
            playerCards[ind].transform.GetChild(0).Find("Generalise").gameObject.SetActive(true);
            StartCoroutine(gameManager.Shake(playerCards[ind].transform, playerCards[ind].GetComponent<CardManager>().animEnumerator, 1, 0));
        } else {
            selectedCards[ind] = false;
            soundManager.PlaySound(1);
            gameManager.variables--;
            playerCards[ind].transform.GetChild(0).Find("Generalise").gameObject.SetActive(false);
            StartCoroutine(gameManager.Shake(playerCards[ind].transform, playerCards[ind].GetComponent<CardManager>().animEnumerator, 1, 0));
        }
    }

    public void ResetSelection() {      // Forcefully reset the sacrfice targets
        for (int i = 0; i < 4; i++) {
            selectedCards[i] = false;
            if (playerCards[i] != null) playerCards[i].transform.GetChild(0).Find("Generalise").gameObject.SetActive(false);
        }
    }

    public bool CheckSlot(CardManager card, int index) {    // Check if the card can be placed in the alpha slot
        if (index == 4 && card.GetComponent<CardManager>() is not NumberCard) {
            soundManager.PlaySound(8);
            return false;
        }
        return true;
    }

    // The player plays a card on a certain slot
    public void PlayerPlayCard(GameObject card, int index) {
        // Add the card to the slot
        playerCards[index] = card;
        card.transform.SetParent(playerSlots[index]);

        // Animate the card moving to the slot
        CardManager cardManager = card.GetComponent<CardManager>();
        if (cardManager.animEnumerator != null) {
            StopCoroutine(cardManager.animEnumerator);
        }
        cardManager.animEnumerator = AnimUtils.TweenPos(card.transform, Vector2.zero, 0.25f, AnimUtils.CubicOut);
        StartCoroutine(cardManager.animEnumerator);

        // Assign click callback
        if (index < 4) cardManager.OnClick = CardClicked;
        else cardManager.OnClick = null;

        // Reset the transform of the card
        card.transform.localRotation = Quaternion.Euler(0, 0, 0);
        card.transform.GetChild(0).localPosition = new Vector2(0, 0f);
        card.transform.localScale = playerSlots[index].localScale;

        // Activate the card outline
        card.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        card.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(85/255f, 168/255f, 212/255f);    

        // Reset variables
        gameManager.variables = 0;
        gameManager.requiredVars = 0;
        gameManager.varSearching = false;

        // Destroy all sacrificed cards
        for (int i = 0; i < 4; i++) {
            if (selectedCards[i]) {
                deckManager.discardPile.Add(playerCards[i].GetComponent<CardManager>().cardData);
                StartCoroutine(DestroyCard(playerCards[i]));
                selectedCards[i] = false;
            }
        }

        soundManager.PlaySound(0);

    }

    // Opponent plays a card on a certain slot
    public IEnumerator OpponentPlayCard(CardData cardData, Vector2 oppPos, int index) {
        // Create the card and assign it to the slot
        GameObject card = Instantiate(cardPrefab, oppPos, Quaternion.identity, opponentSlots[index]);
        opponentCards[index] = card;

        CardManager cardManager;

        if (cardData is NumberCardData) {   // Determine the card's type
            cardManager = card.AddComponent<NumberCard>();
        } else if (cardData is OperationCardData) {
            cardManager = card.AddComponent<OperationCard>();
        } else {
            cardManager = card.AddComponent<SpecialCard>();
        }

        // Initialise the card
        cardManager.Init(-1, cardData);
        card.GetComponent<UpdateCard>().InitValues(cardData);

        // Animate the card moving to the slot
        if (cardManager.animEnumerator != null) {
            StopCoroutine(cardManager.animEnumerator);
        }
        cardManager.animEnumerator = AnimUtils.TweenPos(card.transform, new Vector2(0, 0), 0.25f, AnimUtils.CubicOut);
        card.transform.localScale = opponentSlots[index].localScale;

        // Activate the card outline
        card.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        card.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(209/255f, 114/255f, 108/255f);
        yield return cardManager.animEnumerator;
        soundManager.PlaySound(0);
    }

    public IEnumerator ApplyOperationsPlayer() {
        yield return ApplyOperations(playerCards);
    }

    public IEnumerator ApplyOperationsOpponent() {
        yield return ApplyOperations(opponentCards);
    }

    public IEnumerator PlayerAttack() {
        yield return Attack(playerCards, opponentCards);
    }

    public IEnumerator OpponentAttack() {
        yield return Attack(opponentCards, playerCards);
    }

    public IEnumerator PlayerAlphaAttack() {
        yield return AlphaAttack(playerCards, 1);
    }

    public IEnumerator OpponentAlphaAttack() {
        yield return AlphaAttack(opponentCards, -1);
    }

    // Apply all operations to the alpha card
    private IEnumerator ApplyOperations(GameObject[] cards) {
        if (cards[4] != null) {   // Check if an alpha card has been played
            Transform alphaCardTrans = cards[4].transform;
            NumberCard alphaCard = alphaCardTrans.GetComponent<NumberCard>();
            for (int i = 0; i < 4; i++) {       // Iterate through all operation cards
                if (cards[i] != null) {

                    // Apply the specified operation to the alpha card
                    if (cards[i].GetComponent<CardManager>() is OperationCard opCard) {
                        alphaCard.value = (ulong)ParseEquationString(opCard.equation, (int)alphaCard.value);

                        alphaCardTrans.GetComponent<UpdateCard>().UpdateFaceText(alphaCard.value.ToString());
                        StartCoroutine(gameManager.Shake(alphaCardTrans, alphaCard.animImageRotEnumerator, 2, 0));
                        soundManager.PlaySound(6);
                        yield return gameManager.Shake(cards[i].transform, opCard.animImageRotEnumerator, 2, 0);
                        yield return new WaitForSeconds(0.15f);
                    }
                }
            }
        }
    }

    // Attack with all cards
    private IEnumerator Attack(GameObject[] attackingCards, GameObject[] victimCards) {
        for (int i = 0; i < 4; i++) {       // Iterate through all the played cards
            if (skipCard) {
                skipCard = false;
                continue;
            }
            if (attackingCards[i] != null) {
                CardManager cardManager = attackingCards[i].GetComponent<CardManager>();

                if (cardManager is SpecialCard specCard) {
                    yield return abilityDict[specCard.specialKey](i, cardManager, attackingCards, victimCards, specCard.abilityArgs);
                } else {
                    yield return NormalAttack(i, i, cardManager.damage, cardManager, attackingCards, victimCards);
                }
            }
        }
    }

    // Animates and applies the apha value to the health scale
    private IEnumerator AlphaAttack(GameObject[] cards, int playerOrOpponnent) {
        if (cards[4] != null) {
            soundManager.PlaySound(10);
            StartCoroutine(gameManager.Shake(cards[4].transform, cards[4].GetComponent<CardManager>().animEnumerator, 2, 0));
            yield return AnimUtils.TweenScale(cards[4].transform, new Vector2(1.2f, 1.2f), 0.2f, AnimUtils.ElasticInOut);
            gameManager.TipScale((int)cards[4].GetComponent<NumberCard>().value * playerOrOpponnent);
            yield return AnimUtils.TweenScale(cards[4].transform, new Vector2(1f, 1f), 0.2f, AnimUtils.ElasticInOut);
            yield return new WaitForSeconds(0.5f);
        }
    }

    // Animate card death
    private IEnumerator DestroyCard(GameObject card) {
        soundManager.PlaySound(2);
        StartCoroutine(gameManager.Shake(card.transform, card.GetComponent<CardManager>().animEnumerator, 2, 0));
        yield return AnimUtils.TweenScale(card.transform, new Vector2(0.01f, 0.01f), 0.7f, AnimUtils.ElasticInOut);;
        StopCoroutine(card.GetComponent<CardManager>().animEnumerator);
        Destroy(card);
    }

    public int ParseEquationString(string equation, int alpha) {
        string[] operations = equation.Split(';');
        float res = alpha;
        foreach (string op in operations) {

            if (op[0] == 'r') {
                res = (float)Math.Sqrt(res);
            }

            int pointer = 1;
            float left = 0;

            if (op[0] == 'a') left = res;
            else if (op[0] == 't') left = gameManager.round;
            else {
                string num = "";
                for (int i = 0; i < op.Length; i++) {
                    if (char.IsDigit(op[i])) {
                        num += op[i];
                    } else {
                        left = int.Parse(num);
                        pointer = i;
                        break;
                    }
                }
            }

            char operation = op[pointer];
            pointer++;

            float right = 0;

            if (op[pointer] == 'a') right = res;
            else if (op[pointer] == 't') right = gameManager.round;
            else {
                string num = "";
                for (int i = pointer; i < op.Length; i++) {
                    if (char.IsDigit(op[i])) {
                        num += op[i];
                    } else {
                        right = int.Parse(num);
                        break;
                    }
                }
                right = int.Parse(num);
            }

            switch (operation) {
                case '+':
                    res = left + right;
                    break;
                case '-':
                    res = left - right;
                    break;
                case '*':
                    res = left * right;
                    break;
                case '/':
                    res = left / right;
                    break;
                case '^':
                    res = (float)Math.Pow(left, right);
                    break;
            }
        }

        return (int)res;
    }

    private IEnumerator NormalAttack(int attackInd, int victimInd, int damage, CardManager cardManager, GameObject[] attackingCards, GameObject[] victimCards) {
        if (victimCards[victimInd] != null && damage != 0) {       // Only attack if there is a played card and there is a valid opponent           
            CardManager oppCardManager = victimCards[victimInd].GetComponent<CardManager>();

            Vector2 initPos = attackingCards[attackInd].transform.localPosition;

            // Animate the attack
            yield return AnimUtils.TweenPos(attackingCards[attackInd].transform, new Vector2(initPos.x, -60), 0.1f, AnimUtils.CubicIn);
            soundManager.PlaySound(3);
            yield return AnimUtils.TweenPos(attackingCards[attackInd].transform, new Vector2(attackingCards[attackInd].transform.InverseTransformPoint(victimCards[victimInd].transform.position).x, 530), 0.25f, AnimUtils.CubicIn);
            StartCoroutine(gameManager.Shake(attackingCards[attackInd].transform, cardManager.animImageRotEnumerator, 2, 0));

            // Deal damage to the victim card
            oppCardManager.health = Math.Max(0, oppCardManager.health - damage);
            victimCards[victimInd].GetComponent<UpdateCard>().UpdateHealth(oppCardManager.health);

            // Destroy the victim if it dies
            if (oppCardManager.health <= 0) {
                if (victimCards == playerCards) {
                    deckManager.discardPile.Add(oppCardManager.cardData);
                }
                StartCoroutine(DestroyCard(victimCards[victimInd]));
            } else {
                StartCoroutine(gameManager.Shake(victimCards[victimInd].transform, oppCardManager.animEnumerator, 2, 0));
            }

            // Return the card to the slot
            yield return AnimUtils.TweenPos(attackingCards[attackInd].transform, initPos, 1.2f, AnimUtils.QuintInOut);
            yield return new WaitForSeconds(0.04f);

        }
    }
    private IEnumerator DoubleAttack(int cardInd, CardManager cardManager, GameObject[] attackingCards, GameObject[] victimCards, int[] damages) {

        if (cardInd == 0) {

            yield return NormalAttack(cardInd, cardInd+1, damages[1], cardManager, attackingCards, victimCards);

        } else if (cardInd == 3 && victimCards[cardInd-1] != null) {

            yield return NormalAttack(cardInd, cardInd-1, damages[0], cardManager, attackingCards, victimCards);

        } else {
            yield return NormalAttack(cardInd, cardInd-1, damages[0], cardManager, attackingCards, victimCards);
            yield return NormalAttack(cardInd, cardInd+1, damages[1], cardManager, attackingCards, victimCards);
        }
    }

    private IEnumerator TripleAttack(int cardInd, CardManager cardManager, GameObject[] attackingCards, GameObject[] victimCards, int[] damages) {

        if (cardInd == 0) {

            yield return NormalAttack(cardInd, cardInd, damages[1], cardManager, attackingCards, victimCards);
            yield return NormalAttack(cardInd, cardInd+1, damages[2], cardManager, attackingCards, victimCards);

        } else if (cardInd == 3 && victimCards[cardInd-1] != null) {

            yield return NormalAttack(cardInd, cardInd-1, damages[0], cardManager, attackingCards, victimCards);
            yield return NormalAttack(cardInd, cardInd, damages[1], cardManager, attackingCards, victimCards);

        } else {
            yield return NormalAttack(cardInd, cardInd-1, damages[0], cardManager, attackingCards, victimCards);
            yield return NormalAttack(cardInd, cardInd, damages[1], cardManager, attackingCards, victimCards);
            yield return NormalAttack(cardInd, cardInd+1, damages[2], cardManager, attackingCards, victimCards);
        }
    }

    private IEnumerator Move(int cardInd, CardManager cardManager, GameObject[] attackingCards, GameObject[] victimCards, int[] args) {
        int moveDir = args[0];

        if (victimCards[cardInd] != null) {
            yield return NormalAttack(cardInd, cardInd, cardManager.damage, cardManager, attackingCards, victimCards);
        }

        if (cardInd + moveDir > -1 && cardInd + moveDir < 4 && attackingCards[cardInd + moveDir] == null) {
            attackingCards[cardInd + moveDir] = attackingCards[cardInd];
            attackingCards[cardInd] = null;
            if (attackingCards == playerCards) {
                attackingCards[cardInd + moveDir].transform.SetParent(playerSlots[cardInd + moveDir]);
            } else {
                attackingCards[cardInd + moveDir].transform.SetParent(opponentSlots[cardInd + moveDir]);
            }
            
            yield return AnimUtils.TweenPos(attackingCards[cardInd + moveDir].transform, Vector2.zero, 0.7f, AnimUtils.CubicOut);
            yield return new WaitForSeconds(0.04f);

            skipCard = true;
        }
    }

    private IEnumerator Push(int cardInd, CardManager cardManager, GameObject[] attackingCards, GameObject[] victimCards, int[] args) {
        int moveDir = args[0];

        if (victimCards[cardInd] != null) {
            yield return NormalAttack(cardInd, cardInd, cardManager.damage, cardManager, attackingCards, victimCards);
        }

        int nullInd = -1;

        for (int i = cardInd; cardInd + moveDir > -1 && cardInd + moveDir < 4; i += moveDir) {
            if (attackingCards[i] == null) {
                nullInd = i;
                break;
            }
        }

        if (nullInd != -1) {
            while (nullInd != cardInd) {
                attackingCards[nullInd] = attackingCards[nullInd - moveDir];
                if (attackingCards == playerCards) {
                    attackingCards[nullInd].transform.SetParent(playerSlots[nullInd]);
                } else {
                    attackingCards[nullInd].transform.SetParent(opponentSlots[nullInd]);
                }
                
                StartCoroutine(AnimUtils.TweenPos(attackingCards[nullInd].transform, Vector2.zero, 0.7f, AnimUtils.CubicOut));

                nullInd = nullInd - moveDir;
            }

            attackingCards[cardInd + moveDir] = attackingCards[cardInd];
            attackingCards[cardInd] = null;
            if (attackingCards == playerCards) {
                attackingCards[cardInd + moveDir].transform.SetParent(playerSlots[cardInd + moveDir]);
            } else {
                attackingCards[cardInd + moveDir].transform.SetParent(opponentSlots[cardInd + moveDir]);
            }
            
            yield return AnimUtils.TweenPos(attackingCards[cardInd + moveDir].transform, Vector2.zero, 0.7f, AnimUtils.CubicOut);
            yield return new WaitForSeconds(0.04f);
            
            skipCard = true;
        }
    }
}
