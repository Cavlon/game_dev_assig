using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public abstract class CardManager : MonoBehaviour
{

    public delegate void ClickCallback(int id);
    public ClickCallback OnClick;
    public int id;
    public IEnumerator animImageRotEnumerator;
    public IEnumerator animImagePosEnumerator;
    public IEnumerator animEnumerator;
    public CardData cardData;
    public int health;
    public int damage;

    public void Clicked() {
        Debug.Log("Card " + id + " Clicked");
        if (OnClick == null) return;
        OnClick(id);
    }

    public virtual void Init(int newId, CardData newCardData) {
        id = newId;
        cardData = newCardData;
        health = cardData.health;
        damage = cardData.damage;
    }

    public void AddClickFunction() {
        EventTrigger eventTrigger = transform.GetChild(0).GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => { Clicked(); });
        eventTrigger.triggers.Add(entry);
    }
}
