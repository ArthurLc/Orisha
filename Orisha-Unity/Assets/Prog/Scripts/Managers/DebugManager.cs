/* DebugManager.cs
 * Fonctions pour nous faciliter la vie dans la prod
 * 
 * Fait : 
 * - Changement du mode d'inputs en cours de partie
 * 
 * - Lancement immédiat d'une partie avec le persoA et une IA =>inputs manette
 * - Lancement immédiat d'une partie avec le persoA et une IA =>inputs clavier
 * 
 * - Retour rapide au menu
 * 
 * - Changement de langue
 * 
 * - Invocations : kill/damage
 * 
 * - Cameras :
 *          - unit/split
 *          - switch entre auto/free/focus
 * 
 * A faire :
 * - 
 * 
 * 
 * Crée par Ambre LACOUR le 16/10/2017
 * Dernière modification par Ambre le 27/11/2017 (ajout des inputs debug camera) 
 */

using UnityEngine;
using UnityEngine.SceneManagement;
using vd_Player;


namespace vd_Debug
{

    public class DebugManager : MonoBehaviour
    {
		private static DebugManager instance;
        public CharacterInitialization player1;

        [Header("Changement d'Inputs")]
        public vd_Inputs.InputMode modePlayer1;
        public bool change = false;

        [Header("Lancement rapide d'une partie")]
        [SerializeField] private string levelToLaunch;
        [SerializeField] private KeyCode launchGameKeyboard = KeyCode.Keypad1;

        [Header("Retour menu rapide")]
        [SerializeField] private string menuToLaunch = "Menu";
        [SerializeField] private KeyCode backMenuKeyboard = KeyCode.Keypad0;

        [Header("Changement de langue")]
        [SerializeField] private KeyCode switchLangage = KeyCode.KeypadPlus;

        [Header("Gamepads")]
        [SerializeField] private KeyCode vibrateGamepadJ1 = KeyCode.F1;
        [SerializeField] private KeyCode vibrateGamepadJ2 = KeyCode.F2;

        void Start()
        {
			if (instance != null) {
				Destroy (this);
			}
			instance = this;

            DontDestroyOnLoad(gameObject);
        }

        void Update()
        {
            //_______________PLAYER INIT___________________//
            if(change == true)
            {
                vd_Inputs.InputManager.newInputMode(modePlayer1);
                change = false;
            }

       
            // Lancement d'une partie avec contrôle clavier du persoA et une IA
            if (Input.GetKeyDown(launchGameKeyboard))
            {
                vd_Inputs.InputManager.newInputMode(vd_Inputs.InputMode.keyboard);
                Debug.Log("DebugManager: " + levelToLaunch + " chargé avec J1 clavier et j2 IA");

                GameLoopManager.LaunchLevel(levelToLaunch, false);
            }

            // Retour au menu
            else if (Input.GetKeyDown(backMenuKeyboard))
            {
                Debug.Log("DebugManager: " + menuToLaunch + " chargé");
                SceneManager.LoadScene(menuToLaunch);
            }


            // Changement de la langue
            else if(Input.GetKeyDown(switchLangage))
            {
                if (GameLoopManager.CurrentLangage == "EN")
                    GameLoopManager.CurrentLangage = "FR";
                else
                    GameLoopManager.CurrentLangage = "EN";

                Debug.Log("DebugManager: changement de langue");
            }


            //_____________________GAMEPAD___________________________//
            else if (Input.GetKeyDown(vibrateGamepadJ1))
            {
                vd_Inputs.InputManager.GamePad_StartVibration(0, 0.5f, 0.5f, 0.5f);
            }
            else if (Input.GetKeyDown(vibrateGamepadJ2))
            {
                vd_Inputs.InputManager.GamePad_StartVibration(1, 0.5f, 0.5f, 0.5f);
            }
        }
    }

}
