using System.Collections;
using UnityEngine;
using Cinemachine;

/*
* @AmbreLacour
* @PlayerCamera.cs
* @17/01/2017
* @Le script s'attache a un empty qui contient en enfant la Camera et les camera cinemachine liées au joueur
*   - Il permet de switch entre les différents types de cameras et affine le comportement des cameras
* 
* @Fonctionnnement actuel :
*   - Camera d'explo hors combat (comportement basé sur les states d'animations et s'oriente vers les points d'intérêts)
*   - Camera de fight en combat (rien ne l'oriente à part ses rigs et les inputs joueur)
*  
*/


namespace vd_Player
{

    /// <summary>
    /// Classe de camera du player /
    /// oscille entre deux cameras : exploration et combat /
    /// s'oriente vers les points d'intérêt
    /// </summary>
    public class PlayerCamera : MonoBehaviour
    {
        /// <summary>
        /// Référence pour récupérer les infos du joueur
        /// </summary>
        [Header("Référence vers le joueur")]    
        [SerializeField] CharacterInitialization ci;

        /// <summary>
        /// State actuel de la camera (explo, fight ou none)
        /// </summary>
        public enum CamState
        {
            none,
            explo,
            fight
        }
        /// <summary>
        /// Etat atuel de la camera (fight ou explo)
        /// </summary>
        [Header("Etat de la camera")]
        [SerializeField] private CamState state;
        /// <summary>
        /// Etat atuel de la camera (fight ou explo)
        /// </summary>
        public CamState State
        {
            get { return state; }
        }
        [SerializeField] private bool isDebugOn = false;

        /// <summary>
        /// Camera d'exploration (choisit quelle camera est active en fonction de l'animation du player)
        /// </summary>
        [Header("Cinemachines CM")]
        [SerializeField] private CinemachineStateDrivenCamera CM_Explo_StateDriven;
        /// <summary>
        /// Cameras d'explorations de chaque animation
        /// </summary>
        private CinemachineFreeLook[] CM_SubExploCams;
        /// <summary>
        /// Camera de combat
        /// </summary>
        [SerializeField] private CinemachineFreeLook CM_Fight;


        [Header("Vitesse de la camera")]
        [SerializeField] [Range(1, 1000.0f)] private float freeCamSpeed_Joystick = 300.0f;
        [SerializeField] [Range(1, 1000.0f)] private float freeCamSpeed_Keyboard = 300.0f;


        /// <summary>
        /// Point d'intérêt regardé par le joueur
        /// </summary>
        [Header("Landmarks")]
        [SerializeField] private Landmark currentLandmark = null;
        /// <summary>
        /// Distance entre le point d'intérêt courant et le joueur
        /// </summary>
        float currentLandmark_Distance = float.MaxValue;
        /// <summary>
        /// Liste de tous les points d'intérêts de la scène
        /// </summary>
        private Landmark[] landmarks;
        /// <summary>
        /// Vitesse du lerp de camera quand elle s'oriente vers un point d'intérêt
        /// </summary>
        [SerializeField][Range(0.01f, 1.0f)] private float cameraMovingSpeed = 0.25f;

        /// <summary>
        ///  Référence de la coroutine pour pouvoir l'arrêter
        /// </summary>
        Coroutine cameraMovingCoroutine;

        /// <summary>
        /// Place du player quand il est à gauche de l'écran
        /// </summary>
        [SerializeField] [Range(0.10f, 0.45f)] private float playerLeftPos = 0.25f;
        /// <summary>
        /// Place du player quand il est à droite de l'écran
        /// </summary>
        [SerializeField] [Range(0.55f, 0.90f)] private float playerRightPos = 0.75f;

        /// <summary>
        /// Positions du joueur à l'écran
        /// </summary>
        enum PlayerPosOnScreen
        {
            middle,
            right,
            left
        }
        /// <summary>
        /// Poisition actuelle du joueur à l'écran
        /// </summary>
        [SerializeField] PlayerPosOnScreen currentPlayerPos = PlayerPosOnScreen.middle;

