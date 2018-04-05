using UnityEngine;
using vd_Inputs;

/* PlayerController.cs
* Gestion du controle du player (déplacement libre, strafe et focus)
* 
* Fonctions : 
*       - Update du déplacement libre par défaut (lié aux camera libre et automatique)
*       - En cas de focus, update du déplacement en strafe (lié à la camera focus)
*       
* NB :
*       - N'update PAS le mode de camera
*       - Déplace le rigidbody (pas le transform) du joueur
*       
* A faire :
*       - Gestion plus smooth de la vitesse au joystick (retoucher la curve)
*
*       
* Crée par Julien LOPEZ le 11/10/2017
* Dernière modification par Ambre Lacour le 08/11/2017 
*/


namespace vd_Player
{

    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {

        [SerializeField] CharacterInitialization ci; //Référence nécessaire pour récupérer le numéro du joueur
        [SerializeField] Animator anim;
        [SerializeField] Rigidbody rb;
        [SerializeField] Transform tr;

        [SerializeField] private float rotationSpeed = 180.0f; //Rate of the rotate movement
        [SerializeField] private AnimationCurve movementCurve_Joystick;

        private bool canRotate;
        public bool CanRotate
        {
            get { return canRotate; }
            set { canRotate = value; }
        }

        public bool IsGrounded
        {
            get { return isGrounded;  }
        }

        private bool isRunLocked;

        float inputHorizontal = 0.0f;
        float inputVertical = 0.0f;
        Vector3 currentDir = Vector3.zero;
        Vector3 rotationDir = Vector3.zero;
        float currentSpeed = 0.0f;


        // Grounded
        [SerializeField] private bool isGrounded;            //If the player is on the ground
        float raycastLength = 0.5f;


        // Use this for initialization
        void Start()
        {
            Debug.Assert(ci != null, "PlayerController n'a pas accès au script CharacterInitialization");

            isGrounded = false;
            canRotate = true;
        }

        void Update()
        {
            inputHorizontal = Input.GetAxis(InputManager.Horizontal);
            inputVertical = Input.GetAxis(InputManager.Vertical);

            anim.SetFloat("InputHorizontal", inputHorizontal);
            anim.SetFloat("InputVertical", inputVertical);
            anim.SetFloat("Speed", currentSpeed);
            anim.SetBool("IsGrounded", isGrounded);

            if (Input.GetButtonDown(InputManager.LockRun))
                isRunLocked = !isRunLocked;

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                TimeManager.Instance.Slow_OneCharacter_WithCurve(anim.gameObject, 2.0f, "Parabole");
            }
            if (Input.GetKeyDown(KeyCode.Insert))
            {
                TimeManager.Instance.Slow_OneCharacter_WithCurve(anim.gameObject, 2.0f, "Parabole");
            }

            if (Input.GetButtonDown(InputManager.Jump) && isGrounded)
                anim.SetTrigger("IsJumping");
        }

        void FixedUpdate()
        {
            if (ci.AreInputsFrozen == false)
                Move();
            else
                currentSpeed = 0.0f;

            CheckIfGrounded();
        }


        /// <summary>
        /// Calcul du déplacement du perso : rotation et idle/marche/course
        /// </summary>
        private void Move()
        {
            // Application des inputs sur le rigidbody (déplacement direct vers la direction où on veut aller)
            // NB : mouvements relatifs à la camera
            currentDir = inputVertical * ci.Cam.transform.forward + inputHorizontal * ci.Cam.transform.right;
            currentDir.y = 0.0f;
            currentDir.Normalize();

            if (InputManager.GetInputMode == InputMode.keyboard)
            {
                currentSpeed = currentDir.magnitude;
                if (isRunLocked == false && Input.GetButton(InputManager.Run) == false)
                    currentSpeed *= 0.5f;
            }
            else
            {
                currentSpeed = movementCurve_Joystick.Evaluate(currentDir.magnitude);
            }

            // Rotation du rigidbody pour faire face à la direction du mouvement (rotation rapide)
            rotationDir = currentDir;


            if (canRotate == true && rotationDir != Vector3.zero)
            {
                rb.MoveRotation(Quaternion.Slerp(
                    rb.rotation,
                    Quaternion.LookRotation(rotationDir),
                    Time.deltaTime * rotationSpeed
                    ));
            }


        }


        /// <summary>
        /// Met à jour le booléen IsGrounded (RayCast)
        /// </summary>
        private void CheckIfGrounded()
        {
            Vector3 begin = new Vector3(tr.position.x, tr.position.y + 0.05f, tr.position.z);
            isGrounded = Physics.Raycast(begin, -Vector3.up, raycastLength);        
        }


        //______Affichage debug du raycast de grounded du personnage
        //private void OnDrawGizmos()
        //{
        //    Gizmos.color = Color.blue;
        //    Vector3 begin = new Vector3(tr.position.x, tr.position.y + 0.05f, tr.position.z);
        //    Vector3 end = new Vector3(tr.position.x, tr.position.y - raycastLength, tr.position.z);
        //    Gizmos.DrawLine(begin, end);
        //}
    }
}
