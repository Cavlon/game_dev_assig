using System.Collections;
using UnityEngine;
using CavlonUtils;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    private Transform screenWipe;
    private IEnumerator wipeEnumerator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        screenWipe = transform.GetChild(0).GetChild(0);
        StartCoroutine(StartScene());
    }

    private IEnumerator StartScene() {
        wipeEnumerator = AnimUtils.TweenPos(screenWipe, new Vector2(0, 1147), 1.5f, AnimUtils.QuintInOut);
        yield return wipeEnumerator;
        screenWipe.gameObject.SetActive(false);
    }

    public IEnumerator ChangeScene(string sceneName) {
        StopCoroutine(wipeEnumerator);

        screenWipe.gameObject.SetActive(true);

        wipeEnumerator = AnimUtils.TweenPos(screenWipe, Vector2.zero, 1.5f, AnimUtils.QuintInOut);
        yield return wipeEnumerator;

        AsyncOperation asyncSceneLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncSceneLoad.isDone){
            Debug.Log("Loading the Scene"); 
            yield return null;
        }
    }
}
