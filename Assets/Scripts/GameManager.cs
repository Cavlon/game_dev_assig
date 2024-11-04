using UnityEngine;
using TMPro;
using System.Collections;
using CavlonUtils;
using Unity.VisualScripting;
using Unity.Mathematics;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text roundCounter;
    [SerializeField]
    private TMP_Text bytesCounter;
    [SerializeField]
    private Transform endTurn;
    [SerializeField]
    private Transform bytesImage;

    [SerializeField]
    private SlotManager slotManager;
    [SerializeField]
    private DeckManager deckManager;
    [SerializeField]
    private HandManager handManager;

    [SerializeField]
    private GameObject opponentPrefab;

    public int round = 0;
    public int bytes = 5;
    public int variables = 0;
    public int requiredVars = 0;

    public bool varSearching = false;

    private GameObject opponent;
    private Opponent opponentScript;

    private IEnumerator endTurnScaleEnumerator;
    private IEnumerator endTurnRotEnumerator;
    private IEnumerator BytesAnimEnumerator;
    private IEnumerator roundCounterAnimEnumerator;

    void Start() {
        UpdateBytes(bytes);
        opponent = Instantiate(opponentPrefab, transform.GetChild(0).position, Quaternion.identity, transform.GetChild(0));
        opponentScript = opponent.GetComponent<Opponent>();
        opponentScript.slotManager = slotManager;
        opponentScript.gameManager = this;

        deckManager.Shuffle();
        deckManager.DrawCard(true);
        deckManager.DrawCard(true);
        deckManager.DrawCard(true);
        deckManager.DrawCard(true);
        deckManager.DrawCard(false);
    }

    public void EndTurn() {
        StartCoroutine(EndTurnEnum());
    }

    private IEnumerator EndTurnEnum() {
        StartCoroutine(Shake(endTurn, endTurnRotEnumerator, 3, 0));
        yield return slotManager.ApplyOperationsPlayer();
        yield return slotManager.PlayerAttack();
        yield return opponentScript.OpponentTurn();
        yield return slotManager.ApplyOperationsOpponent();
        yield return slotManager.OpponentAttack();
        StartTurn();
    }

    private void StartTurn() {
        Debug.Log("Turn Started");
        deckManager.canDraw = true;
        round++;
        roundCounter.text = "t = " + round;
        UpdateBytes(bytes+1);
        StartCoroutine(Bounce(roundCounter.transform, roundCounterAnimEnumerator, 50));
    }

    public void UpdateBytes(int newVal) {
        if (newVal > 99) newVal = 99;
        bytes = newVal;
        bytesCounter.text = "x" + bytes;
    }

    public void OnEndTurnHover() {
        Debug.Log("End Turn Hover");
        if (endTurnScaleEnumerator != null) {
            StopCoroutine(endTurnScaleEnumerator);
        }
        endTurnScaleEnumerator = AnimUtils.TweenScale(endTurn, new Vector3(1.6f, 1.6f, 1f), 0.2f, AnimUtils.CubicOut);
        StartCoroutine(endTurnScaleEnumerator);
    }

    public void OnEndTurnUnHover() {
        Debug.Log("End Turn Unhover");
        if (endTurnScaleEnumerator != null) {
            StopCoroutine(endTurnScaleEnumerator);
        }
        endTurnScaleEnumerator = AnimUtils.TweenScale(endTurn, new Vector3(1.4f, 1.4f, 1f), 0.2f, AnimUtils.CubicOut);
        StartCoroutine(endTurnScaleEnumerator);
    }

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

    public IEnumerator ShakeBytes() {
        StartCoroutine(Shake(bytesImage, BytesAnimEnumerator, 2, 0));
        yield return 0;
    }
}
