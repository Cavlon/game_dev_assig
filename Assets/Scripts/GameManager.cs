using UnityEngine;
using TMPro;
using System.Collections;
using CavlonUtils;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text roundCounter;
    [SerializeField]
    private Transform bytesCounter;
    [SerializeField]
    private Transform endTurn;
    [SerializeField]
    private Slider healthScale;
    [SerializeField]
    private TMP_Text healthText;

    [SerializeField]
    private SlotManager slotManager;
    [SerializeField]
    private DeckManager deckManager;
    [SerializeField]
    private HandManager handManager;

    [SerializeField]
    private GameObject opponentPrefab;

    private Transform bytesImage;
    private TMP_Text bytesCounterText;

    public int round = 0;
    public int bytes = 5;
    public int variables = 0;
    public int requiredVars = 0;

    public bool varSearching = false;

    private bool canEnd = true;
    private int health = 0;

    private GameObject opponent;
    private Opponent opponentScript;

    private IEnumerator endTurnScaleEnumerator;
    private IEnumerator endTurnRotEnumerator;
    private IEnumerator BytesAnimEnumerator;
    private IEnumerator roundCounterAnimEnumerator;
    private IEnumerator healthAnimEnumerator;

    // Initialise the game
    void Start() {
        // Get bytes components
        bytesImage = bytesCounter.GetChild(0);
        bytesCounterText = bytesCounter.GetChild(1).GetComponent<TMP_Text>();
        UpdateBytes(bytes);

        // Create and add the opponent
        opponent = Instantiate(opponentPrefab, transform.GetChild(0).position, Quaternion.identity, transform.GetChild(0));
        opponentScript = opponent.GetComponent<Opponent>();
        opponentScript.slotManager = slotManager;
        opponentScript.gameManager = this;

        // Shuffle the deck and draw the starting hand
        deckManager.Shuffle();
        deckManager.DrawCard(true);
        deckManager.DrawCard(true);
        deckManager.DrawCard(true);
        deckManager.DrawCard(true);
        deckManager.DrawCard(true);
        deckManager.canDraw = false;
    }

    public void EndTurn() {     // Try to end the turn
        StartCoroutine(Shake(endTurn, endTurnRotEnumerator, 3, 0));
        if (!canEnd) return;    // Only end the turn if it's the player's turn
        StartCoroutine(EndTurnEnum());
    }

    // End the player's turn and perform the opponent's turn
    private IEnumerator EndTurnEnum() {
        canEnd = false;
        handManager.ResetSelection();
        handManager.LowerHand();
        handManager.selectedInd = -2;

        // Perform end-of-turn player actions
        yield return slotManager.ApplyOperationsPlayer();
        yield return slotManager.PlayerAttack();
        yield return slotManager.PlayerAlphaAttack();

        // Perform opponent turn and their end-of-turn actions
        yield return opponentScript.OpponentTurn();
        yield return slotManager.ApplyOperationsOpponent();
        yield return slotManager.OpponentAttack();
        yield return slotManager.OpponentAlphaAttack();

        // Start the player's turn again
        StartTurn();
    }

    // Start the player's turn
    private void StartTurn() {
        Debug.Log("Turn Started");
        deckManager.canDraw = true;    
        round++;
        roundCounter.text = "t = " + round;
        UpdateBytes(bytes+1);
        StartCoroutine(Bounce(roundCounter.transform, roundCounterAnimEnumerator, 50));
    }

    // The player has to draw before they can play any cards or end their turn
    public void CardDawn() {
        handManager.RaiseHand();
        handManager.selectedInd = -1;
        canEnd = true;
    }

    // Update the player's bytes
    public void UpdateBytes(int newVal) {
        if (newVal > 99) newVal = 99;
        bytes = newVal;
        bytesCounterText.text = "x" + bytes;
    }

    // Updates the health scale with a new value
    public void TipScale(int valueDelta) {
        health += valueDelta;
        if (healthAnimEnumerator != null) {
            StopCoroutine(healthAnimEnumerator);
        }
        healthAnimEnumerator = TweenHealth(health);
        StartCoroutine(healthAnimEnumerator);
    }

    // Animate the end turn icon when hovered over
    public void OnEndTurnHover() {
        Debug.Log("End Turn Hover");
        if (endTurnScaleEnumerator != null) {
            StopCoroutine(endTurnScaleEnumerator);
        }
        endTurnScaleEnumerator = AnimUtils.TweenScale(endTurn, new Vector3(1.6f, 1.6f, 1f), 0.2f, AnimUtils.CubicOut);
        StartCoroutine(endTurnScaleEnumerator);
    }

    // Animate the end turn icon when no longer hovered over
    public void OnEndTurnUnHover() {
        Debug.Log("End Turn Unhover");
        if (endTurnScaleEnumerator != null) {
            StopCoroutine(endTurnScaleEnumerator);
        }
        endTurnScaleEnumerator = AnimUtils.TweenScale(endTurn, new Vector3(1.4f, 1.4f, 1f), 0.2f, AnimUtils.CubicOut);
        StartCoroutine(endTurnScaleEnumerator);
    }

    // Interpolate the health slider value
    public IEnumerator TweenHealth(int targetHealth)
    {
        Debug.Log(targetHealth);
        float elapsed_time = 0; //Elapsed time
        float tmpHealth = healthScale.value;
        do 
        {
            elapsed_time += Time.deltaTime; //Adds to the elapsed time the amount of time needed to skip/wait one frame
            tmpHealth = Mathf.Lerp(tmpHealth, targetHealth, AnimUtils.SineOut(elapsed_time / 4f)); //Changes and interpolates the health
            healthScale.value = tmpHealth;
            healthText.text = healthScale.value.ToString();

            // Changes the colour of the text depending on who's winning
            if (healthScale.value > 0) {
                healthText.color = new Color(106/255f, 185/255f, 207/255f);
            } else if (healthScale.value < 0) {
                healthText.color = new Color(224/255f, 119/255f, 119/255f);
            } else {
                healthText.color = Color.white;
            }
            yield return 0;
        } while (elapsed_time <= 4f); //Inside the loop until the time expires
    }

    // Universal shake animation
    public IEnumerator Shake(Transform trans, IEnumerator enumerator, int shakes, float centerAngle, float shakeDelay = 0.1f) {
        if (enumerator != null) {
            StopCoroutine(enumerator);
        }
        for ( int i = 0; i < shakes; i++)
        {
            enumerator = AnimUtils.TweenRotZ(trans, centerAngle + 10f, shakeDelay, AnimUtils.CubicOut);
            StartCoroutine(enumerator);
            yield return new WaitForSeconds(shakeDelay);
            enumerator = AnimUtils.TweenRotZ(trans, centerAngle - 10f, shakeDelay, AnimUtils.CubicOut);
            StartCoroutine(enumerator);
            yield return new WaitForSeconds(shakeDelay);
        }
        enumerator = AnimUtils.TweenRotZ(trans, centerAngle, shakeDelay, AnimUtils.CubicOut);
        StartCoroutine(enumerator);
    }

    // Universal bounce animation
    public IEnumerator Bounce(Transform trans, IEnumerator enumerator, float height) {
        if (enumerator != null) {
            StopCoroutine(enumerator);
        }
        enumerator = AnimUtils.TweenPos(trans, new Vector2(0, height), 0.1f, AnimUtils.CubicOut);
        StartCoroutine(enumerator);
        yield return new WaitForSeconds(0.1f);
        enumerator = AnimUtils.TweenPos(trans, new Vector2(0, 0), 0.1f, AnimUtils.CubicIn);
        StartCoroutine(enumerator);
        yield return new WaitForSeconds(0.1f);
    }

    // Shakes the bytes icon
    public IEnumerator ShakeBytes() {
        StartCoroutine(Shake(bytesImage, BytesAnimEnumerator, 2, 0));
        yield return 0;
    }
}
