// PlayerInputs.cs
// Gestion du mode d'inputs des deux joueurs (clavier, manette)
// Objectif : 
// - Initialisation et update des inputs dans le menu de lancement
// - Quel que soit le mode d'inputs (manette, joystick), même nom d'input
// Crée par Ambre LACOUR le 08/10/2017
// Dernière modification par Ambre LACOUR le 23/11/2017 


// Utilisation, exemple : pour avoir l'axe horizontal du joueur 1 : Input.GetAxis(PlayerInputs.Horizontal(PlayerInputs.Player1))
// NB : la touche d'input controle a été remplacée par tab dans les inputs settings pour éviter les raccourcis claviers perraves unity



using UnityEngine;
using XInputDotNetPure; // Required in C# for GamePad vibration

public enum Player
{
    Player1,
    Player2,
    AI
};

namespace vd_Inputs
{


    public enum InputMode
    {
        none,
        keyboard,
        joy
    };


    public class InputManager : MonoBehaviour
    {
		private static InputManager instance;
        private static InputMode inputMode;
        /// <summary>
        /// Get the player's current controller (none, keyboard or joystick)
        /// </summary>
        public static InputMode GetInputMode
        {
            get { return inputMode; }
        }

        /// <summary>
        /// string array :name of the current controller
        /// </summary>
        private static string currentInputMode_Name;

        /// <summary>
        /// Delegate : ajouter des fonctions à newInputMode si elles doivent être appelés lorsque le mode d'inputs change
        /// </summary>
        /// <param name="_newMode"></param>
        /// <returns></returns>
        public delegate bool ChangeInputDelegate(InputMode _newMode);
        public static ChangeInputDelegate newInputMode;

        ///////Keys : ensembles des touches d'inputs

        // Déplacements : horizontal, vertical
        static string horizontal;
        public static string Horizontal
        {
                get { return horizontal; }
        }
        static string vertical;
        public static string Vertical
        {
            get { return vertical; }
        }

        // Déplacements : run, dash et jump
        static string run;
        public static string Run
        {
            get { return run; }
        }
        static string lockRun;
        public static string LockRun
        {
            get { return lockRun; }
        }
        static string dash;
        public static string Dash
        {
            get { return dash; }
        }
        static string jump;
        public static string Jump
        {
            get { return jump; }
        }
        // Camera : camX, camY, focus, reset
        static string camX;
        public static string CamX
        {
            get { return camX; }
        }
        static string camY;
        public static string CamY
        {
            get { return camY; }
        }
        static string focus;
        public static string Focus
        {
            get { return focus; }
        }
        static string resetCamera;
        public static string ResetCamera
        {
            get { return resetCamera; }
        }

        // Menu : confirm, cancel, launch, pause
        static string confirm;
        public static string Confirm
        {
            get { return confirm; }
        }
        static string cancel;
        public static string Cancel
        {
            get { return cancel; }
        }
        static string launch;
        public static string Launch
        {
            get { return launch; }
        }
        static string pause;
        public static string Pause
        {
            get { return pause; }
        }        

        // Attaques : attackHeavy, attackSlight
        static string attackHeavy;
        public static string AttackHeavy
        {
            get { return attackHeavy; }
        }
        static string attackSlight;
        public static string AttackSlight
        {
            get { return attackSlight; }
        }

        // Nécessaire pour les vribrations mannette
        int nbrPlayersSet;
        PlayerIndex[] playerIndex;
        GamePadState[] state;
        //GamePadState[] prevState;     Non utilisée ?

        static bool[] isPlayerVibrate;
        static float[] leftMotor;
        static float[] rightMotor;
        static float[] vibrDuration;

        

        void Start()
        {
			if (instance != null) {
				Destroy (this.gameObject);
			}
			instance = this;
            ChangeInputMode(InputMode.keyboard);
            
            InitGamePadFactors();
            DontDestroyOnLoad(gameObject);
        }

