using UnityEngine;
using System.Collections.Generic;

public class SlotManager : MonoBehaviour
{
    [SerializeField]
    private HandManager handManager;

    private Transform[] slots = new Transform[5];
    private GameObject[] playedCards = new GameObject[5];

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

    public bool PlayCard(GameObject card, int index) {
        if (playedCards[index] != null) {
            return false;
        }
        if (index == 4 && card.GetComponent<CardManager>() is not NumberCard) {
            return false;
        }
        playedCards[index] = card;
        card.transform.SetParent(slots[index]);
        card.transform.localPosition = new Vector2(0, 0);
        card.transform.localRotation = Quaternion.Euler(0, 0, 0);
        card.transform.GetChild(0).localPosition = new Vector2(0, 0f);
        return true;
    }

}
