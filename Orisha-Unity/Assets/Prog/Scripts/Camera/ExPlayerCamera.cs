using System.Collections.Generic;

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
*   - Camera automatique par défaut
*   - S'il y a des inputs de camera libre, camera libre
*   - S'il y a un input de focus, camera focus (cible l'ennemi le plus proche et le joueur)
*   - A la fin d'un input focus, retour en camera automatique (la touche de focus peut être utilisée pour "recentrer"
*       la camera derrière le personnage)
*       
*   - NB : le mode de camera auto est mis de côté
*   
* @A faire :
*   - Ajuster les différentes cameras dans la scène (surtout la libre)
*   - Mettre un blend custom pour que la transition vers le focus soit rapide, mais pas forcément les autres trans'
*/


namespace vd_Player
{

    /// <summary>
    /// /!\[Obsolete]/!\
    /// </summary>
    public class ExPlayerCamera : MonoBehaviour
    {
        public enum CamState
        {
            none,
            free,
            focus
        }

        public enum FocusTarget
        {
            me,
            other
        }

        [Header("General")]
        [SerializeField] CharacterInitialization ci; //Référence nécessaire pour récupérer le numéro du joueur
        [SerializeField] private CamState state;
        public CamState State
        {
            get { return state; }
        }
        [SerializeField] private bool isDebugOn;

        [Header("Cinemachines CM")]
        [SerializeField] private CinemachineFreeLook CM_Free;
        [SerializeField] private CinemachineVirtualCamera CM_Focus;


        [Header("Paramètres de camera libre")]
        [SerializeField] [Range(1, 50000.0f)] private float freeCamSpeed_Joystick = 12000.0f;
        [SerializeField] [Range(1, 50000.0f)] private float freeCamSpeed_Keyboard = 6000.0f;

        [Header("Paramètre de camera follow")]
        [SerializeField] private Transform follow_playerTarget;      // cible de la camera follow (initialise sa pos en script, se place devant el player au moment du focus)
        [Header("Paramètre de camera focus")]
        [SerializeField] private CinemachineTargetGroup focus_targetGroup;
        [SerializeField] private Transform focus_playerTarget;      // cible de la camera focus (initialise sa pos en script, se place devant el player au moment du focus)
        [SerializeField] private Vector3 focus_playerTargetOffset;  // Offset pour calibrer la camera focus (où elle vise devant le joueur, +/- loin  ou haut)
        private Vector3 focus_playerForward = Vector3.zero;
        private List<Transform> focus_nearTargets;                  // targets à proximité (leur distance au joueur et leur transform)
        private Transform focus_selectedTarget;
        [SerializeField] private float focus_selectionDistance;     // Distance max à laquelle on peut viser un objet

        // CameraShaking
        private float amplitudeGain = 0.0f;
        private float vibrationDuration = 0.0f;
        private CinemachineBasicMultiChannelPerlin cm_noise;


        private Vector3 camForward;
        public Vector3 CamForward
        {
            get { return camForward; }
        }

        private Vector3 camRight;
        public Vector3 CamRight
        {
            get { return camRight; }
        }


        private void OnEnable()
        {
            vd_Inputs.InputManager.newInputMode += UpdateInputMode;
        }
        private void OnDisable()
        {
            vd_Inputs.InputManager.newInputMode -= UpdateInputMode;

        }


        private void Start()
        {
            Start_InitFocus();
        }

        void Update()
        {
            // Récupération des inputs de camera libre et update du repère de la camera 
            // (par rapport à quel repère vont se faire les déplacements du joueur)
            UpdateCamForwardAndRight();
            UpdateFocus();

            // Update du mode de camera actif
            UpdateCameraMode();

            // CameraShaking
            UpdateCameraShaking();

            // Affichage debug
            focus_playerTarget.GetComponent<MeshRenderer>().enabled = isDebugOn && state == CamState.focus; // affiche ou non la sphere debug sur la/les cible/s du focus

        }

        /// <summary>
        /// Update du mode de camera en fonction des inputs joueur
        /// </summary>
        void UpdateCameraMode()
        {
            if (Input.GetButton(vd_Inputs.InputManager.Focus)
                   || Input.GetAxisRaw(vd_Inputs.InputManager.Focus) >= 0.1f)
                SetCamState(CamState.focus);
            else
                SetCamState(CamState.free);

        }

        /// <summary>
        /// Fonction a appeler en cas de changement du mode d'input d'un joueur, des axes inversés/non (ou d'initialisation)
        /// </summary>
        public bool UpdateInputMode(vd_Inputs.InputMode _newInputMode)
        {
            CM_Free.m_XAxis.m_InputAxisName = vd_Inputs.InputManager.CamX;
            CM_Free.m_YAxis.m_InputAxisName = vd_Inputs.InputManager.CamY;

            if (_newInputMode == vd_Inputs.InputMode.keyboard)
            {
                CM_Free.m_XAxis.m_MaxSpeed = freeCamSpeed_Keyboard;
            }
            else
            {
                CM_Free.m_XAxis.m_MaxSpeed = freeCamSpeed_Joystick;
            }

            return true;
        }

        /// <summary>
        /// Update des vecteurs camForward et camRight qui servent de repères de déplacement au joueur
        /// </summary>
        private void UpdateCamForwardAndRight()
        {
                camForward = ci.Cam.transform.forward;
                camRight = ci.Cam.transform.right;
        }

