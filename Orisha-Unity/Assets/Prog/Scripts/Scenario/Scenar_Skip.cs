/*
* @ArthurLacour
* @Scenar_Skip.cs
* @23/05/2018
* @ - Le Script s'attache à une cinematic
*   
*   Y ajoute un bouton de skip
*/

using UnityEngine;
using UnityEngine.UI;
using vd_Inputs;

public class Scenar_Skip : MonoBehaviour {

    [Header("Links")]
    [SerializeField] private UnityEngine.Playables.PlayableDirector playableDirector;
    [SerializeField] private ZoneDiscovery zoneDiscovery;
    [SerializeField] private Cinemachine.CinemachineVirtualCamera vCam1;
    [SerializeField] private Image imageFilled;
    [SerializeField] private Image imageButtonFB;
    [SerializeField] private Sprite spriteKeyboard;
    [SerializeField] private Sprite spriteGamepad;

    float fillAmoutCounter;

    private void Start()
    {
        this.enabled = false;
    }

    private void OnEnable()
    {
        imageFilled.fillAmount = 0.0f;
        fillAmoutCounter = 0.0f;
        imageButtonFB.color = new Color(imageButtonFB.color.r, imageButtonFB.color.g, imageButtonFB.color.b, 0.0f);
    }
    private void OnDisable()
    {
        imageFilled.fillAmount = 0.0f;
        fillAmoutCounter = 0.0f;
        imageButtonFB.color = new Color(imageButtonFB.color.r, imageButtonFB.color.g, imageButtonFB.color.b, 0.0f);
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetButton(vd_Inputs.InputManager.Cancel))
        {
            if(fillAmoutCounter < 1.0f)
            {
                fillAmoutCounter += Time.deltaTime;
                imageFilled.fillAmount = fillAmoutCounter;
            }
            else
            {
                imageFilled.fillAmount = 1.0f;
                if(zoneDiscovery != null)
                    zoneDiscovery.isBeginDisplay = true;
                playableDirector.Stop();
                vCam1.gameObject.SetActive(false);
                TimeManager.Instance.Unblock_Player();
                this.enabled = false;
            }

            imageButtonFB.color = new Color(imageButtonFB.color.r, imageButtonFB.color.g, imageButtonFB.color.b, 0.0f);
        }
        else
        {
            fillAmoutCounter = 0.0f;
            imageFilled.fillAmount = 0.0f;

            imageButtonFB.color = new Color(imageButtonFB.color.r, imageButtonFB.color.g, imageButtonFB.color.b, Mathf.PingPong(Time.time, 1.0f));
            UpdateInputModeFeedback();
        }

    }

    void UpdateInputModeFeedback()
    {
        switch (InputManager.GetInputMode)
        {
            case InputMode.keyboard:
                imageButtonFB.sprite = spriteKeyboard;
                break;
            case InputMode.joy:
                imageButtonFB.sprite = spriteGamepad;
                break;
            default:
                Debug.LogError("Error: unknown input mode");
                break;

        }
    }
}
