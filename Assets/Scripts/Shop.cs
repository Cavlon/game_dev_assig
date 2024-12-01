using System.Collections;
using CavlonUtils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [SerializeField]
    private CardData[] numPackCards;
    [SerializeField]
    private CardData[] arithPackCards;  
    [SerializeField]
    private CardData[] linAlgPackCards;  
    [SerializeField]
    private CardData[] pathPackCards;

    private CardData[][] packCards;
    
    private Transform[] cardPacks = new Transform[4];


    [SerializeField]
    private Sprite[] packSprites = new Sprite[5];
    [SerializeField]
    private GameObject cardPrefab;

    private TMP_Text creditsText;
    public bool canClick = true;
    private Transform newCardsHolder;
    private GameObject newCardsText;
    public bool cardsDrawn = false;
    private OverworldManager overworldManager;
    private SoundManager soundManager;

    private IEnumerator[] packEnumerators = new IEnumerator[4];

    void Start() {
        overworldManager = GameObject.Find("/GameManager").GetComponent<OverworldManager>();
        soundManager = GameObject.Find("/SoundManager").GetComponent<SoundManager>();

        for (int i = 0; i < 4; i++) {
            cardPacks[i] = transform.Find("CardPacks").GetChild(i);
        }

        packCards = new CardData[][] {numPackCards, arithPackCards, linAlgPackCards, pathPackCards};

        creditsText = transform.Find("Credits").GetComponentInChildren<TMP_Text>();
        newCardsHolder = transform.Find("NewCards");
        newCardsText = newCardsHolder.Find("Text").gameObject;
        newCardsText.SetActive(false);
    }

    public void PackClicked(int ind) {
        if (!canClick || ind > StaticData.bossesBeat + 1) return;

        StartCoroutine(Shake(cardPacks[ind].transform, 2, 0));

        if (StaticData.credits > 4) {

            soundManager.PlaySound(4);
            DestroyCards();

            cardsDrawn = true;

            StartCoroutine(pickCards(ind));

            StaticData.credits -= 5;
            UpdateVisuals();
            overworldManager.UpdateHUD();

            newCardsText.SetActive(true);
        }
    }

    private IEnumerator pickCards(int ind) {
        canClick = false;
        CardData[] pickedCards = new CardData[3];

        for (int i = 0; i < 3; i++) {
            pickedCards[i] = packCards[ind][UnityEngine.Random.Range(0, packCards[ind].Length)];
            StaticData.AddCardToInventory(pickedCards[i]);
            GameObject newCard = Instantiate(cardPrefab, cardPacks[ind].position, Quaternion.identity, newCardsHolder.GetChild(i));
            newCard.transform.localScale = new Vector2(0.7f, 0.7f);
            newCard.GetComponent<UpdateCard>().InitValues(pickedCards[i]);
            soundManager.PlaySound(0);
            yield return AnimUtils.TweenPos(newCard.transform, Vector2.zero, 0.5f, AnimUtils.QuintOut);
        }

        canClick = true;
    }

    public void UpdateVisuals() {
        creditsText.text = "x" + StaticData.credits;

        if (!cardsDrawn) {
            newCardsText.SetActive(false);
        }

        if (StaticData.bossesBeat == 0) {
            cardPacks[2].GetComponentInChildren<Image>().sprite = packSprites[4];
            cardPacks[3].GetComponentInChildren<Image>().sprite = packSprites[4];

            cardPacks[2].GetComponentInChildren<TMP_Text>().text = "???";
            cardPacks[3].GetComponentInChildren<TMP_Text>().text = "???";
        } else if (StaticData.bossesBeat == 1) {
            cardPacks[2].GetComponentInChildren<Image>().sprite = packSprites[2];
            cardPacks[3].GetComponentInChildren<Image>().sprite = packSprites[4];

            cardPacks[2].GetComponentInChildren<TMP_Text>().text = "Linear Algebra Pack";
            cardPacks[3].GetComponentInChildren<TMP_Text>().text = "???";
        } else {
            cardPacks[2].GetComponentInChildren<Image>().sprite = packSprites[2];
            cardPacks[3].GetComponentInChildren<Image>().sprite = packSprites[3];

            cardPacks[2].GetComponentInChildren<TMP_Text>().text = "Linear Algebra Pack";
            cardPacks[3].GetComponentInChildren<TMP_Text>().text = "Pathfinding Pack";
        }
    }

    public void DestroyCards() {
        if (cardsDrawn) {
            soundManager.PlaySound(2);
            for (int i = 0; i < 3; i++) {
                StartCoroutine(DestroyCard(newCardsHolder.GetChild(i).GetChild(0).gameObject));
            }
        }
    }

    public void OnPackHover(int ind) {
        if (ind > StaticData.bossesBeat + 1) return;

        if (packEnumerators[ind] != null) {
            StopCoroutine(packEnumerators[ind]);
        }
        packEnumerators[ind] = AnimUtils.TweenScale(cardPacks[ind].GetChild(0), new Vector2(1.1f, 1.1f), 0.2f, AnimUtils.CubicOut);
        StartCoroutine(packEnumerators[ind]);
    }

    public void OnPackUnHover(int ind) {
        if (ind > StaticData.bossesBeat + 1) return;

        if (packEnumerators[ind] != null) {
            StopCoroutine(packEnumerators[ind]);
        }
        packEnumerators[ind] = AnimUtils.TweenScale(cardPacks[ind].GetChild(0), Vector2.one, 0.2f, AnimUtils.CubicOut);
        StartCoroutine(packEnumerators[ind]);
    }

    // Animate card death
    private IEnumerator DestroyCard(GameObject card) {
        StartCoroutine(Shake(card.transform, 2, 0));
        yield return AnimUtils.TweenScale(card.transform, new Vector2(0.01f, 0.01f), 0.7f, AnimUtils.ElasticInOut);
        Destroy(card);
    }

    public IEnumerator Shake(Transform trans, int shakes, float centerAngle, float shakeDelay = 0.1f) {
        for ( int i = 0; i < shakes; i++)
        {
            StartCoroutine(AnimUtils.TweenRotZ(trans, centerAngle + 10f, shakeDelay, AnimUtils.CubicOut));
            yield return new WaitForSeconds(shakeDelay);
            StartCoroutine(AnimUtils.TweenRotZ(trans, centerAngle - 10f, shakeDelay, AnimUtils.CubicOut));
            yield return new WaitForSeconds(shakeDelay);
        }
        StartCoroutine(AnimUtils.TweenRotZ(trans, centerAngle, shakeDelay, AnimUtils.CubicOut));
    }
}
