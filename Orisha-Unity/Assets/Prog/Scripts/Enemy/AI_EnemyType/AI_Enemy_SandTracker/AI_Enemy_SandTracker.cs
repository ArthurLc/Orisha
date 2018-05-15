using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/*
* @GauthierPierre
* @Temp_AI_Enemy.cs
* @23/10/2017
* @ - Le Script s'attache a chaque ennemi
*   - Doit doit avoir ules deux joueurs en référence.
*/

public class AI_Enemy_SandTracker : AI_Enemy_Basic
{
    // Lieu d'idle
    protected Vector3 startTransform;

    Potential_Enemy pe;


    
    Rigidbody rb;
    [Header("Links")]
	[SerializeField] Material weaponSandMaterial;

    private SandShaderPositionner sandShaderPos;
    public SandShaderPositionner SandShaderPos
    {
        get { return sandShaderPos; }
    }

	private SandShaderPositionner weaponSandShaderPos;
	public SandShaderPositionner WeaponSandShaderPos
	{
		get { return weaponSandShaderPos; }
	}

    void Start ()
    {
        startTransform = transform.position;

        OnBegin();
        myAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
		sandShaderPos = GetComponentInChildren<SandShaderPositionner>();
		weaponSandShaderPos = weaponAnim.GetComponentInChildren<SandShaderPositionner>();
        myAgent.speed = speed;
        health = Basehealth;
        agentIsControlledByOther = false;

        ChangeState(State.Idle, true);

        if (crocoAnim == null) {
            Debug.LogError("Il manque le link de l'Animator du Croco !");
            Destroy(this);
        }
    }
	
	void Update ()
    {
        if(debugLog)
            Debug.Log("State: " + state);

        UpdateDmgBoxList();
        if(!isFreeze && myCurrentState != null && myCurrentState.UpdateState != null)
            myCurrentState.UpdateState(); 
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
                    myCurrentState = new AI_EnemyStateIdleSandTracker();
                    (myCurrentState as AI_EnemyStateIdleSandTracker).OnBegin(this, crocoAnim, weaponAnim, myAgent, rb, startTransform);
                    break;
                case State.Patroling:
                    myCurrentState = new AI_EnemyStatePatrolSandTracker();
                    (myCurrentState as AI_EnemyStatePatrolSandTracker).OnBegin(this, crocoAnim, weaponAnim, myAgent, rb, startTransform, currentTarget);
                    (myCurrentState as AI_EnemyStatePatrolSandTracker).InitCombat(abandonDistance, range, myAgent.stoppingDistance, dieWhenTouchingTarget);
                    myAgent.speed = walkSpeed;
                    break;
                case State.Alert:
                    myCurrentState = new AI_EnemyStateAlertSandTracker();
                    (myCurrentState as AI_EnemyStateAlertSandTracker).OnBegin(this, crocoAnim, weaponAnim, myAgent, rb, startTransform, currentTarget);
                    (myCurrentState as AI_EnemyStateAlertSandTracker).InitCombat(abandonDistance, range, myAgent.stoppingDistance, dieWhenTouchingTarget);
                    break;
                case State.Chasing:
                    myCurrentState = new AI_EnemyStateChaseSandTracker();
                    (myCurrentState as AI_EnemyStateChaseSandTracker).OnBegin(this, crocoAnim, weaponAnim, myAgent, rb, startTransform, currentTarget);
                    (myCurrentState as AI_EnemyStateChaseSandTracker).InitCombat(abandonDistance, range, myAgent.stoppingDistance, dieWhenTouchingTarget);
                    myAgent.speed = speed;
                    break;
                case State.Fighting:
                    myCurrentState = new AI_EnemyStateFightSandTracker();
                    (myCurrentState as AI_EnemyStateFightSandTracker).OnBegin(this, crocoAnim, weaponAnim, myAgent, rb, startTransform, currentTarget);
                    (myCurrentState as AI_EnemyStateFightSandTracker).InitCombat(abandonDistance, range, myAgent.stoppingDistance, dieWhenTouchingTarget);

                    break;
                case State.Esquive:
                      myCurrentState = new AI_EnemyStateEsquiveSandTracker();
                    (myCurrentState as AI_EnemyStateEsquiveSandTracker).OnBegin(this, crocoAnim, weaponAnim, myAgent, rb, startTransform, currentTarget);
                    (myCurrentState as AI_EnemyStateEsquiveSandTracker).InitCombat(abandonDistance, range, myAgent.stoppingDistance, dieWhenTouchingTarget);
                    myAgent.speed = speed;
                    break;
                case State.Die:
                    myCurrentState = new Ai_EnemyStateDieSandTracker();
                    (myCurrentState as Ai_EnemyStateDieSandTracker).OnBegin(this, crocoAnim, weaponAnim, myAgent, rb, startTransform);
                    StopAllCoroutines();
                    break;
                case State.IsHit:
                    myCurrentState = new AI_EnemyStateIsHitSandTracker();
                    (myCurrentState as AI_EnemyStateIsHitSandTracker).OnBegin(this, crocoAnim, weaponAnim, myAgent, rb, startTransform, currentTarget);
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
