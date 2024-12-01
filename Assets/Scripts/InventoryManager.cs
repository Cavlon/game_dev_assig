using System.Collections;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using CavlonUtils;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField]
    private GameObject item;
    [SerializeField]
    private GameObject cardPrefab;
    private Transform deckContent;
    private Transform invContent;
    private ScrollRect deckScroll;
    private ScrollRect invScroll;

    private Transform deckText;
    private IEnumerator deckEnumerator;

    private SoundManager soundManager;

    void Start() {
        deckText = transform.GetChild(0).Find("DeckText");

        deckScroll = transform.GetChild(0).GetComponentInChildren<ScrollRect>();
        invScroll = transform.GetChild(1).GetComponentInChildren<ScrollRect>();

        deckContent = transform.GetChild(0).Find("ScrollArea/Viewport/Content");
        invContent = transform.GetChild(1).Find("ScrollArea/Viewport/Content");

        soundManager = GameObject.Find("/SoundManager").GetComponent<SoundManager>();
    }

    public void UpdateInventory() {
        while (invContent.childCount > 0) {
            DestroyImmediate(invContent.GetChild(0).gameObject);
        }

        foreach (CardData key in StaticData.inventory.Keys) {
            GameObject newItem = Instantiate(item, Vector2.zero, Quaternion.identity, invContent);
            newItem.transform.GetChild(1).GetComponent<TMP_Text>().text = "x" + (StaticData.inventory[key] - StaticData.GetDeckCount(key));

            Transform card = newItem.transform.GetChild(0);
            card.GetComponent<UpdateCard>().InitValues(key);

            ScrollableItem scrollableItem = card.GetChild(0).AddComponent<ScrollableItem>();
            scrollableItem.cardData = key;
            scrollableItem.OnClick = InventoryItemClicked;
            scrollableItem.scrollArea = invScroll;
        }
    }

    public void UpdateDeck() {
        while (deckContent.childCount > 0) {
            DestroyImmediate(deckContent.GetChild(0).gameObject);
        }

        foreach (CardData card in StaticData.deck) {
            GameObject newItem = Instantiate(cardPrefab, Vector2.zero, Quaternion.identity, deckContent);
            newItem.GetComponent<UpdateCard>().InitValues(card);
            newItem.transform.localScale = new Vector2(0.64f, 0.64f);

            ScrollableItem scrollableItem = newItem.transform.GetChild(0).AddComponent<ScrollableItem>();
            scrollableItem.scrollArea = deckScroll;
        }
    }

    public void InventoryItemClicked(CardData card) {
        if (StaticData.deck.Count < 20) {
            soundManager.PlaySound(1);

            if (StaticData.deckDict.ContainsKey(card)) {
                if (StaticData.inventory[card] > StaticData.deckDict[card]) {
                    StaticData.deck.Add(card); 
                    StaticData.AddCardToDeck(card);
                }
            } else {
                StaticData.deck.Add(card);
                StaticData.AddCardToDeck(card);
            }

            UpdateDeck();
            UpdateInventory();
        }
    }

    public void EmptyDeck() {
        soundManager.PlaySound(2);
        StaticData.deck.Clear();
        StaticData.deckDict.Clear();
        UpdateDeck();
        UpdateInventory();
    }

    public void ShakeDeck() {
        soundManager.PlaySound(9);
        if (deckEnumerator != null) {
            StopCoroutine(deckEnumerator);
        }

        deckEnumerator = UnTimeScaledShake(deckText, 2, 0);
        StartCoroutine(deckEnumerator);
    }

    public IEnumerator UnTimeScaledShake(Transform trans, int shakes, float centerAngle, float shakeDelay = 0.1f) {
        for ( int i = 0; i < shakes; i++)
        {
            yield return AnimUtils.UnTimeScaledTweenRotZ(trans, centerAngle + 10f, shakeDelay, AnimUtils.CubicOut);
            yield return AnimUtils.UnTimeScaledTweenRotZ(trans, centerAngle - 10f, shakeDelay, AnimUtils.CubicOut);
        }
        yield return AnimUtils.UnTimeScaledTweenRotZ(trans, centerAngle, shakeDelay, AnimUtils.CubicOut);
    }
}