        /// <summary>
        /// Change le mode de camera du joueur (auto, free ou focus)
        /// </summary>
        /// <param name="_state"></param>
        public void SetCamState(CamState _state)
        {
            // si on est déjà dans la state, return
            if (state == _state)
                return;

            CM_Free.Priority = 0;
            CM_Focus.Priority = 0;

            if (state == CamState.focus)
                EndFocus();

            switch (_state)
            {
                case CamState.free:
                    CM_Free.Priority = 1;
                    InitFreeCam();
                    break;
                case CamState.focus:
                    CM_Focus.Priority = 1;
                    InitFocusTargets();
                    DisableFreeCam();
                    break;
                default:break;
            }

            state = _state;

        }


        public CinemachineBasicMultiChannelPerlin GetCurrentCamNoise()
        {
            switch (state)
            {
                case CamState.free:
                    return CM_Free.GetRig(0).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                case CamState.focus:
                    return CM_Focus.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                default:
                    Debug.LogError("GetCurrentCam(): La camera actuel n'est pas répertorié !");
                    return null;
            }
        }


        //_______________FREE CAM_______________//

        /// <summary>
        /// Initialisation de la free cam (à appeler à chaque fois qu'on entre en state free)
        /// </summary>
        private void InitFreeCam()
        {
            CM_Free.m_XAxis.Value = ci.Cam.transform.rotation.eulerAngles.y;
            CM_Free.m_XAxis.m_InputAxisName = vd_Inputs.InputManager.CamX;
            CM_Free.m_YAxis.m_InputAxisName = vd_Inputs.InputManager.CamY;
        }

        /// <summary>
        /// Désactive les inputs de camera libre (à appeler à chaque fois qu'on sort d'un state free)
        /// </summary>
        private void DisableFreeCam()
        {
            CM_Free.m_XAxis.m_InputAxisName = "";
            CM_Free.m_YAxis.m_InputAxisName = "";
        }


        //________________FOCUS__________________//

        /// <summary>
        /// Initialisation du focus (à appeler une fois dans le Start)
        /// </summary>
        private void Start_InitFocus()
        {
            focus_nearTargets = new List<Transform>();

            if (focus_targetGroup == null)
                Debug.LogError("Attention, la camera n'a pas de TargetGroup pour son focus");
        }

        /// <summary>
        /// Initialisation des cibles du focus (joueur et ennemi), à appeler quand on commence le focus
        /// </summary>
        private void InitFocusTargets()
        {
            focus_playerForward = new Vector3(ci.PlayerTr.forward.x * focus_playerTargetOffset.z, ci.PlayerTr.forward.y + focus_playerTargetOffset.y, ci.PlayerTr.forward.z * focus_playerTargetOffset.z);
            focus_playerTarget.forward = ci.PlayerTr.forward;
            focus_playerTarget.position = ci.PlayerTr.position + focus_playerForward;

            focus_nearTargets = ci.GetNearbyTargets(focus_selectionDistance);
            focus_selectedTarget = ci.InitSelectedTarget(focus_nearTargets, focus_selectionDistance);
            if (focus_selectedTarget == null)
                focus_selectedTarget = focus_playerTarget;

            focus_targetGroup.m_Targets[1].target = focus_selectedTarget;
        }

        /// <summary>
        /// Mise à jour du focus (à appeler dans l'Update) : cibles et déplacement de la cam libre inactive
        /// </summary>
        private void UpdateFocus()
        {
            focus_playerTarget.position = ci.PlayerTr.position + focus_playerForward;
            if (state == CamState.focus)
            {

                if (Vector3.Distance(focus_playerTarget.position, focus_selectedTarget.position) < 2.0f)
                {
                    CM_Focus.m_Follow = null;
                    focus_targetGroup.m_Targets[0].target = focus_selectedTarget;
                }
                else
                {
                    CM_Focus.m_Follow = follow_playerTarget;
                    focus_targetGroup.m_Targets[0].target = focus_playerTarget;
                }

                if (focus_nearTargets.Count > 0)
                    focus_playerTarget.LookAt(focus_selectedTarget);
                else
                    CM_Focus.m_Follow = follow_playerTarget;
            }
        }

        /// <summary>
        /// Renvoie la position de la cible du focus (s'il n'y en a pas, renvoie 0)
        /// </summary>
        /// <returns></returns>
        public Vector3 GetFocusTargetPosition()
        {
            if (state == CamState.focus)
                return focus_selectedTarget.position;
            else
                return Vector3.zero;
        }

        /// <summary>
        /// Fin du focus (appelé quand on sort de la state focus)
        /// </summary>
        private void EndFocus()
        {
            CM_Focus.m_Follow = follow_playerTarget;
            focus_targetGroup.m_Targets[0].target = focus_playerTarget;
            focus_targetGroup.m_Targets[1].target = focus_playerTarget;
            focus_selectedTarget = null;
            focus_nearTargets.Clear();
        }


        //___________________CAMERA SHAKING_____________//


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
            if (state == CamState.free && Input.GetButtonDown(vd_Inputs.InputManager.ResetCamera))
            {
                if (isDebugOn)
                    Debug.Log("retourne a ta position");
                //CM_Free.transform.position = transform.position + (-transform.forward * 1000);
                //Camera.main.transform.position = transform.position + (-transform.forward * 1000);
            }
        }
    }


}