        // CameraShaking
        private float amplitudeGain = 0.0f;
        private float vibrationDuration = 0.0f;
        private CinemachineBasicMultiChannelPerlin cm_noise;



        private void OnEnable()
        {
            vd_Inputs.InputManager.newInputMode += UpdateInputMode;
            Potential_Enemy.enteringFight += SetCameraOnFightMode;
        }

        private void OnDisable()
        {
            vd_Inputs.InputManager.newInputMode -= UpdateInputMode;
            Potential_Enemy.enteringFight -= SetCameraOnFightMode;
        }


        private void Start()
        {
            if(CM_SubExploCams == null)
                CM_SubExploCams = CM_Explo_StateDriven.GetComponentsInChildren<CinemachineFreeLook>(true);
            SetCamState(CamState.explo);
            InitLandmarks();

            if (isDebugOn)
                Debug.Log("Camera: " + CM_SubExploCams.Length + " cameras d'animations en explo");
        }

        void Update()
        {
            if (isDebugOn)
                DebugChangeState();                
        }

        #region MODE D'INPUTS
        //__________MODE D'INPUTS___________//

        /// <summary>
        /// Fonction a appeler en cas de changement du mode d'input d'un joueur, des axes inversés/non (ou d'initialisation)
        /// </summary>
        bool UpdateInputMode(vd_Inputs.InputMode _newInputMode)
        {
            UpdateFreeLookCamInput(CM_Fight, _newInputMode); // Update de la camera de fight


            if(CM_SubExploCams == null)
                CM_SubExploCams = CM_Explo_StateDriven.GetComponentsInChildren<CinemachineFreeLook>(true);
            for (int i = 0; i < CM_SubExploCams.Length; i++) // Update de toutes les sous-cams d'explo
            {
                UpdateFreeLookCamInput(CM_SubExploCams[i], _newInputMode);
            }

            if (isDebugOn) // Feedback de debug
                Debug.Log("Update camera input mode: " + _newInputMode);
            return true;
        }

        /// <summary>
        /// Changement des axes d'une camera FreeLook (appelée pour chaque camera lors d'un changement d'inputs)
        /// </summary>
        /// <param name="_cm"></param>
        /// <param name="_newInputMode"></param>
        void UpdateFreeLookCamInput(CinemachineFreeLook _cm, vd_Inputs.InputMode _newInputMode)
        {
            _cm.m_XAxis.m_InputAxisName = vd_Inputs.InputManager.CamX;
            _cm.m_YAxis.m_InputAxisName = vd_Inputs.InputManager.CamY;

            _cm.m_XAxis.m_InvertAxis = !GameLoopManager.camera_isXaxisInversed; // Cinemachine est un peu à l'envers...
            _cm.m_YAxis.m_InvertAxis = !GameLoopManager.camera_isYaxisInversed;

            if (_newInputMode == vd_Inputs.InputMode.keyboard)
            {
                _cm.m_XAxis.m_MaxSpeed = freeCamSpeed_Keyboard * GameLoopManager.camera_sensitivity;
            }
            else
            {
                _cm.m_XAxis.m_MaxSpeed = freeCamSpeed_Joystick * GameLoopManager.camera_sensitivity;
            }
        }

        #endregion

        #region DEBUG
        //_______________DEBUG_______________//

        /// <summary>
        /// Change le mode de camera vec "Entrée"
        /// </summary>
        void DebugChangeState()
        {
            if(Input.GetKeyDown(KeyCode.Return))
            {
                if (state == CamState.explo)
                    SetCamState(CamState.fight);
                else
                    SetCamState(CamState.explo);
            }
        }
        #endregion

        #region SWITCH DE CAMERA
        //____________SWITCH DE CAMERA_________//

