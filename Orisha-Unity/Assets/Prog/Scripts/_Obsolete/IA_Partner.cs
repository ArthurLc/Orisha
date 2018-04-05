using UnityEngine;
using UnityEngine.AI;

/*
* @ArthurLacour, RomainSeurot
* @IA_Teubé.cs
* @04/10/2017
* @Le script s'attache au personnage qui n'est pas controllé par un joueur
*   - Doit avoir le joueur avec lequel elle joue en référence.
*/


namespace vd_Player
{

    public class IA_Partner : MonoBehaviour
    {
        public bool Debug_AI = false;
        public string Debug_StateName;

        enum State
        {
            followPlayer,
            waitNextToPlayer,
            chargeIA,
            waitNextToEnemy,
            retreat
        }
        Animator animator;
        [SerializeField] Transform player;
        CharacterInitialization myCharacter;
        [SerializeField] Transform ai;
        [SerializeField] NavMeshAgent agent;
        GameObject enemy;
        State stateIA;

        [SerializeField] int minPdvBeforeFlee = 50;
        [SerializeField] int Damages = 50;

        [Header("Paramaters")]
        [SerializeField]
        float limitBeforeFollowPlayer = 8.0f;
        [SerializeField] float distanceMinBetweenPlayerIa = 3.0f;
        [SerializeField] float limitBeforeRetreat = 12.0f;
        [SerializeField] float distanceMinBeforeIaAttack = 5.0f;
        [SerializeField] float distanceMinBetweenPantinIA = 2.0f;

        float timerBetweenTwoAttacks = 0.0f;
        [SerializeField] float MaxTimerBetweenTwoAttacks = 2.0f;

        [SerializeField] float AI_PartnerRangeToCheckEnemy = 20.0f;
        float timerBeforeCkeckIfEnemyInRange = 0.0f;
        [SerializeField] float MaxTimerBeforeCkeckIfEnemyInRange = 1.0f;

        void Start()
        {
            myCharacter = transform.GetComponentInParent<CharacterInitialization>();
            if (myCharacter == null)
            {
                Debug.Log("I can't find my charactere parent!!!");
            }
            animator = agent.gameObject.GetComponent<Animator>();
            stateIA = State.followPlayer;
        }

        void Update()
        {
            if (agent.isOnNavMesh == true) // Sécurité : on s'assure que l'agent soit bien sur un NavMesh pour éviter les erreurs
            {
                if (enemy != null)
                {
                    if ((stateIA != State.chargeIA && stateIA != State.waitNextToEnemy && stateIA != State.retreat) && Vector3.Distance(ai.transform.position, enemy.transform.position) <= distanceMinBeforeIaAttack)
                    {
                        SetState_ChargeIA();
                    }
                }
                else
                {
                    timerBeforeCkeckIfEnemyInRange += Time.deltaTime;
                    if (timerBeforeCkeckIfEnemyInRange >= MaxTimerBeforeCkeckIfEnemyInRange) // Check si il y a des enemy dans sa range (pas à chaque frame)
                    {
                        enemy = AI_Tools.CheckForObject(agent.transform.position, AI_PartnerRangeToCheckEnemy, "Enemy", true);
                    }
                }               

                switch (stateIA)
                {
                    case State.followPlayer:
                        if (Vector3.Distance(ai.transform.position, player.position) <= distanceMinBetweenPlayerIa) //Si je suis suffisament près, je m'arrête.
                        {
                            SetState_WaitNextToPlayer();
                        }
                        else if (Vector3.Distance(ai.transform.position, agent.destination) <= 0.1f) //Sinon, si je suis arrivé (et que le player n'est pas dans le coin..)
                        {
                            agent.destination = player.position;
                        }
                        Debug_StateName = "follow player";
                        break;
                    case State.waitNextToPlayer:
                        if (Vector3.Distance(ai.transform.position, player.position) >= limitBeforeFollowPlayer) //Si je suis trop loin, je suis.
                        {
                            SetState_FollowPlayer();
                        }
                        Debug_StateName = "waitNextToPlayer";
                        break;
                    case State.chargeIA:
                        if (Vector3.Distance(ai.transform.position, enemy.transform.position) <= distanceMinBetweenPantinIA) //Si je suis suffisament près, je m'arrête.
                        {
                            SetState_WaitNextToEnemy();
                        }
                        Debug_StateName = "chargeIA";
                        break;
                    case State.waitNextToEnemy:
                        if (Vector3.Distance(ai.transform.position, player.position) >= limitBeforeRetreat) //Si je suis trop loin, je bat en retraite !
                        {
                            SetState_Retreat();
                        }
                        AttackEnemy();
                        Debug_StateName = "waitNextToEnemy";
                        break;
                    case State.retreat:
                        if (Vector3.Distance(ai.transform.position, player.position) <= distanceMinBetweenPlayerIa) //Si je suis suffisament près, je m'arrête.
                        {
                            SetState_WaitNextToPlayer();
                        }
                        else
                        {
                            agent.destination = player.position;
                        }
                        Debug_StateName = "retreat";
                        break;
                    default:
                        break;
                }

                // Update animator speed (to blend between idle/walk/run)
                animator.SetFloat("Speed", agent.velocity.magnitude / agent.speed);
            }      
            else
            {
                Debug.LogWarning("Attention, " + gameObject.name + " n'est pas sur un navMesh, comportement IA_Player impossible");
            }

            if (Debug_AI)
            {
                Debug.Log("AI_Partner state = " + Debug_StateName);
            }
            GetPdv();
        }

