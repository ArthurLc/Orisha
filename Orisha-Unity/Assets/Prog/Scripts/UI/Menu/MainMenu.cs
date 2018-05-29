/* MainMenu.cs
* ancien "CharacterSelection" : script contenant ce qui est utile au menu principal
* Objectifs : 
    - Lancement d'une partie
    - Affichage des crédits
    - Affichage des options
    - Quitter le jeu

* Autre :
    - Affichage du mode d'inputs actuel


* Crée par Ambre LACOUR le 08/10/2017
* Dernière modification par Ambre LACOUR le 15/01/2017 

*/

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using vd_Inputs;

namespace vd_Menu
{
    public class MainMenu : MonoBehaviour
    {
        [Header("Level")]
        [SerializeField] private string level;

        [Header("Mode d'inputs")]
        [SerializeField] private Sprite inputIconJoystick;
        [SerializeField] private Sprite inputIconKeyboard;
        [SerializeField] private Image currentInputMode;

        [Header("Cursor")]
        [SerializeField] private Texture2D cursorTexture;
        [SerializeField] private Vector2 hotSpot = Vector2.zero;

        [Header("Cosmetic")]
        [SerializeField] private Image backgroundTransition;
        [SerializeField] private float backgroundTransitionDuration = 1.0f;


        private void Start()
        {
            backgroundTransition.color = Color.black;
            Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);

            StartCoroutine(BackgroundTransition());
        }

        private void OnApplicationQuit()
        {
            StopAllCoroutines();
        }

        void Update()
        {
            UpdateInputModeFeedback();
        }

        void UpdateInputModeFeedback()
        {
            switch (InputManager.GetInputMode)
            {
                case InputMode.keyboard:
                    currentInputMode.sprite = inputIconKeyboard;
                    break;
                case InputMode.joy:
                    currentInputMode.sprite = inputIconJoystick;
                    break;
                default:
                    Debug.LogError("Error: unknown input mode");
                    break;

            }
        }

        /// <summary>
        /// Launch the level
        /// </summary>
        public void LaunchGame()
        {
            GameLoopManager.LaunchLevel(level, true);
			SoundManager.instance.SFX_PlayOneShot (SoundManager.instance.audioList.MainMenuSound [0]);
        }

        /// <summary>
        /// Quit the game
        /// </summary>
        public void QuitGame()
        {
            Application.Quit();
        }

        IEnumerator BackgroundTransition()
        {
            float timer = backgroundTransitionDuration;

            while (timer > 0.0f)
            {
                timer -= Time.fixedDeltaTime;
                backgroundTransition.color = new Color(backgroundTransition.color.r, backgroundTransition.color.g, backgroundTransition.color.b, timer / backgroundTransitionDuration);
                yield return null;
            }

            backgroundTransition.color = new Color(backgroundTransition.color.r, backgroundTransition.color.g, backgroundTransition.color.b, 0.0f);
            yield return null;
        }
    }
}
