using UnityEngine;
using CavlonUtils;
using System.Collections;
using UnityEngine.PlayerLoop;

public class OverworldManager : MonoBehaviour
{

    [SerializeField]
    private GameObject pauseMenu;
    [SerializeField]
    public CardData[] startingDeck = new CardData[20];

    private RectTransform pauseBar;
    private GameObject pauseOverlay;

    public static bool paused = false;


    private IEnumerator pauseMenuEnumerator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pauseMenu.SetActive(false);
        pauseOverlay = pauseMenu.transform.GetChild(0).gameObject;
        pauseBar = (RectTransform)pauseMenu.transform.GetChild(1);

        if (StaticData.deck[0] == null) {
            for (int i = 0; i < 20; i++) {
                StaticData.deck[i] = startingDeck[i];
            }
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (paused) {
                ResumeGame();
            } else {
                PauseGame();
            }
        }
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
}
