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
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class OpeningManager : MonoBehaviour {

    [SerializeField] private Image fadeBG;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private string sceneToLoadAfter = "MainMenu";

    private bool isLoadingFinish; // is the loading done??
    private bool isLaunchReadyToStart; // is the launch ready to start??
    private bool isFadeStart; // is the fadeToLoad started ??

    private AsyncOperation async;

    private void Start()
    {
        isLoadingFinish = false;
        isFadeStart = false;

        fadeBG.color = Color.clear;

        StartCoroutine(LoadNewScene());
    }
    private void OnApplicationQuit()
    {
        StopAllCoroutines();
    }

    // Update is called once per frame
    void Update () {
		if(isLoadingFinish)
        {
            if (Input.GetButtonDown(vd_Inputs.InputManager.Pause) || (videoPlayer.frame == (long)videoPlayer.frameCount)) {
                isLaunchReadyToStart = true;
            }

            if ((isFadeStart == false) && isLaunchReadyToStart)
            {
                isFadeStart = true;

                StopAllCoroutines();
                StartCoroutine(FadeToLaunchNewScene());
            }
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
    IEnumerator FadeToLaunchNewScene()
    {
        float alphaLevel = fadeBG.color.a;

        while (fadeBG.color.a < 1.0f)
        {
            alphaLevel += Time.fixedDeltaTime;
            fadeBG.color = new Color(fadeBG.color.r, fadeBG.color.g, fadeBG.color.b, alphaLevel);
            videoPlayer.SetDirectAudioVolume(0, 1.0f - alphaLevel);
            yield return null;
        }

        async.allowSceneActivation = true; // change scene to sceneToLoad
        yield return null;
    }
}
