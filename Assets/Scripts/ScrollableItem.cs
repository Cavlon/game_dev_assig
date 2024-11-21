using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using CavlonUtils;
public class ScrollableItem : EventTrigger {

    public delegate void ClickCallback(CardData cardData);
    public ClickCallback OnClick;

    public ScrollRect scrollArea;

    public CardData cardData;

    private IEnumerator cardEnumerator;
    private Vector2 initScale;

    void Start() {
        initScale = transform.parent.localScale;
    }

    public override void OnBeginDrag( PointerEventData PED )
    {
        scrollArea.OnBeginDrag( PED );
    }

    public override void OnDrag( PointerEventData PED )
    {
        scrollArea.OnDrag( PED );
    }

    public override void OnEndDrag( PointerEventData PED )
    {
        scrollArea.OnEndDrag( PED );
    }

    public override void OnScroll( PointerEventData PED )
    {
        scrollArea.OnScroll( PED );
    }
    public override void OnPointerClick (PointerEventData PED )
    {
        if (OnClick == null) return;
        OnClick(cardData);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (cardEnumerator != null) {
            StopCoroutine(cardEnumerator);
        }
        cardEnumerator = AnimUtils.UnTimeScaledTweenScale(transform.parent, new Vector2(initScale.x * 1.1f, initScale.y * 1.1f), 0.2f, AnimUtils.CubicOut);
        StartCoroutine(cardEnumerator);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (cardEnumerator != null) {
            StopCoroutine(cardEnumerator);
        }
        cardEnumerator = AnimUtils.UnTimeScaledTweenScale(transform.parent, initScale, 0.2f, AnimUtils.CubicOut);
        StartCoroutine(cardEnumerator);
    }
}
