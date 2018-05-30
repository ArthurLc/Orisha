/*
* @ArthurLacour
* @InputMode_ImageSwapper.cs
* @23/05/2018
* @ - Le Script s'attache à une Image
*   
*   Change l'image selon le mode d'input
*/

using UnityEngine;
using UnityEngine.UI;
using vd_Inputs;

public class InputMode_ImageSwapper : MonoBehaviour
{

    [Header("Links")]
    [SerializeField] private Image image;
    [SerializeField] private Sprite spriteKeyboard;
    [SerializeField] private Sprite spriteGamepad;

    // Update is called once per frame
    void Update()
    {
        UpdateInputModeFeedback();
    }

    void UpdateInputModeFeedback()
    {
        switch (InputManager.GetInputMode)
        {
            case InputMode.keyboard:
                image.sprite = spriteKeyboard;
                break;
            case InputMode.joy:
                image.sprite = spriteGamepad;
                break;
            default:
                Debug.LogError("Error: unknown input mode");
                break;

        }
    }
}