        void SetState_FollowPlayer()
        {
            agent.destination = player.position;
            stateIA = State.followPlayer;
        }
        void SetState_WaitNextToPlayer()
        {
            agent.destination = ai.transform.position;
            stateIA = State.waitNextToPlayer;
        }
        void SetState_ChargeIA()
        {
            agent.destination = enemy.transform.position;
            stateIA = State.chargeIA;
        }
        void SetState_WaitNextToEnemy()
        {
            agent.destination = ai.transform.position;
            stateIA = State.waitNextToEnemy;
        }
        void SetState_Retreat()
        {
            agent.destination = player.position;
            stateIA = State.retreat;
        }
        
        void AttackEnemy()
        {
            timerBetweenTwoAttacks += Time.deltaTime;
            if (enemy != null)
            {
                agent.transform.LookAt(new Vector3(enemy.transform.position.x, agent.transform.position.y, enemy.transform.position.z));
            }
            if (Vector3.Distance(agent.transform.position, enemy.transform.position) < distanceMinBetweenPantinIA + 1.0f)
            {
                if (timerBetweenTwoAttacks >= MaxTimerBetweenTwoAttacks)
                {
                    timerBetweenTwoAttacks = 0.0f;
                    SetDamages(Damages);
                    animator.SetTrigger("AttackSlight_01");
                }
            }
            else
            {
                timerBetweenTwoAttacks = 0.0f;
                SetState_ChargeIA();
            }
        }
        void SetDamages(int _damages)
        {
            Debug.Log(" AI_Partner: \"I attack\"");
            if (enemy != null)
            {
                if (enemy.GetComponent<AI_Enemy_Basic>() != null)
                {
                    //enemy.GetComponent<AI_Enemy_Basic>().TakeDamage(Damages)
                }
                else//security
                {
                    Debug.LogError("I want to make dammages but my enemy haven't \"AI_Enemy_Basic\" script!");
                }
            }
            else//security
            {
                Debug.LogError("I want to make dammages but i haven't enemy!");
            }
        }
        void GetPdv()
        {
            if (myCharacter.Health < minPdvBeforeFlee) // if I haven't lot of life... I Run AWAY!!!
            {
                // Here new state = state "run away"
                agent.destination = player.position;
                stateIA = State.retreat;
            }
            else if (myCharacter.Health <= 0)
            {
                // He kill me!!!
            }
        }
    }

}