        /// <summary>
        /// Passage de la camera en mode combat ou hors combat (delegate qui sera à appeler lors d'un début ou d'une fin de combat
        /// </summary>
        /// <param name="_onFightMode"></param>
        private void SetCameraOnFightMode(bool _onFightMode)
        {
            if (_onFightMode)
                SetCamState(CamState.fight);
            else
                SetCamState(CamState.explo);
        }

        /// <summary>
        /// Change le mode de camera du joueur (auto, free ou focus)
        /// </summary>
        /// <param name="_state"></param>
        public void SetCamState(CamState _state)
        {
            // si on est déjà dans la state, return
            if (state == _state)
            {
                if (isDebugOn)
                    Debug.Log("Camera: mode " + _state + " déjà activé");
                return;
            }

            CM_Explo_StateDriven.Priority = 0;
            CM_Fight.Priority = 0;

            switch (_state)
            {
                case CamState.explo:
                    CM_Explo_StateDriven.Priority = 1;
                    break;
                case CamState.fight:
                    CM_Fight.Priority = 1;
                    break;
                default: break;
            }

            state = _state;

        }
        #endregion


        #region POINTS D'INTERET
        //_______________POINTS D'INTERET_______________//

        //__Public

        /// <summary>
        /// Est-ce qu'il y a un point d'intérêt à l'écran
        /// </summary>
        public bool SeesLandmark()
        {
            return currentLandmark != null;
        }

        /// <summary>
        /// Renvoie la position du point d'intérêt à l'écran le plus proche du joueur (renvoie null si aucun)
        /// </summary>
        /// <returns></returns>
        public Vector3 GetNearLandmarkPosition()
        {
            if (currentLandmark == null)
                return Vector3.zero;

            return currentLandmark.transform.position;
        }

        //__Private

        /// <summary>
        /// Stockage de tous les landmarks de la scène
        /// </summary>
        private void InitLandmarks()
        {
            landmarks = FindObjectsOfType<Landmark>();
            StartCoroutine(LookForLandmarks());
            if (isDebugOn)
                Debug.Log("Player camera: " + landmarks.Length + " landmark(s) found");
        }

        /// <summary>
        /// Coroutine : toutes les 0.5 secs, recherche des landmarks à proximité
        /// </summary>
        /// <returns></returns>
        private IEnumerator LookForLandmarks()
        {
            while (true)
            {
                while (ci.Anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    if (isDebugOn)
                        Debug.Log("\t\t\tLandmarks check : deactivated");
                    yield return null;
                }
                if (isDebugOn)
                    Debug.Log("\t\t\tLandmarks check : activated");
                bool newLandscapeFound = false;
                for (int i = 0; i < landmarks.Length; i++) // Pour tous les point d'intérêt
                {
                    if (landmarks[i].IsVisible()) // S'il est à l'écran
                    {
                        float tempDist = Vector3.Distance(ci.PlayerTr.position, landmarks[i].transform.position); 
                        if (tempDist < currentLandmark_Distance && currentLandmark != landmarks[i]) // Et s'il est le plus proche et pas encore le point d'intérêt courant
                        {
                            if (tempDist < landmarks[i].Radius)
                            {
                                // Nouveau point d'intérêt pour le joueur
                                currentLandmark = landmarks[i];
                                currentLandmark_Distance = tempDist;
                                newLandscapeFound = true;

                                if (isDebugOn)
                                    Debug.Log("Camera: nouveau point d'intérêt");
                            }
                        }
                    }
                }

                if (newLandscapeFound == true) // S'il y a un nouveau point d'intérêt
                    InitCameraMoveTowardLandmark();
                else if (currentLandmark == null) // S'il n'y a aucun point d'intérêt à l'écran
                    InitRecenterCamera();
                else // Sinon update de la position du landscape courant (si on est passé à sa droite ou autre)
                    InitCameraMoveTowardLandmark();

                yield return new WaitForSeconds(1.0f);
            }
        }

