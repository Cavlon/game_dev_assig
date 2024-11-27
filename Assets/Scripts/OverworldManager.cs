using UnityEngine;
using CavlonUtils;
using System.Collections;
using TMPro;

public class OverworldManager : MonoBehaviour
{

    private GameObject pauseMenu;
    [SerializeField]
    public CardData[] startingDeck = new CardData[20];
    [SerializeField]
    private GameObject[] invisWalls = new GameObject[2];

    [SerializeField]
    private DialogueText bossBeatDialogue;
    [SerializeField]
    private DialogueText startDialogue;

    private RectTransform pauseBar;
    private GameObject pauseOverlay;
    private TMP_Text creditsText;

    private InventoryManager inventoryMenu;

    private DialogueManager dialogueManager;

    public static bool paused = false;
    public bool canPause = false;
    private bool invOpen = false;


    private IEnumerator pauseMenuEnumerator;
    private IEnumerator invMenuEnumerator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        creditsText = transform.Find("HUD").Find("Credits").Find("CreditsText").GetComponent<TMP_Text>();
        UpdateHUD();

        pauseMenu = transform.Find("PauseMenu").gameObject;
        canPause = false;
        pauseMenu.SetActive(false);
        pauseOverlay = pauseMenu.transform.GetChild(0).gameObject;
        pauseBar = (RectTransform)pauseMenu.transform.GetChild(1);

        inventoryMenu = pauseMenu.transform.Find("InventoryMenu").GetComponent<InventoryManager>();
        inventoryMenu.gameObject.SetActive(false);

        dialogueManager = GameObject.Find("/DialogueManager").GetComponent<DialogueManager>();

        DestroyBarriers();

        if (StaticData.firstLoad) {
            for (int i = 0; i < 20; i++) {
                StaticData.AddCardToInventory(startingDeck[i]);
                StaticData.deck.Add(startingDeck[i]); 
                StaticData.AddCardToDeck(startingDeck[i]);
            }

            dialogueManager.StartDialogue(null, startDialogue, null);
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (!invOpen) {
                if (paused) {
                    ResumeGame();
                } else if (canPause) {
                    PauseGame();
                }
            } else {
                CloseInventory();
            }           
        }
    }

    public void UpdateHUD() {
        creditsText.text = "x" + StaticData.credits;
    }

    public void PauseGame() {
        pauseMenu.SetActive(true);
        pauseOverlay.SetActive(true);
        Time.timeScale = 0f;
        paused = true;
        if (pauseMenuEnumerator != null) {
            StopCoroutine(pauseMenuEnumerator);
        }
        pauseMenuEnumerator = AnimUtils.TweenRectPos(pauseBar, new Vector2(195, 0), 0.5f, AnimUtils.QuintOut);
        StartCoroutine(pauseMenuEnumerator);
    }

    public void ResumeGame() {
        StartCoroutine(ResumeGameEnumerator());        
    }

    private IEnumerator ResumeGameEnumerator() {
        Time.timeScale = 1f;
        pauseOverlay.SetActive(false);
        paused = false;
        if (pauseMenuEnumerator != null) {
            StopCoroutine(pauseMenuEnumerator);
        }
        pauseMenuEnumerator = AnimUtils.TweenRectPos(pauseBar, new Vector2(-468, 0), 0.5f, AnimUtils.QuintOut);
        yield return pauseMenuEnumerator;
        pauseMenu.SetActive(false);
    }

    public void OpenInventory() {
        inventoryMenu.gameObject.SetActive(true);
        invOpen = true;
        inventoryMenu.UpdateInventory();
        inventoryMenu.UpdateDeck();

        if (invMenuEnumerator != null) {
            StopCoroutine(invMenuEnumerator);
        }
        invMenuEnumerator = AnimUtils.UnTimeScaledTweenPos(inventoryMenu.transform, Vector2.zero, 0.8f, AnimUtils.QuintOut);
        StartCoroutine(invMenuEnumerator);
    }

    public void CloseInventory() {
        if (StaticData.deck.Count < 20) {
            inventoryMenu.ShakeDeck();
            return;
        }
        StartCoroutine(CloseInventoryEnumerator());
    }

    private IEnumerator CloseInventoryEnumerator() {
        if (invMenuEnumerator != null) {
            StopCoroutine(invMenuEnumerator);
        }
        invMenuEnumerator = AnimUtils.UnTimeScaledTweenPos(inventoryMenu.transform, new Vector2(-2171, 0), 0.8f, AnimUtils.QuintOut);
        yield return invMenuEnumerator;
        invOpen = false;
        inventoryMenu.gameObject.SetActive(false);
    }

    public void BossJustBeat() {
        if (StaticData.bossesBeat == 3) return;
        dialogueManager.StartDialogue(null, bossBeatDialogue, null);
        DestroyBarriers();
    }

    private void DestroyBarriers() {
        if (StaticData.bossesBeat > 0) {
            Destroy(invisWalls[0]);
            if (StaticData.bossesBeat > 1) {
                Destroy(invisWalls[1]);
            }
        }
    }
}
