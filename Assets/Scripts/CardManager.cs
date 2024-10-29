using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class CardManager : MonoBehaviour
{

    public delegate void ClickCallback(int id);
    public ClickCallback OnClick;
    public int id;

    public void Clicked() {
        Debug.Log("Card " + id + " Clicked");
        if (OnClick == null) return;
        OnClick(id);
    }

    public virtual void Init(int newId, CardData newCardData) {
        id = newId;
        EventTrigger eventTrigger = transform.GetChild(0).GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => { Clicked(); });
        eventTrigger.triggers.Add(entry);
    }
}
