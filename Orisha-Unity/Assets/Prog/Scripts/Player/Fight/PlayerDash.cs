using UnityEngine;

/* PlayerDash.cs
* Gestion du dash du player
* 
* Fonctions : 
*       - Déclenchement du dash
*       - Update du dash
*       
* NB :
*       - Coopère avec le PlayerController qui est sur le même GameObject (PlayerController)
*       
* A faire :
*       - 
*
*       
* Crée par Ambre LACOUR le 22/11/2017
*/


using vd_Inputs;

namespace vd_Player
{
    public class PlayerDash : MonoBehaviour
    {
        [SerializeField] private CharacterInitialization ci;
        private PlayerController playerController;

        [SerializeField] private bool isDashing = false;
        private bool canDash = true;
        [SerializeField][Range(0.0f, 50.0f)] private float speed = 20.0f;
        [SerializeField][Range(0.0f, 1.0f)] private float height = 0.5f;
        [SerializeField][Range(0.1f, 0.5f)] private float duration = 0.05f;
        [SerializeField][Range(0.0f, 2.0f)] private float dashCooldown = 0.5f;
        private float timer;

        private Vector3 dashDirection;

        private float inputHorizontal;
        private float inputVertical;

        [SerializeField] private bool verbose = false;

        private ParticleSystem particleDash;

        // Use this for initialization
        void Start()
        {
            playerController = GetComponent<PlayerController>();
            particleDash = GetComponentInChildren<ParticleSystem>();
            if (playerController == null)
                Debug.LogError("PlayerDash n'a pas acces au PlayerController");
            else if (ci == null)
                Debug.LogError("PlayerController n'a pas de lien vers CharacterInitialization");
            else if (particleDash == null)
                Debug.LogError("Pas de particle system pour le dash");
            timer = 0.0f;
        }



        void Update()
        {
            //Debug.DrawLine(ci.PlayerTr.transform.position, ci.PlayerTr.transform.position + dashDirection, Color.magenta);

            if (ci.AreInputsFrozen == false)
            {
                if (isDashing == false && canDash == true)
                {
                    inputHorizontal = Input.GetAxis(InputManager.Horizontal);
                    inputVertical = Input.GetAxis(InputManager.Vertical);

                    if (playerController.IsGrounded && Input.GetButtonDown(InputManager.Dash))
                    {
                        canDash = false;
                        BeginDash();
                    }
                }
                else
                {
                    timer += Time.deltaTime;
                    if (timer < duration)
                    {
                        ci.Rb.AddForce(dashDirection.normalized * speed * 50);
                        dashDirection.y = (height * (duration * 0.4f) / timer) - height;


                        if (verbose)
                            Debug.Log(dashDirection.normalized);
                    }
                    else if (timer < duration + dashCooldown)
                    {
                        isDashing = false;
                    }
                    else
                    {
                        canDash = true;
                        timer = 0.0f;
                        ci.AnimEvents.DisableAllBoxes();
                        ci.AnimEvents.EnableEnemyCollisions();
                    }
                }
            }
            else
            {
                isDashing = false;
            }

            ci.Anim.SetBool("IsDashing", isDashing);

        }


        void BeginDash()
        {
            ci.PlayerFight.ClearListInputs();

            // direction du dash en fonction des inputs de déplacement
            dashDirection = inputVertical * ci.Cam.transform.forward
                 + inputHorizontal * ci.Cam.transform.right;
            // en cas d'absence d'inputs, dash en fonction du forward
            if (dashDirection.magnitude < 0.1f)
                dashDirection = ci.PlayerTr.transform.forward;
            dashDirection.y = height;
            
            particleDash.Play();

            isDashing = true;
        }
    }
}
