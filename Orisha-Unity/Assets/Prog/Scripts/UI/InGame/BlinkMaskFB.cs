/*
*@ ArthurLacour
*@ BlinkMaskFB.cs
*@ Date: 23/05/2017
*@ Description: Script which blink the button for the FB controls
*/

using UnityEngine;
using UnityEngine.UI;
using vd_Inputs;

public class BlinkMaskFB : MonoBehaviour {

    [SerializeField] Image imageUI;
    [SerializeField] Sprite spriteKeyboard;
    [SerializeField] Sprite spriteGamepad;

    private void OnEnable()
    {
        imageUI.color = new Color(imageUI.color.r, imageUI.color.g, imageUI.color.b, 0.0f);
    }

    // Update is called once per frame
    void Update () {
        imageUI.color = new Color(imageUI.color.r, imageUI.color.g, imageUI.color.b, Mathf.PingPong(Time.time, 1.0f));
        UpdateInputModeFeedback();
    }

    void UpdateInputModeFeedback()
    {
        switch (InputManager.GetInputMode)
        {
            case InputMode.keyboard:
                imageUI.sprite = spriteKeyboard;
                break;
            case InputMode.joy:
                imageUI.sprite = spriteGamepad;
                break;
            default:
                Debug.LogError("Error: unknown input mode");
                break;

        }
    }
}