        /// <summary>
        /// Initialise le prochain lerp de la camera pour inclure le point d'intérêt
        /// </summary>
        void InitCameraMoveTowardLandmark()
        {
            if(currentLandmark == null) // Sécu
            {
                Debug.LogError("PlayerCam: camera tente de regarder un point d'intérêt null");
                return;
            }

            // Détermine si le point d'intérêt est à droite ou à gauche du player
            PlayerPosOnScreen nextPos = PlayerPosWithLandmark(currentLandmark);
            if(nextPos != currentPlayerPos) // Si on a besoin de recaler le player
            {
                currentPlayerPos = nextPos;
                LaunchMoveCameraCoroutine(); // Lancement d'un recentrage de camera
            }
        }

        /// <summary>
        /// Initialise un retour du joueur au centre de l'écran
        /// </summary>
        void InitRecenterCamera()
        {
            if (currentPlayerPos == PlayerPosOnScreen.middle)
                return;
          
            LaunchMoveCameraCoroutine(); // Lancement d'un recentrage de camera

        }

        /// <summary>
        /// Oriente la camera en fonction de la pos du player à l'écran
        /// </summary>
        void LaunchMoveCameraCoroutine()
        {
            float camCenterX = 0.5f;
            switch (currentPlayerPos)
            {
                case PlayerPosOnScreen.middle:
                    camCenterX = 0.5f;
                    break;
                case PlayerPosOnScreen.right:
                    camCenterX = playerRightPos;
                    break;
                case PlayerPosOnScreen.left:
                    camCenterX = playerLeftPos;
                    break;
            }

            if (isDebugOn)
                Debug.Log("Camera : nouveau lerp enclenché");


            if (cameraMovingCoroutine != null)
                StopCoroutine(cameraMovingCoroutine);

            cameraMovingCoroutine = StartCoroutine(SetCamsOrientation(camCenterX));
        }

        /// <summary>
        /// Coroutine : lerp de la camera jusqu'à ce que le player soit bien placé sur l'écran
        /// </summary>
        /// <param name="_camCenterX"></param>
        /// <returns></returns>
        IEnumerator SetCamsOrientation(float _camCenterX)
        {
            CinemachineComposer comp;
            float timerMovingCamera = 0.0f;
            float playerPosBeforeMoving = ci.Cam.WorldToScreenPoint(ci.PlayerTr.transform.position).x / Screen.width;

            float endLerp = currentLandmark != null ? currentLandmark.Rank/currentLandmark.MaxRank : 1.0f;

            // Tant que le lerp n'est pas terminé, la coroutine tourne
            do
            {
                while (ci.Anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    if(isDebugOn)
                        Debug.Log("Camera: lerp en pause");
                    yield return null;
                }

                timerMovingCamera += Time.deltaTime * cameraMovingSpeed;

                if (isDebugOn)
                    Debug.Log("Camera: lerp en cours (poids " + endLerp + "/1.0");

                for (int i = 0; i < CM_SubExploCams.Length; i++) // Update de l'orientation de toutes les sous cameras d'explo
                {
                    comp = CM_SubExploCams[i].GetRig(0).GetCinemachineComponent(CinemachineCore.Stage.Aim) as CinemachineComposer;
                    comp.m_ScreenX = Mathf.Lerp(playerPosBeforeMoving, _camCenterX, timerMovingCamera);

                    comp = CM_SubExploCams[i].GetRig(1).GetCinemachineComponent(CinemachineCore.Stage.Aim) as CinemachineComposer;
                    comp.m_ScreenX = Mathf.Lerp(playerPosBeforeMoving, _camCenterX, timerMovingCamera);

                    comp = CM_SubExploCams[i].GetRig(2).GetCinemachineComponent(CinemachineCore.Stage.Aim) as CinemachineComposer;
                    comp.m_ScreenX = Mathf.Lerp(playerPosBeforeMoving, _camCenterX, timerMovingCamera);
                }
                yield return null;
            }
            while (timerMovingCamera <= endLerp);

            if (isDebugOn)
                Debug.Log("Camera : lerp terminé");
        }

