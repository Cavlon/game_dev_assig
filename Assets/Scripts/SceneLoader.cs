using System.Collections;
using UnityEngine;
using CavlonUtils;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    private Transform screenWipe;
    private OverworldManager overworldManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        screenWipe = transform.GetChild(0).GetChild(0);
        if (SceneManager.GetActiveScene().name == "Overworld") {
            overworldManager = GameObject.Find("/GameManager").GetComponent<OverworldManager>();
        }   
        StartCoroutine(StartScene());
    }

    private IEnumerator StartScene() {
        yield return AnimUtils.TweenPos(screenWipe, new Vector2(0, 1147), 1.5f, AnimUtils.QuintInOut);
        screenWipe.gameObject.SetActive(false);
        if (overworldManager != null) {
            overworldManager.canPause = true;
        }   
    }

    public IEnumerator ChangeScene(string sceneName) {
        StaticData.firstLoad = false;
        if (overworldManager != null) {
            overworldManager.canPause = false;
        }   
        screenWipe.gameObject.SetActive(true);
        yield return AnimUtils.TweenPos(screenWipe, Vector2.zero, 1.5f, AnimUtils.QuintInOut);
        AsyncOperation asyncSceneLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncSceneLoad.isDone){
            Debug.Log("Loading the Scene"); 
            yield return null;
        }
    }
}
