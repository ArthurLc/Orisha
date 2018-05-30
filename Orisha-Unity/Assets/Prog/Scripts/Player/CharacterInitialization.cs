/* CharacterInitialization.cs
* Au lancement du niveau, détermine qui jour quel personnage et les initialise en fonction (active/désactive les scripts de controlleurs et d'IA)
* Objectifs : 
*   - Optimisation : active/désactive les scripts d'IA/controlleurs
*   - Interface : permet d'accéder à tous les script du player
*   - Changement in game possible (fonction publique à appeler)
*
* A faire : switch d'un joueur vers une IA et inversement en jeu
* 
* Crée par Ambre LACOUR le 11/10/2017
* Dernière modification par Ambre le 15/01/2017 
*/

using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;


namespace vd_Player
{
    public class CharacterInitialization : MonoBehaviour
    {
        // Scripts du joueur
        [Header("Infos")]
        [SerializeField] private bool IsMenuPrefab = false;
        [SerializeField] private Transform playerTr;
        public Transform PlayerTr
        {
            get { return playerTr; }
        }

        private PlayerCamera playerCam;
        public PlayerCamera PlayerCam
        {
            get { return playerCam; }
        }
        private Camera cam;
        public Camera Cam
        {
            get { return cam; }
        }

        private Rigidbody rb;
        public Rigidbody Rb
        {
            get { return rb; }
        }

        private PlayerController playerController;
        public PlayerController PlayerController
        {
            get { return playerController; }
        }

        private PlayerDash playerDash;
        public PlayerDash DashController
        {
            get { return playerDash; }
        }

        private PlayerFight playerFight;
        public PlayerFight PlayerFight
        {
            get { return playerFight; }
        }

        private Animator anim;
        public Animator Anim
        {
            get { return anim; }
        }

        private PlayerAnimEvents animEvents;
        public PlayerAnimEvents AnimEvents
        {
            get { return animEvents; }
        }


        private bool areInputsFrozen = false;
        public bool AreInputsFrozen {  get { return areInputsFrozen; } }
        public void FreezeInputs() { areInputsFrozen = true; }
        public void UnfreezeInputs() { areInputsFrozen = false; }



        //Caractéristiques du player
        int originMaxHealth; //Vie max d'origine du joueur.
        int maxHealth; //Vie max actuel du joueur.
        public int MaxHealth
        {
            get { return maxHealth; }
        }
        [SerializeField] int health = 200; //Vie actuel du joueur.
        public int Health {
            get { return health;}
        }

        //Mort du player
        private bool isPlayerDying;
        public bool IsPlayerDying
        {
            get { return isPlayerDying; }
            set { isPlayerDying = value; }
        }



        private DisplayZoneName zoneScript;
        private LifeBar lifeBarHUD;
        public LifeBar LifeBarHUD
        {
            get { return lifeBarHUD; }
        }
        [Header("UI")]
        [SerializeField] float fadeInDuration = 3.0f;
        [SerializeField] float displayDuration = 3.0f;
        [Header("ParticleSystem")]
        [SerializeField] ParticleSystem ParticleHeal;


        void Start()
        {
            playerController = GetComponentInChildren<PlayerController>();
            playerDash = GetComponentInChildren<PlayerDash>();
            playerFight = GetComponentInChildren<PlayerFight>();
            playerCam = GetComponentInChildren<PlayerCamera>();
            cam = GetComponentInChildren<Camera>();
            rb = GetComponentInChildren<Rigidbody>();
            anim = GetComponentInChildren<Animator>();
            animEvents = GetComponentInChildren<PlayerAnimEvents>();
            zoneScript = FindObjectOfType<DisplayZoneName>();
            lifeBarHUD = FindObjectOfType<LifeBar>();

            originMaxHealth = health;
            maxHealth = health;
            isPlayerDying = false;

            CheckpointsManager.PlayerRef = this;

            if(IsMenuPrefab == false)
                GameLoopManager.DisableMouse();
        }

        public void ChangeControleMode(vd_Inputs.InputMode _newMode, Player _player)
        {
            vd_Inputs.InputManager.newInputMode(_newMode);

            switch (_newMode)
            {
                case vd_Inputs.InputMode.none:
                    playerController.enabled = false;
                    playerDash.enabled = false;
                    playerFight.enabled = false;
                    playerCam.gameObject.SetActive(false);
                    rb.Sleep();
                    break;

                case vd_Inputs.InputMode.joy:
                    playerController.enabled = true;
                    playerDash.enabled = true;
                    playerFight.enabled = true;
                    playerCam.gameObject.SetActive(true);
                    if (rb.IsSleeping()) rb.WakeUp();
                    break;

                case vd_Inputs.InputMode.keyboard:
                    playerController.enabled = true;
                    playerDash.enabled = true;
                    playerFight.enabled = true;
                    playerCam.gameObject.SetActive(true);
                    if (rb.IsSleeping()) rb.WakeUp();
                    break;

                default:
                    break;
            }
        }


