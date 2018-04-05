using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_EnemyStateEsquiveSandTracker : AI_EnemyStateReplaceToFight
{
    public enum EsquiveState
    {
        GoesUnderSand,
        InDeplacement,
        GoesUpperSand
    }
    private EsquiveState myEsquiveState;

    private float moveSpeed = 3.5f;
    private float enemySize = 3.0f;
    private float currentYPos = 0.0f;
    private float initialeYdeltaPos;

    private Vector3 destination;

    private HittenBox_Enemy[] allHittenBox;

    public override void OnBegin()
    {
        base.OnBegin();
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
        myEsquiveState = EsquiveState.GoesUnderSand;
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _anim, NavMeshAgent _agent, Rigidbody _rb, Vector3 _startPosition)
    {
        base.OnBegin(_individu, _anim, _agent, _rb, _startPosition);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
        myEsquiveState = EsquiveState.GoesUnderSand;
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _anim, NavMeshAgent _agent, Rigidbody _rb, List<Transform> _patrolPositions)
    {
        base.OnBegin(_individu, _anim, _agent, _rb, _patrolPositions);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
        myEsquiveState = EsquiveState.GoesUnderSand;
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _anim, NavMeshAgent _agent, Rigidbody _rb, Vector3 _startPosition, Transform _myTarget)
    {
        base.OnBegin(_individu, _anim, _agent, _rb, _startPosition, _myTarget);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
        myEsquiveState = EsquiveState.GoesUnderSand;
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _anim, NavMeshAgent _agent, Rigidbody _rb, List<Transform> _patrolPositions, Transform _myTarget)
    {
        base.OnBegin(_individu, _anim, _agent, _rb, _patrolPositions, _myTarget);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
        myEsquiveState = EsquiveState.GoesUnderSand;
    }


    protected override void CurrentUpdate()
    {
        base.CurrentUpdate();

        // Final instruction of the global state update
        OnEndCurrentUpdate();
    }

    protected override void CurrentFixedUpdate()
    {
        base.CurrentFixedUpdate();

        if (myEsquiveState == EsquiveState.GoesUnderSand)
        {
            
            myAgent.isStopped = true;
            myAnim.transform.position -= Vector3.up * (Time.deltaTime * moveSpeed);
            currentYPos += (Time.deltaTime * moveSpeed);
            myIndividu.GetComponentInChildren<SandShaderPositionner>().HightUpdate(((currentYPos > 0.1f) ? currentYPos : 0.1f) * 1.0f);

            if (currentYPos >= enemySize)
            {
                foreach (HittenBox_Enemy hbe in allHittenBox)
                {
                    hbe.gameObject.SetActive(false);
                }

                myEsquiveState = EsquiveState.InDeplacement;

                Vector3 tempDir = currentTarget.transform.position - myIndividu.transform.position;
                tempDir = Vector3.Normalize(tempDir);
                destination = currentTarget.transform.position + (tempDir * minDistanceToTarget * 1.5f);

                destination = PositionOnNavMesh(destination);

                myAgent.destination = destination;
                
            }
        }
        else if (myEsquiveState == EsquiveState.InDeplacement)
        {
            myAgent.isStopped = false;
            
            if (Vector3.Distance(myIndividu.transform.position, destination) < minDistanceToTarget * 0.75f)
            {
                myEsquiveState = EsquiveState.GoesUpperSand;
            }
        }
        else
        {
            myAgent.isStopped = true;
            myAnim.transform.position += Vector3.up * (Time.deltaTime * moveSpeed);
            currentYPos -= (Time.deltaTime * moveSpeed);
            myIndividu.GetComponentInChildren<SandShaderPositionner>().HightUpdate(((currentYPos > 0.1f) ? currentYPos : 0.1f) * 1.0f);
            if (currentYPos <= 0.0f)
            {
                myIndividu.GetComponentInChildren<SandShaderPositionner>().Activate(false);
                myAnim.transform.position = new Vector3(myAnim.transform.position.x, myIndividu.transform.position.y + initialeYdeltaPos, myAnim.transform.position.z);
                myAgent.isStopped = false;

                foreach (HittenBox_Enemy hbe in allHittenBox)
                {
                    hbe.gameObject.SetActive(true);
                }


                (myIndividu as AI_Enemy_SandTracker).ChangeState(AI_Enemy_SandTracker.State.Fighting);
            }
        }
    }

    public override void InitCombat(float _abandonDistance, float _range, float _minDistanceToTarget, bool _dieWhenTouchingTarget)
    {
        base.InitCombat(_abandonDistance, _range, _minDistanceToTarget, _dieWhenTouchingTarget);
        initialeYdeltaPos = myAnim.transform.position.y - myIndividu.transform.position.y;

        allHittenBox = myIndividu.GetComponentsInChildren<HittenBox_Enemy>();        

    }

    public override void OnEnd()
    {
        base.OnEnd();
    }

    public override void PropulseAgent(Vector3 _dir)
    {
        myAnim.transform.position = new Vector3(myAnim.transform.position.x, myIndividu.transform.position.y + initialeYdeltaPos, myAnim.transform.position.z);
        myIndividu.GetComponentInChildren<SandShaderPositionner>().Activate(false);
        foreach (HittenBox_Enemy hbe in allHittenBox)
        {
            hbe.gameObject.SetActive(true);
        }

        base.PropulseAgent(_dir);

        (myIndividu as AI_Enemy_SandTracker).ChangeState(AI_Enemy_SandTracker.State.IsHit);
    }
}