        private void FixedUpdate()
        {
            for (int indexPlayer = 0; indexPlayer < 2; indexPlayer++)
                if (isPlayerVibrate[indexPlayer] == true)
                {
                    if (vibrDuration[indexPlayer] <= 0.0f)
                    {
                        GamePad.SetVibration(playerIndex[indexPlayer], 0, 0);
                        isPlayerVibrate[indexPlayer] = false;
                    }
                    else
                    {
                        GamePad.SetVibration(playerIndex[indexPlayer], leftMotor[indexPlayer], rightMotor[indexPlayer]);
                        vibrDuration[indexPlayer] -= Time.deltaTime;
                    }
                }
        }
        private void Update()
        {
            // Update du mode d'inputs
            CheckForNewInputMode();

            // Trouve un PlayerIndex pour 2 joueurs
            if (nbrPlayersSet < 2)
            {
                for (int i = 0; i < 4; ++i)
                {
                    PlayerIndex testPlayerIndex = (PlayerIndex)i;
                    GamePadState testState = GamePad.GetState(testPlayerIndex);
                    if (testState.IsConnected)
                    {
                        //Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
                        playerIndex[nbrPlayersSet] = testPlayerIndex;
                        nbrPlayersSet += 1;
                    }
                }
            }

            // Actualisation des états des GamePad
            //prevState = state;
            state[0] = GamePad.GetState(playerIndex[0]);
            state[1] = GamePad.GetState(playerIndex[1]);
        }


        /// <summary>
        /// Change the player controller (keyboard, joystick1 or joystick2)
        /// </summary>
        /// <param name="_newMode"></param>
        /// <param name="_player"></param>
        private static bool ChangeInputMode(InputMode _newMode)
        {
            if (inputMode == _newMode)
                return false;

            inputMode = _newMode;
            switch (inputMode)
            {
                case InputMode.none:
                    currentInputMode_Name = "";

                    horizontal = "None";
                    vertical = "None";

                    run = "None";
                    lockRun = "None";
                    dash = "None";
                    jump = "None";

                    camX = "None";
                    camY = "None";
                    focus = "None";
                    resetCamera = "none";

                    confirm = "None";
                    cancel = "None";
                    launch = "None";
                    pause = "None";

                    attackHeavy = "None";
                    attackSlight = "None";

                    break;

                case InputMode.keyboard:
                    currentInputMode_Name = "Keyboard";

                    horizontal = "Keyboard_Horizontal";
                    vertical = "Keyboard_Vertical";

                    run = "Keyboard_Shift";
                    lockRun = "Keyboard_Maj";
                    dash = "Keyboard_Space";
                    jump = "Keyboard_E";

                    camX = GameLoopManager.camera_isXaxisInversed ? "Keyboard_CamX_Inverted" : "Keyboard_CamX";
                    camY = GameLoopManager.camera_isYaxisInversed ? "Keyboard_CamY_Inverted" : "Keyboard_CamY";
                    focus = "Keyboard_Ctrl";
                    resetCamera = "Keyboard_MouseMiddle";

                    confirm = "Keyboard_Space";
                    cancel = "Keyboard_Escape";
                    launch = "Keyboard_Return";
                    pause = "Keyboard_Escape";

                    attackHeavy = "Keyboard_MouseRight";
                    attackSlight = "Keyboard_MouseLeft";

                    break;

                case InputMode.joy:
                    currentInputMode_Name = Input.GetJoystickNames()[0];

                    horizontal = "Joy1_Horizontal";
                    vertical = "Joy1_Vertical";

                    run = "None";
                    lockRun = "None";
                    dash = "Joy1_B";
                    jump = "Joy1_A";

                    camX = GameLoopManager.camera_isXaxisInversed ? "Joy1_CamX_Inverted" : "Joy1_CamX";
                    camY = GameLoopManager.camera_isYaxisInversed ? "Joy1_CamY_Inverted" : "Joy1_CamY";
                    focus = "Joy1_LeftTrigger";
                    resetCamera = "Joy1_RightStickClick";


                    confirm = "Joy1_A";
                    cancel = "Joy1_B";
                    launch = "Joy1_Y";
                    pause = "Joy1_Start";

                    attackHeavy = "Joy1_Y";
                    attackSlight = "Joy1_X";

                    break;
            }

            Debug.Log("New input mode : " + currentInputMode_Name);

            if(newInputMode != null)
                newInputMode(_newMode); // delegate : va appeler toutes les méthodes qui ont besoin d'update les inputs

            return true;
        }


