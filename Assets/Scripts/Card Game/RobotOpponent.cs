using System.Collections;
using UnityEngine;

public class RobotOpponent : Opponent
{
    public override IEnumerator OpponentTurn()
    {
        if (gameManager.round == 1) {
            yield return slotManager.OpponentPlayCard(cards[1], transform.position, 4);
            yield return slotManager.OpponentPlayCard(cards[6], transform.position, 0);
            yield return slotManager.OpponentPlayCard(cards[7], transform.position, 3);
            yield return slotManager.OpponentPlayCard(cards[2], transform.position, 2);
        } else if (gameManager.round % 3 == 0) {
            for (int i = 0; i < 4; i++) {
                if (slotManager.opponentCards[i] == null) {
                    yield return slotManager.OpponentPlayCard(cards[5], transform.position, i);
                    break;
                }
            }
        } else if (gameManager.round % 4 == 0) {
            for (int i = 0; i < 4; i++) {
                if (slotManager.opponentCards[i] == null) {
                    yield return slotManager.OpponentPlayCard(cards[3], transform.position, i);
                    break;
                }
            }
            for (int i = 3; i > -1; i--) {
                if (slotManager.opponentCards[i] == null) {
                    yield return slotManager.OpponentPlayCard(cards[7], transform.position, i);
                    break;
                }
            }
        } else if (gameManager.round % 5 == 0) {
            for (int i = 0; i < 4; i++) {
                if (slotManager.opponentCards[i] == null) {
                    yield return slotManager.OpponentPlayCard(cards[4], transform.position, i);
                    break;
                }
            }
        } else if (gameManager.round % 7 == 0) {
            for (int i = 0; i < 4; i++) {
                if (slotManager.opponentCards[i] == null) {
                    yield return slotManager.OpponentPlayCard(cards[6], transform.position, i);
                    break;
                }
            }
        }
    }
}
