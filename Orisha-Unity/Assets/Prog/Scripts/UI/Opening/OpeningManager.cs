/*
* @ArthurLacour
* @OpeningManager.cs
* @22/05/2018
* @ - Le Script s'attache sur un GameObject.
*   
* Contient :
*   - Le management de la scène Opening. (Lancement du menu qui suit, etc...)
*/

using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class OpeningManager : MonoBehaviour {

    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private string sceneToLoadAfter = "MainMenu";

    private bool isLoadingFinish; // is the loading done??

    private AsyncOperation async;

    private void Start()
    {
        isLoadingFinish = false;
        StartCoroutine(LoadNewScene());
    }
    private void OnApplicationQuit()
    {
        StopAllCoroutines();
    }

    // Update is called once per frame
    void Update () {
		if(isLoadingFinish && videoPlayer.frame == (long)videoPlayer.frameCount)
        {
            StopAllCoroutines();
            async.allowSceneActivation = true; // change scene to sceneToLoad
        }
	}

    IEnumerator LoadNewScene()
    {
        async = SceneManager.LoadSceneAsync(sceneToLoadAfter, LoadSceneMode.Single);
        async.allowSceneActivation = false;

        while (!async.isDone)
        {
            if (async.progress >= 0.9f)
            {
                isLoadingFinish = true;
            }

            yield return null;
        }
    }
}
