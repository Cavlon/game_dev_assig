using System.Collections;

public class TestOpponent : Opponent
{
    public override IEnumerator OpponentTurn()
    {
        if (gameManager.round == 1) {
            yield return slotManager.OpponentPlayCard(cards[0], transform.position, 4);
            yield return slotManager.OpponentPlayCard(cards[1], transform.position, 0);
            yield return slotManager.OpponentPlayCard(cards[1], transform.position, 1);
        } else if (gameManager.round % 2 == 0) {
            for (int i = 0; i < 4; i++) {
                if (slotManager.opponentCards[i] == null) {
                    yield return slotManager.OpponentPlayCard(cards[0], transform.position, i);
                    break;
                }
            }
        } else if (gameManager.round % 3 == 0) {
            for (int i = 0; i < 4; i++) {
                if (slotManager.opponentCards[i] == null) {
                    yield return slotManager.OpponentPlayCard(cards[2], transform.position, i);
                    break;
                }
            }
        }
    }
}