        /// <summary>
        /// Indique la position que devrait occuper le joueur par rapport au point d'intérêt
        /// </summary>
        /// <param name="_landmark"></param>
        /// <returns></returns>
        PlayerPosOnScreen PlayerPosWithLandmark(Landmark _landmark)
        {
            float landmarkOnScreenX = ci.Cam.WorldToScreenPoint(currentLandmark.transform.position).x / Screen.width;

            if (isDebugOn)
                Debug.Log("Camera : landmark posX on screen : " + landmarkOnScreenX);
            if (landmarkOnScreenX < 0.0f || landmarkOnScreenX > 1.0f) // Sécu : si point d'intérêt sort de l'écran
            {
                currentLandmark = null;
                currentLandmark_Distance = float.MaxValue;
                return PlayerPosOnScreen.middle;
            }

            switch(currentPlayerPos)
            {
                case PlayerPosOnScreen.middle:
                    if (landmarkOnScreenX < 0.4f) // Point d'intérêt à gauche=>layer à droite
                        return PlayerPosOnScreen.right;
                    else if (landmarkOnScreenX > 0.6f) // Point d'intérêt à droite=>layer à gauche
                        return PlayerPosOnScreen.left;
                    else // Point d'intérêt au milieu : player au milieu
                        return PlayerPosOnScreen.middle;

                //////////A FAIRE
                case PlayerPosOnScreen.left:
                    if (landmarkOnScreenX < 0.4f) // Point d'intérêt à gauche=>on recentre tout le monde
                        return PlayerPosOnScreen.middle;
                    else if (landmarkOnScreenX > 0.6f) // Point d'intérêt à droite=>on change rien
                        return PlayerPosOnScreen.left;
                    else // Point d'intérêt au milieu=>on change rien
                        return PlayerPosOnScreen.left;

                case PlayerPosOnScreen.right:
                    if (landmarkOnScreenX < 0.4f) // Point d'intérêt à gauche=>on change rien
                        return PlayerPosOnScreen.right;
                    else if (landmarkOnScreenX > 0.6f) // Point d'intérêt à droite=>on recentre tout le monde
                        return PlayerPosOnScreen.middle;
                    else // Point d'intérêt au milieu=>on change rien
                        return PlayerPosOnScreen.right;
                ///////////

                default:
                    return PlayerPosOnScreen.middle;
            }

        }

        #endregion

        #region CAMERA SHAKING
        //___________________CAMERA SHAKING_____________//

        public CinemachineBasicMultiChannelPerlin GetCurrentCamNoise()
        {
            switch (state)
            {
                case CamState.explo:
                    //return CM_Explo_StateDriven..GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                case CamState.fight:
                    return CM_Fight.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                default:
                    Debug.LogError("GetCurrentCam(): La camera actuel n'est pas répertorié !");
                    return null;
            }
        }

        private void UpdateCameraShaking()
        {
            if (amplitudeGain > 0)
            {
                amplitudeGain -= Time.deltaTime * (1 / vibrationDuration);
                cm_noise.m_AmplitudeGain = amplitudeGain;

                if (amplitudeGain <= 0)
                {
                    amplitudeGain = 0;
                    cm_noise.m_AmplitudeGain = 0;
                }
            }
        }

        public void StartCameraShaking(float _amplitudegain, float _frequencyGain, float _duration)
        {
            
            cm_noise = GetCurrentCamNoise();

            amplitudeGain = _amplitudegain;
            cm_noise.m_FrequencyGain = _frequencyGain;
            vibrationDuration = _duration;
            
        }

        private void RestCamera()
        {
            if (state == CamState.explo && Input.GetButtonDown(vd_Inputs.InputManager.ResetCamera))
            {
                if (isDebugOn)
                    Debug.Log("retourne a ta position");
                //CM_Free.transform.position = transform.position + (-transform.forward * 1000);
                //Camera.main.transform.position = transform.position + (-transform.forward * 1000);
            }
        }

        #endregion
    }


}
