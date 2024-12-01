using System.Collections;
using CavlonUtils;
using UnityEngine;

public class MainMenu : MonoBehaviour
{

    private SceneLoader sceneLoader;
    private SoundManager soundManager;

    [SerializeField]
    private GameObject[] tabs = new GameObject[2];

    private IEnumerator[] tabEnumerators = new IEnumerator[2];

    private bool canClick = true;
    private int currentTab = -1;

    void Start() {
        sceneLoader = GameObject.Find("/SceneLoader").GetComponent<SceneLoader>();
        soundManager = GameObject.Find("/SoundManager").GetComponent<SoundManager>();
    }

    void Update() {
        if (currentTab != -1 && Input.GetKeyDown(KeyCode.Escape)) {
            CloseTab(currentTab);
        }
    }

    public void StartGame() {
        if (!canClick) return;
        canClick = false;
        Debug.Log("Starting Game");
        soundManager.PlaySound(2);
        StartCoroutine(sceneLoader.ChangeScene("Overworld"));
    }

    public void OpenTab(int tabId) {
        if (!canClick) return;
        canClick = false;
        soundManager.PlaySound(2);
        if (tabEnumerators[tabId] != null) {
            StopCoroutine(tabEnumerators[tabId]);
        }
        tabEnumerators[tabId] = AnimUtils.TweenRectPos((RectTransform)tabs[tabId].transform, Vector2.zero, 1f, AnimUtils.QuintOut);
        StartCoroutine(tabEnumerators[tabId]);
        currentTab = tabId;
    }

    public void CloseTab(int tabId) {
        currentTab = -1;
        canClick = true;
        soundManager.PlaySound(2);
        if (tabEnumerators[tabId] != null) {
            StopCoroutine(tabEnumerators[tabId]);
        }
        tabEnumerators[tabId] = AnimUtils.TweenRectPos((RectTransform)tabs[tabId].transform, new Vector2(1287, 0), 1f, AnimUtils.QuintOut);
        StartCoroutine(tabEnumerators[tabId]);
    }

    public void ExitGame() {
        if (!canClick) return;
        soundManager.PlaySound(2);
        Debug.Log("Exiting Game");
        Application.Quit();
    }
}
