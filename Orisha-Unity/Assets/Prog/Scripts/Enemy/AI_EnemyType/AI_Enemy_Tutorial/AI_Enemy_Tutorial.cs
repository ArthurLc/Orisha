using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/*
* @ArthurLacour
* @AI_Enemy_Tutorial.cs
* @04/05/2018
* @Le Script s'attache a chaque ennemi
*/

public class AI_Enemy_Tutorial : AI_Enemy_Basic
{
    // Lieu d'idle
    protected Vector3 startTransform;
    [SerializeField] private List<Transform> patrolTransform;
    protected int patrolIndex;

    Potential_Enemy pe;

    [HideInInspector] public Transform LookAtPlayer;

    [SerializeField] private bool asCrocoHaveTheMask;
    private bool asCrocoEquippedTheMask;
    [SerializeField][Range(0.5f, 8.0f)] private float emissiveApparitionSpeed = 3.0f;
    [SerializeField][Range(0.0f, 1.0f)] private float emissiveIntensityMax = 1.0f;

    private SandShaderPositionner sandShaderPos;
    public SandShaderPositionner SandShaderPos
    {
        get { return sandShaderPos; }
    }

    void Start ()
    {
        startTransform = transform.position;

        OnBegin();
        myAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        sandShaderPos = GetComponentInChildren<SandShaderPositionner>();
        myAgent.speed = sprintSpeed;
        health = Basehealth;
        agentIsControlledByOther = false;
        patrolIndex = 0;

        LookAtPlayer = null;

        ChangeState(State.Idle, true);

        asCrocoEquippedTheMask = asCrocoHaveTheMask;
        CrocoMaterial.SetFloat("_EmissiveIntensity", 0.0f);

        if (crocoAnim == null) {
            Debug.LogError("Il manque le link de l'Animator du Croco !");
            Destroy(this);
        }
    }

    void OnApplicationQuit()
    {
        CrocoMaterial.SetFloat("_EmissiveIntensity", 1.0f);
    }

    void Update ()
    {
        if(debugLog)
            Debug.Log("State: " + state);

        UpdateDmgBoxList();
        if(!isFreeze && myCurrentState != null && myCurrentState.UpdateState != null)
            myCurrentState.UpdateState();

        if (asCrocoEquippedTheMask == false && asCrocoHaveTheMask)
            EquipMask();
    }


    private void FixedUpdate()
    {
        if (!isFreeze && myCurrentState != null && myCurrentState.FixedUpdateState != null)
            myCurrentState.FixedUpdateState();
    }

    private void OnDisable()
    {
        if (pe)
        {
            pe.Pop_Potential_Ennemy(this);
        }
    }

