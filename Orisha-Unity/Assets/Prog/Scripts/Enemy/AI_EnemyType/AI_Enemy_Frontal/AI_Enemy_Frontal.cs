﻿using System.Collections.Generic;
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

public class AI_Enemy_Frontal : AI_Enemy_Basic
{

    public enum State
    {
        Idle,
        Patroling,
        Chasing,
        Alert,
        Fighting,
        Esquive,
        ReplacingToFight,
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
    protected Vector3 startTransform;

    void Start ()
    {
        startTransform = transform.position;

        OnBegin();
        ChangeState(State.Idle, true);
        GetComponent<NavMeshAgent>().speed = speed;
        health = Basehealth;
        agentIsControlledByOther = false;
    }

    void Update ()
    {
        if (debugLog)
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
                    myCurrentState = new AI_EnemyStateIdleFrontal();
                    (myCurrentState as AI_EnemyStateIdleFrontal).OnBegin(this, GetComponentInChildren<Animator>(), GetComponent<NavMeshAgent>(), GetComponent<Rigidbody>(), startTransform);
                    break;
                case State.Alert:
                    myCurrentState = new AI_EnemyStateAlertFrontal();
                    (myCurrentState as AI_EnemyStateAlertFrontal).OnBegin(this, GetComponentInChildren<Animator>(), GetComponent<NavMeshAgent>(), GetComponent<Rigidbody>(), startTransform, currentTarget);
                    (myCurrentState as AI_EnemyStateAlertFrontal).InitCombat(abandonDistance, range, minDistanceToTarget, dieWhenTouchingTarget);
                    break;
                case State.Chasing:
                    myCurrentState = new AI_EnemyStateChaseFrontal();
                    (myCurrentState as AI_EnemyStateChaseFrontal).OnBegin(this, GetComponentInChildren<Animator>(), GetComponent<NavMeshAgent>(), GetComponent<Rigidbody>(), startTransform, currentTarget);
                    (myCurrentState as AI_EnemyStateChaseFrontal).InitCombat(abandonDistance, range, minDistanceToTarget, dieWhenTouchingTarget);
                    break;
                case State.Fighting:
                    myCurrentState = new AI_EnemyStateFightFrontal();
                    (myCurrentState as AI_EnemyStateFightFrontal).OnBegin(this, GetComponentInChildren<Animator>(), GetComponent<NavMeshAgent>(), GetComponent<Rigidbody>(), startTransform, currentTarget);
                    (myCurrentState as AI_EnemyStateFightFrontal).InitCombat(abandonDistance, range, minDistanceToTarget, dieWhenTouchingTarget);
                    break;
                case State.ReplacingToFight:
                    myCurrentState = new AI_EnemyStateReplaceToFightFrontal();
                    (myCurrentState as AI_EnemyStateReplaceToFightFrontal).OnBegin(this, GetComponentInChildren<Animator>(), GetComponent<NavMeshAgent>(), GetComponent<Rigidbody>(), startTransform, currentTarget);
                    (myCurrentState as AI_EnemyStateReplaceToFightFrontal).InitCombat(abandonDistance, range, minDistanceToTarget, dieWhenTouchingTarget);
                    break;
                case State.IsHit:
                    myCurrentState = new AI_EnemyStateIsHitFrontal();
                    (myCurrentState as AI_EnemyStateIsHitFrontal).OnBegin(this, GetComponentInChildren<Animator>(), GetComponent<NavMeshAgent>(), GetComponent<Rigidbody>(), startTransform, currentTarget);
                    break;
                case State.Die:
                    myCurrentState = new Ai_EnemyStateDieFrontal();
                    (myCurrentState as Ai_EnemyStateDieFrontal).OnBegin(this, GetComponentInChildren<Animator>(), GetComponent<NavMeshAgent>(), GetComponent<Rigidbody>(), startTransform);
                    StopAllCoroutines();
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

        if(health <= 0)
        {
            if (pe)
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