        /// <summary>
        /// Update input mode (return true if it has changed)
        /// </summary>
        /// <param name="_playerIndex"></param>
        /// <returns></returns>
        private static bool CheckForNewInputMode()
        {
            if (inputMode != InputMode.keyboard && IsThereKeyboardInput())
            {
                ChangeInputMode(InputMode.keyboard);
                return true;
            }
            else if (inputMode != InputMode.joy && IsThereControlerInput())
            {
                ChangeInputMode(InputMode.joy);
                return true;
            }

            return false;
        }

        private static bool IsThereKeyboardInput()
        {
            // mouse & keyboard buttons
            
            if (Event.current != null && (Event.current.isKey ||
                Event.current.isMouse))
            {
                return true;
            }
            // mouse movement
            if (Input.GetAxis("Keyboard_CamX") != 0.0f ||
                Input.GetAxis("Keyboard_CamY") != 0.0f)
            {
                return true;
            }
            return false;
        }

        private static bool IsThereControlerInput()
        {
            // joystick buttons
            if (Input.GetKey(KeyCode.JoystickButton0) ||
               Input.GetKey(KeyCode.JoystickButton1) ||
               Input.GetKey(KeyCode.JoystickButton2) ||
               Input.GetKey(KeyCode.JoystickButton3) ||
               Input.GetKey(KeyCode.JoystickButton4) ||
               Input.GetKey(KeyCode.JoystickButton5) ||
               Input.GetKey(KeyCode.JoystickButton6) ||
               Input.GetKey(KeyCode.JoystickButton7) ||
               Input.GetKey(KeyCode.JoystickButton8) ||
               Input.GetKey(KeyCode.JoystickButton9) ||
               Input.GetKey(KeyCode.JoystickButton10) ||
               Input.GetKey(KeyCode.JoystickButton11) ||
               Input.GetKey(KeyCode.JoystickButton12) ||
               Input.GetKey(KeyCode.JoystickButton13) ||
               Input.GetKey(KeyCode.JoystickButton14) ||
               Input.GetKey(KeyCode.JoystickButton15) ||
               Input.GetKey(KeyCode.JoystickButton16) ||
               Input.GetKey(KeyCode.JoystickButton17) ||
               Input.GetKey(KeyCode.JoystickButton18) ||
               Input.GetKey(KeyCode.JoystickButton19))
            {
                return true;
            }

            // joystick axis
            if (Input.GetAxis("Joy1_Horizontal") != 0.0f ||
               Input.GetAxis("Joy1_Vertical") != 0.0f ||
               Input.GetAxis("Joy1_CamX") != 0.0f ||
               Input.GetAxis("Joy1_CamY") != 0.0f)
            {
                return true;
            }

            return false;
        }


        /// <summary> Fonction qui regroupe les valeur à Init pour la vibration des GamePads
        /// 
        /// </summary>
        private void InitGamePadFactors()
        {
            nbrPlayersSet = 0;
            playerIndex = new PlayerIndex[2];
            state = new GamePadState[2];
            //prevState = new GamePadState[2];

            isPlayerVibrate = new bool[2];
            isPlayerVibrate[0] = false; isPlayerVibrate[1] = false;
            leftMotor = new float[2];
            leftMotor[0] = 0.0f; leftMotor[1] = 0.0f;
            rightMotor = new float[2];
            rightMotor[0] = 0.0f; rightMotor[1] = 0.0f;
            vibrDuration = new float[2];
            vibrDuration[0] = 0.0f; vibrDuration[1] = 0.0f;
        }
        
        /// <summary> Lance une vibration manette
        /// 
        /// </summary>
        /// <param name="_playerIndex">Index du joueur.</param>
        /// <param name="_leftMotor">Puissance du moteur gauche. (0->1)</param>
        /// <param name="_rightMotor">Puissance du moteur droit. (0->1)</param>
        /// <param name="_duration">Durée de vibration de la manette. (en sec)</param>
        public static void GamePad_StartVibration(int _playerIndex, float _leftMotor, float _rightMotor, float _duration)
        {
            isPlayerVibrate[_playerIndex] = true;
            leftMotor[_playerIndex] = _leftMotor;
            rightMotor[_playerIndex] = _rightMotor;
            vibrDuration[_playerIndex] = _duration;
        }

        private void OnApplicationQuit()
        {
            // Sécuritée: Stop la vibration lorsque le jeu s'arrête.
            GamePad.SetVibration(playerIndex[0], 0, 0);
            GamePad.SetVibration(playerIndex[1], 0, 0);
        }
    }

}


