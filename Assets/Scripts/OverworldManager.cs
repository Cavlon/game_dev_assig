using UnityEngine;
using CavlonUtils;
using System.Collections;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.UI;

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

    [SerializeField]
    private GameObject NPCHolder;

    private RectTransform pauseBar;
    private GameObject pauseOverlay;
    private GameObject detailToggle;
    private TMP_Text creditsText;

    private InventoryManager inventoryMenu;

    private DialogueManager dialogueManager;
    private SceneLoader sceneLoader;
    private SoundManager soundManager;
    private AudioSource bgm;

    public static bool paused = false;
    public bool canPause = false;
    private bool invOpen = false;


    private IEnumerator pauseMenuEnumerator;
    private IEnumerator invMenuEnumerator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sceneLoader = GameObject.Find("/SceneLoader").GetComponent<SceneLoader>();

        soundManager = GameObject.Find("/SoundManager").GetComponent<SoundManager>();
        bgm = GameObject.Find("/BGM").GetComponent<AudioSource>();

        creditsText = transform.Find("HUD").Find("Credits").Find("CreditsText").GetComponent<TMP_Text>();
        UpdateHUD();

        pauseMenu = transform.Find("PauseMenu").gameObject;
        canPause = false;
        paused = false;
        pauseMenu.SetActive(false);
        pauseOverlay = pauseMenu.transform.GetChild(0).gameObject;
        detailToggle = pauseMenu.transform.Find("DetailToggle").gameObject;
        pauseBar = (RectTransform)pauseMenu.transform.GetChild(1);

        inventoryMenu = pauseMenu.transform.Find("InventoryMenu").GetComponent<InventoryManager>();
        inventoryMenu.gameObject.SetActive(false);

        dialogueManager = GameObject.Find("/DialogueManager").GetComponent<DialogueManager>();

        DestroyBarriers();

        if (StaticData.lowDetail) {
            NPCHolder.SetActive(false);
            QualitySettings.SetQualityLevel(1, true);
        } else {
            NPCHolder.SetActive(true);
            QualitySettings.SetQualityLevel(0, true);
        }

        Toggle toggle = detailToggle.GetComponent<Toggle>();

        toggle.isOn = StaticData.lowDetail;

        toggle.onValueChanged.AddListener(delegate {ToggleLowDetail(toggle);});

        if (StaticData.firstLoad) {
            for (int i = 0; i < 20; i++) {
                StaticData.AddCardToInventory(startingDeck[i]);
                StaticData.deck.Add(startingDeck[i]); 
                StaticData.AddCardToDeck(startingDeck[i]);
            }

            dialogueManager.StartDialogue(null, startDialogue, null);
            StaticData.firstLoad = false;
        }

        Invoke("SetPause", 1.5f);
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
        Debug.Log("Pausing");
        pauseMenu.SetActive(true);
        pauseOverlay.SetActive(true);
        detailToggle.SetActive(true);
        soundManager.PlaySound(2);
        Time.timeScale = 0f;
        paused = true;
        bgm.volume = 0.1f;
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
        detailToggle.SetActive(false);
        soundManager.PlaySound(2);
        bgm.volume = 0.3f;
        paused = false;
        if (pauseMenuEnumerator != null) {
            StopCoroutine(pauseMenuEnumerator);
        }
        pauseMenuEnumerator = AnimUtils.TweenRectPos(pauseBar, new Vector2(-468, 0), 0.5f, AnimUtils.QuintOut);
        yield return pauseMenuEnumerator;
        pauseMenu.SetActive(false);
    }

    public void ExitGame() {
        soundManager.PlaySound(2);
        Time.timeScale = 1f;
        StaticData.playerPos = GameObject.Find("/Player").transform.position;
        GameObject.Find("/GameManager").GetComponent<OverworldManager>().canPause = false;
        StartCoroutine(sceneLoader.ChangeScene("MainMenu"));
    }

    public void OpenInventory() {
        inventoryMenu.gameObject.SetActive(true);
        invOpen = true;
        inventoryMenu.UpdateInventory();
        inventoryMenu.UpdateDeck();
        detailToggle.SetActive(false);
        soundManager.PlaySound(2);

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
        detailToggle.SetActive(true);
        soundManager.PlaySound(2);
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

    public void ToggleLowDetail(Toggle toggle) {
        Debug.Log("Toggling Detail");
        soundManager.PlaySound(1);
        if (toggle.isOn) {
            NPCHolder.SetActive(false);
            QualitySettings.SetQualityLevel(1, true);
            StaticData.lowDetail = true;
        } else {
            NPCHolder.SetActive(true);
            QualitySettings.SetQualityLevel(0, true);
            StaticData.lowDetail = false;
        }
    }

    private void SetPause() {
        canPause = true;
    }
}
