using UnityEngine;
using TMPro;
using System.Collections;
using CavlonUtils;

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

    private int round = 0;
    public int bytes = 5;
    public int variables = 0;
    public int requiredVars = 0;

    public bool varSearching = false;

    private IEnumerator endTurnScaleEnumerator;
    private IEnumerator endTurnRotEnumerator;
    private IEnumerator BytesAnimEnumerator;
    private IEnumerator roundCounterAnimEnumerator;

    void Start() {
        UpdateBytes(bytes);
    }

    public void EndTurn() {
        Debug.Log("Turn Ended");
        round++;
        roundCounter.text = "t = " + round;
        UpdateBytes(bytes+1);
        StartCoroutine(Shake(endTurn, endTurnRotEnumerator, 3, 0));
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