    public void ChangeState(State _newState, bool forceChange = false)
    {
        if (_newState != state || forceChange)
        {
            if(myCurrentState != null)
                myCurrentState.OnEnd();

            switch (_newState)
            {
                case State.Idle:
                    myCurrentState = new AI_EnemyStateIdleTutorial();
                    (myCurrentState as AI_EnemyStateIdleTutorial).OnBegin(this, crocoAnim, weaponAnim, armorAnim, myAgent, rb, patrolTransform);
                    break;
                case State.Taunt:
                    myCurrentState = new AI_EnemyStateTauntTutorial();
                    (myCurrentState as AI_EnemyStateTauntTutorial).OnBegin(this, crocoAnim, weaponAnim, armorAnim, myAgent, rb, patrolTransform);
                    break;
                case State.Patroling:
                    myCurrentState = new AI_EnemyStatePatrolTutorial();
                    (myCurrentState as AI_EnemyStatePatrolTutorial).OnBegin(this, crocoAnim, weaponAnim, armorAnim, myAgent, rb, patrolTransform, currentTarget);
                    (myCurrentState as AI_EnemyStatePatrolTutorial).InitCombat(abandonDistance, range, myAgent.stoppingDistance, dieWhenTouchingTarget);
                    break;
                case State.Alert:
                    myCurrentState = new AI_EnemyStateAlertTutorial();
                    (myCurrentState as AI_EnemyStateAlertTutorial).OnBegin(this, crocoAnim, weaponAnim, armorAnim, myAgent, rb, patrolTransform, currentTarget);
                    (myCurrentState as AI_EnemyStateAlertTutorial).InitCombat(abandonDistance, range, myAgent.stoppingDistance, dieWhenTouchingTarget);
                    break;
                case State.Fighting:
                    myCurrentState = new AI_EnemyStateFightTutorial();
                    (myCurrentState as AI_EnemyStateFightTutorial).OnBegin(this, crocoAnim, weaponAnim, armorAnim, myAgent, rb, patrolTransform, currentTarget);
                    (myCurrentState as AI_EnemyStateFightTutorial).InitCombat(abandonDistance, range, myAgent.stoppingDistance, dieWhenTouchingTarget);
                    break;
                case State.IsHit:
                    myCurrentState = new AI_EnemyStateIsHitTutorial();
                    (myCurrentState as AI_EnemyStateIsHitTutorial).OnBegin(this, crocoAnim, weaponAnim, armorAnim, myAgent, rb, patrolTransform, currentTarget);
                    break;
                case State.Die:
                    myCurrentState = new Ai_EnemyStateDieTutorial();
                    (myCurrentState as Ai_EnemyStateDieTutorial).OnBegin(this, crocoAnim, weaponAnim, armorAnim, myAgent, rb, patrolTransform);
                    StopAllCoroutines();
                    break;
                case State.Fleeing:
                    myCurrentState = new AI_EnemyStateFleeTutorial();
                    (myCurrentState as AI_EnemyStateFleeTutorial).OnBegin(this, crocoAnim, weaponAnim, armorAnim, myAgent, rb, patrolTransform, currentTarget, ref patrolIndex);
                    (myCurrentState as AI_EnemyStateFleeTutorial).InitCombat(abandonDistance, range, myAgent.stoppingDistance, dieWhenTouchingTarget);
                    break;

            }

            state = _newState;
            Debug.Assert(myCurrentState != null, "myCurrentState == null => impossible");     
        }
    }

    private void UpdateDmgBoxList()
    {
        for(int nbBox = 0; nbBox < DmgBoxList.Count; nbBox++)
        {
            if (DmgBoxList[nbBox].enabled == false)
                DmgBoxList.Remove(DmgBoxList[nbBox]);
        }
    }

    public override void TakeDamage(int damages)
    {
        base.TakeDamage(damages);

        if (health <= 0)
        {
            if(pe)
                pe.Pop_Potential_Ennemy(this);

            if(myState != State.Die)
                ChangeState(State.Die, false);
        }
    }

    private IEnumerator PotentialEnemypopRequest(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        Collider[] col = Physics.OverlapSphere(transform.position, 10.0f, 1 << 16);
        if (col.Length == 0)
        {
            pe.Pop_Potential_Ennemy(this);
        }
        //else
        //{
        //    for (int i = 0; i < col.Length; i++)
        //        Debug.Log(col[i].name);
        //}

        yield break;
    }


    private void EquipMask()
    {
        asCrocoEquippedTheMask = true;
        StartCoroutine(EquipLerpMask());
    }

    private IEnumerator EquipLerpMask()
    {
        float timer = 0.0f;

        while(timer < 1.0f)
        {
            timer = Mathf.Clamp(timer + (Time.fixedDeltaTime * emissiveApparitionSpeed), 0.0f, emissiveIntensityMax);
            CrocoMaterial.SetFloat("_EmissiveIntensity", timer);
            yield return null;
        }

        yield return null;
    }


    private void OnTriggerEnter(Collider other)
    {
        //if (debugEnabled)
        if (other.tag == "Player")
        {
            if (targetsInReach.Contains(other.transform) == false)
                targetsInReach.Add(other.transform);
            //StopAllCoroutines();
            if (pe && health > 0)
            {
                pe.Add_Potential_Ennemy(this);
            }
            else if (!pe && health > 0)
            {
                if(string.Compare(other.name,"HB_Player") == 0)
                    pe = other.GetComponentInParent<Potential_Enemy>();
                else
                    pe = other.GetComponent<Potential_Enemy>();

                if (pe)
                    pe.Add_Potential_Ennemy(this);
                else
                    Debug.Log("No Potential_Enemy script on : " + other.name + " or :" + other.transform.parent.name);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //if (debugEnabled)
        if (other.tag == "Player")
        {
            if (targetsInReach.Contains(other.transform) == true)
                targetsInReach.Remove(other.transform);

            if (pe)
            {
                StopCoroutine(PotentialEnemypopRequest(2.0f));
                StartCoroutine(PotentialEnemypopRequest(2.0f));
            }
        }
    }

}
