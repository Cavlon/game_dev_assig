using System.Collections;
using UnityEngine;

public class GimbalOpponent : Opponent
{
    public override IEnumerator OpponentTurn()
    {
        if (gameManager.round == 1) {
            yield return slotManager.OpponentPlayCard(cards[1], transform.position, 4);
            yield return slotManager.OpponentPlayCard(cards[2], transform.position, 1);
            yield return slotManager.OpponentPlayCard(cards[5], transform.position, 0);
        } else if (gameManager.round % 2 == 0) {
            for (int i = 3; i > -1; i--) {
                if (slotManager.opponentCards[i] == null) {
                    yield return slotManager.OpponentPlayCard(cards[0], transform.position, i);
                    break;
                }
            }
        } else if (gameManager.round % 3 == 0) {
            for (int i = 0; i < 4; i++) {
                if (slotManager.opponentCards[i] == null) {
                    yield return slotManager.OpponentPlayCard(cards[6], transform.position, i);
                    break;
                }
            }
            for (int i = 3; i > -1; i--) {
                if (slotManager.opponentCards[i] == null) {
                    yield return slotManager.OpponentPlayCard(cards[3], transform.position, i);
                    break;
                }
            }
            for (int i = 0; i < 4; i++) {
                if (slotManager.opponentCards[i] == null) {
                    yield return slotManager.OpponentPlayCard(cards[2], transform.position, i);
                    break;
                }
            }
        } else if (gameManager.round % 4 == 0) {
            for (int i = 0; i < 4; i++) {
                if (slotManager.opponentCards[i] == null) {
                    yield return slotManager.OpponentPlayCard(cards[4], transform.position, i);
                    break;
                }
            }
        } else if (gameManager.round % 6 == 0) {
            for (int i = 0; i < 4; i++) {
                if (slotManager.opponentCards[i] == null) {
                    yield return slotManager.OpponentPlayCard(cards[5], transform.position, i);
                    break;
                }
            }
        }
    }
}
