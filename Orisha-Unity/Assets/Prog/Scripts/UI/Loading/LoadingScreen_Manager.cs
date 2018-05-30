/*
*@ Creator: Romain SEUROT
*@ Worked on it: Romain SEUROT, 
*@ Loading screen
*@ Date: 16/10/2017
*@ Description: This script is running in the loading scene
*               It goal is to load the next scene in a coroutine
*               and show the percentage of resources already load
*@ Last Modification: 25/10/2017
*@ By: Romain Seurot
*/

using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LoadingScreen_Manager : MonoBehaviour
{
    private string sceneToLoad; // name of the scene to load
    public string SceneToLoad
    {
        get
        {
            return sceneToLoad;
        }

        set
        {
            sceneToLoad = value;
        }
    }

    [SerializeField]
    private Canvas LoadingCanvas;
    [SerializeField]
    private Text LoadingText;
    private int numberPointToLoadingText; // keep in memory the number of points in the loading text;
    [SerializeField]
    private Image Loading_IncreaseField; // move with the load progress
    private RectTransform Loading_IncreaseField_Rect;
    private float CurrentIncrease_Width;

    [SerializeField]
    private Image Loading_IncreaseFieldCadre; // doesn't move, it's just the cadre
    private RectTransform Loading_IncreaseFieldCadre_Rect;
    private float cadre_Width;

    private float WidthRatio; // rate IncreaseFieldWidth / IncreaseFieldWidth_Cadre

    private float TimerLoading; // this timer is used for the points after the string "Loading" 
    [SerializeField]
    private float MaxTimerLoading;

    private bool isLoadingFinish; // is the loading done??
    private bool isBeginFadeFinished; // iss the opening scene black fade finished?

    [SerializeField]
    private Image BlackScreen_Image; // used for the black fade
    private float BlackScreen_Image_alphaValue;
    private float timer_fadeBlack;
    [SerializeField]
    private float MaxTimer_fadeBlack;
    [SerializeField]
    private float Timer_waitAfterLoad_BeforeFade;

    private AsyncOperation async;

    void Start()
    {
        sceneToLoad = GameLoopManager.GetLevelToLoad();
        

        timer_fadeBlack = MaxTimer_fadeBlack;
        BlackScreen_Image_alphaValue = 1.0f;
        BlackScreen_Image.color = new Color(BlackScreen_Image.color.r, BlackScreen_Image.color.g, BlackScreen_Image.color.b, BlackScreen_Image_alphaValue);
        isLoadingFinish = false;
        isBeginFadeFinished = false;
        TimerLoading = 0.0f;
        numberPointToLoadingText = 0;
        Loading_IncreaseFieldCadre_Rect = Loading_IncreaseFieldCadre.gameObject.GetComponent<RectTransform>();
        cadre_Width = Loading_IncreaseFieldCadre_Rect.rect.width;
        CurrentIncrease_Width = 1.0f;
        Loading_IncreaseField_Rect = Loading_IncreaseField.gameObject.GetComponent<RectTransform>();

        Loading_IncreaseField_Rect.sizeDelta = new Vector2(CurrentIncrease_Width, Loading_IncreaseField_Rect.sizeDelta.y);

    }

    void Update()
    {
        TimerLoading += Time.deltaTime;
        if (TimerLoading >= MaxTimerLoading) // increase the number of points with the time after the word "loading"
        {
            numberPointToLoadingText = (numberPointToLoadingText == 3) ? 0 : (numberPointToLoadingText + 1);
            LoadingText.text = "Loading ";
            for (int i = 0; i < numberPointToLoadingText; i++)
            {
                LoadingText.text += ".";
            }

            TimerLoading = 0.0f;
        }

        if (!isBeginFadeFinished) // Opening scene black fade
        {
            timer_fadeBlack -= Time.deltaTime;
            if (timer_fadeBlack <= 0.0f)
            {
                isBeginFadeFinished = true;
                BlackScreen_Image_alphaValue = 0.0f;
                timer_fadeBlack = 0.0f - Timer_waitAfterLoad_BeforeFade;
                Debug.Log("StartLoadingScene");
                StartLoadingScene(); // at the end of the fade => lauch the coroutine of loading
            }
            else
            {
                BlackScreen_Image_alphaValue = (timer_fadeBlack / MaxTimer_fadeBlack);
            }
            BlackScreen_Image.color = new Color(BlackScreen_Image.color.r, BlackScreen_Image.color.g, BlackScreen_Image.color.b, BlackScreen_Image_alphaValue);
        }
        else if (isLoadingFinish)// Closing black screen fade
        {
            timer_fadeBlack += Time.deltaTime;
            if (timer_fadeBlack >= MaxTimer_fadeBlack)
            {
                isBeginFadeFinished = true;
                BlackScreen_Image_alphaValue = 1.0f;
                async.allowSceneActivation = true; // change scene to sceneToLoad
            }
            else
            {
                BlackScreen_Image_alphaValue = (timer_fadeBlack / MaxTimer_fadeBlack);
            }
            BlackScreen_Image.color = new Color(BlackScreen_Image.color.r, BlackScreen_Image.color.g, BlackScreen_Image.color.b, BlackScreen_Image_alphaValue);
        }

    }

    private void StartLoadingScene()
    {
        StartCoroutine(LoadNewScene());
    }

    IEnumerator LoadNewScene()
    {
        async = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Single);
        async.allowSceneActivation = false;

        while (!async.isDone)
        {
            WidthRatio = async.progress;
            CurrentIncrease_Width = (cadre_Width * WidthRatio);

            if (async.progress >= 0.9f)
            {
                CurrentIncrease_Width = cadre_Width;
                WidthRatio = 1.0f;
                isLoadingFinish = true;
            }
            Loading_IncreaseField_Rect.sizeDelta = new Vector2(CurrentIncrease_Width * 0.92f, Loading_IncreaseField_Rect.sizeDelta.y);

            yield return null;
        }
    }
}