        /// <summary>
        /// Recherche des ennemis moins loin que selectionDistance)
        /// </summary>
        public List<Transform> GetNearbyTargets(float _selectionDistance)
        {
            List<Transform> targets = new List<Transform>();

            float tempDistance = 0.0f;
            foreach (GameObject target in GameObject.FindGameObjectsWithTag("Target"))
            {
                tempDistance = Vector3.Distance(target.transform.position, PlayerTr.position);
                if (tempDistance < _selectionDistance)
                {
                    targets.Add(target.transform);
                }
            }

            return targets;
        }

        /// <summary>
        /// Obtention de l'ennemi le plus proche
        /// </summary>
        public Transform InitSelectedTarget(List<Transform> _nearbyTargets, float _maxDist)
        {
            Transform tr = null;
            float dist = _maxDist;
            for (int i = 0; i < _nearbyTargets.Count; i++)
            {
                if (Vector3.Distance(_nearbyTargets[i].position, PlayerTr.position) < dist)
                {
                    dist = Vector3.Distance(_nearbyTargets[i].position, PlayerTr.position);
                    tr = _nearbyTargets[i];
                }
            }

            return tr;
        }

        /// <summary>
        /// Search if enemy is nearby and rotate the player toward it
        /// </summary>
        public void RotateToFaceEnemy(float _maxEnemyDist)
        {
            List<Transform> targets = GetNearbyTargets(_maxEnemyDist);
            Transform tr = InitSelectedTarget(targets, _maxEnemyDist);

            if (tr != PlayerTr)
            {
                playerTr.LookAt(tr);
                playerTr.rotation = Quaternion.Euler(0.0f, playerTr.rotation.eulerAngles.y, 0.0f);
            }
        }

        public void ChangeHealthFactor(float _newHealthFactor)
        {
            float currentRatioHealth = (float)((float)health / (float)maxHealth); //Récupération du ratio des pv actuels
            maxHealth = (int)(originMaxHealth * _newHealthFactor); //Ajout de la nouvelle vie max
            health = (int)(maxHealth * currentRatioHealth); //Ajout des nouveaux pv en fonction des pv restants au joueur.
        }


        public void TakeDamage(int _damages, bool _takeAnimControl)
        {
            health -= _damages;
            lifeBarHUD.UpdateLifeBar((float)health / (float)maxHealth);
            //Debug.Log (_damages);

            if (health <= 0.0f)
			{
				if (isPlayerDying == false) 
				{
					zoneScript.BeginDisplay ("GameOver", fadeInDuration, displayDuration);
					// Le player fait "HAAAAAAAAA" parce qu'il meurt.
					// Le player joue l'animation de mort.
					TimeManager.Instance.Block_Player_WithTimer (fadeInDuration + displayDuration);
					TimeManager.Instance.Block_Ennemies_WithTimer (fadeInDuration + displayDuration, true);
					StartCoroutine (WaitToRepop (fadeInDuration + displayDuration));
					anim.SetTrigger ("IsDead");

					isPlayerDying = true;
				}
			}
			else if(!DashController.IsDashing && !_takeAnimControl)
			{
				if (_damages > 10)
					anim.SetTrigger ("HugeHit");
				else
					anim.SetTrigger ("SoftHit");
			}
        }

        public void RoarDamages(int _damages, Vector3 _roarSource)
        {
            TakeDamage(_damages, true);
            playerTr.forward = (playerTr.position - _roarSource).normalized;
            if(health > 0)
            {
                anim.SetTrigger("Eject");
                TimeManager.Instance.Block_Player_WithTimer(0.5f);
            }
        }

        public void HealPlayer(int _healNumber)
        {
            health = Mathf.Clamp(health + _healNumber, 0, maxHealth);
            lifeBarHUD.UpdateLifeBar((float)health / (float)maxHealth);

            ParticleHeal.Play();
        }

        public void PropulsePlayer(Vector3 _dir)
        {
			Vector3 dir = -animEvents.transform.forward;
			dir.y = dir.x + dir.z * 4;

			rb.AddForce(dir * 100, ForceMode.Impulse);
			anim.SetTrigger ("Propulsed");
        }

        public void RepopPlayerAtTransform(Transform _tr)
        {
            Rb.velocity = Vector3.zero;
            Rb.angularVelocity = Vector3.zero;
            if (_tr != null)
            {
                PlayerTr.position = _tr.position;
                PlayerTr.rotation = _tr.rotation;
            }
            else
            {
                PlayerTr.localPosition = Vector3.zero;
                PlayerTr.localRotation = Quaternion.identity;
            }

            health = maxHealth;
            lifeBarHUD.UpdateLifeBar((float)health / (float)maxHealth);

            isPlayerDying = false;
        }



        public IEnumerator WaitToRepop(float _timer)
        {
            float localTimer = _timer;

            while (localTimer > 0)
            {
                localTimer -= Time.deltaTime;
                yield return null;
            }

			anim.SetTrigger ("Repop");
            CheckpointsManager.RepopPlayerToCloserCheckpoint();
            yield return null;
        }
    }

}
