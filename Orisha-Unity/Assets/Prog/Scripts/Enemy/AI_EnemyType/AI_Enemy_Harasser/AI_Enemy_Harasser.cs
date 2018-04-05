using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/*
* AI_Enemy_Harasser.cs
* 22/01/2017
* Comportement d'un ennemi (doit avoir un NavMeshAgent)
*
* Tendances: Patrouille, fonce sur le joueur en essayant de le frapper sur le flanc, une fois qu'il a touché/été touché, fuit puis revient à l'attaque en contournant encore
*/

public class AI_Enemy_Harasser : AI_Enemy_Basic
{

    public enum State
    {
        Patroling,
        Alert,
        ChasingOnFlank,
        Fighting,
        Fleeing,
        IsHit,
        Die
    }

    [Header("AI")]
    [SerializeField]
    private State state;
    public State myState
    {
        get
        {
            return state;
        }

        set
        {
            state = value;
        }
    }

    Potential_Enemy pe;

    // Lieu d'idle
    [SerializeField]private List<Transform> patrolTransform;

    void Start()
    {
        OnBegin();
        ChangeState(State.Patroling, true);
        GetComponent<NavMeshAgent>().speed = speed;
        health = Basehealth;
        agentIsControlledByOther = false;

        Debug.Assert(patrolTransform != null, "Enemy Harasser: missing patrol transforms");

    }

    void Update()
    {
        if (debugLog)
            Debug.Log("State: " + state);

        UpdateDmgBoxList();
        if (!isFreeze && myCurrentState != null && myCurrentState.UpdateState != null)
            myCurrentState.UpdateState();
    }


    private void FixedUpdate()
    {
        if (!isFreeze && myCurrentState != null && myCurrentState.FixedUpdateState != null)
            myCurrentState.FixedUpdateState();
    }


    public void ChangeState(State _newState, bool forceChange = false)
    {
        if (_newState != state || forceChange)
        {
            if (myCurrentState != null)
                myCurrentState.OnEnd();

            switch (_newState)
            {

                case State.Patroling:
                    myCurrentState = new AI_EnemyStatePatrolHarasser();
                    (myCurrentState as AI_EnemyStatePatrolHarasser).OnBegin(this, GetComponentInChildren<Animator>(), GetComponent<NavMeshAgent>(), GetComponent<Rigidbody>(), patrolTransform, currentTarget);
                    (myCurrentState as AI_EnemyStatePatrolHarasser).InitCombat(abandonDistance, range, minDistanceToTarget, dieWhenTouchingTarget);
                    break;
                case State.Alert:
                    myCurrentState = new AI_EnemyStateAlertHarasser();
                    (myCurrentState as AI_EnemyStateAlertHarasser).OnBegin(this, GetComponentInChildren<Animator>(), GetComponent<NavMeshAgent>(), GetComponent<Rigidbody>(), patrolTransform, currentTarget);
                    (myCurrentState as AI_EnemyStateAlertHarasser).InitCombat(abandonDistance, range, minDistanceToTarget, dieWhenTouchingTarget);
                    break;
                case State.Fighting:
                    myCurrentState = new AI_EnemyStateFightHarasser();
                    (myCurrentState as AI_EnemyStateFightHarasser).OnBegin(this, GetComponentInChildren<Animator>(), GetComponent<NavMeshAgent>(), GetComponent<Rigidbody>(), patrolTransform, currentTarget);
                    (myCurrentState as AI_EnemyStateFightHarasser).InitCombat(abandonDistance, range, minDistanceToTarget, dieWhenTouchingTarget);
                    break;
                case State.IsHit:
                    myCurrentState = new AI_EnemyStateIsHitHarasser();
                    (myCurrentState as AI_EnemyStateIsHitHarasser).OnBegin(this, GetComponentInChildren<Animator>(), GetComponent<NavMeshAgent>(), GetComponent<Rigidbody>(), patrolTransform);
                    break;
                case State.Die:
                    myCurrentState = new AI_EnemyStateDieHarasser();
                    (myCurrentState as AI_EnemyStateDieHarasser).OnBegin(this, GetComponentInChildren<Animator>(), GetComponent<NavMeshAgent>(), GetComponent<Rigidbody>(), patrolTransform);
                    StopAllCoroutines();
                    break;

            }

            state = _newState;
            Debug.Assert(myCurrentState != null, "myCurrentState == null => impossible");
        }
    }

    private void UpdateDmgBoxList()
    {
        for (int nbBox = 0; nbBox < DmgBoxList.Count; nbBox++)
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
            if (myState != State.Die)
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
            else if (!pe && !pe && health > 0)
            {
                pe = other.GetComponent<Potential_Enemy>();
                pe.Add_Potential_Ennemy(this);
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
            else if (!pe)
            {
                pe = other.GetComponent<Potential_Enemy>();

                if (pe)
                {
                    StopCoroutine(PotentialEnemypopRequest(2.0f));
                    StartCoroutine(PotentialEnemypopRequest(2.0f));
                }
                else
                    Debug.Log("Triggered by an object without Potential_Enemy script : " + other.name);

            }
        }
    }
}
