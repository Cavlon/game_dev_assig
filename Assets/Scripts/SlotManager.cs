using UnityEngine;
using CavlonUtils;
using System.Collections;

public class SlotManager : MonoBehaviour
{
    [SerializeField]
    private HandManager handManager;
    [SerializeField]
    private GameManager gameManager;

    private Transform[] slots = new Transform[5];
    private GameObject[] playedCards = new GameObject[5];
    private bool[] selectedCards = new bool[4];

    void Start() {
        Transform operationSlots = transform.parent.Find("PlayerSlots").GetChild(0);
        for (int i = 0; i < 4; i++) {
            slots[i] = operationSlots.GetChild(i);
        }
        slots[4] = transform.parent.Find("PlayerSlots").GetChild(1);
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
        } else {
            selectedCards[ind] = false;
            gameManager.variables--;
            playedCards[ind].transform.GetChild(0).Find("Generalise").gameObject.SetActive(false);
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

    public void PlayCard(GameObject card, int index) {
        playedCards[index] = card;
        card.transform.SetParent(slots[index]);

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
        card.transform.localScale = slots[index].localScale;

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

    private IEnumerator DestroyCard(GameObject card) {
        StartCoroutine(gameManager.Shake(card.transform, card.GetComponent<CardManager>().animEnumerator, 2, 0));
        yield return StartCoroutine(AnimUtils.TweenScale(card.transform, new Vector2(0.01f, 0.01f), 0.7f, AnimUtils.ElasticInOut));;
        Destroy(card);
    }

}
