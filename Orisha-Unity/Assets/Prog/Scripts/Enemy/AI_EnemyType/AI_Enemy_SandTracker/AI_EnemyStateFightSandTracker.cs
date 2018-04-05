using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_EnemyStateFightSandTracker : AI_EnemyStateFight
{
    bool hasAlreadyAttack;
    float timerBeforeEsquive = 1.5f;

    public override void OnBegin()
    {
        base.OnBegin();
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
        hasAlreadyAttack = false;
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _anim, NavMeshAgent _agent, Rigidbody _rb, Vector3 _startPosition)
    {
        base.OnBegin(_individu, _anim, _agent, _rb, _startPosition);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
        hasAlreadyAttack = false;
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _anim, NavMeshAgent _agent, Rigidbody _rb, List<Transform> _patrolPositions)
    {
        base.OnBegin(_individu, _anim, _agent, _rb, _patrolPositions);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
        hasAlreadyAttack = false;
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _anim, NavMeshAgent _agent, Rigidbody _rb, Vector3 _startPosition, Transform _myTarget)
    {
        base.OnBegin(_individu, _anim, _agent, _rb, _startPosition, _myTarget);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
        hasAlreadyAttack = false;
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _anim, NavMeshAgent _agent, Rigidbody _rb, List<Transform> _patrolPositions, Transform _myTarget)
    {
        base.OnBegin(_individu, _anim, _agent, _rb, _patrolPositions, _myTarget);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
        hasAlreadyAttack = false;
    }


    protected override void CurrentUpdate()
    {
        base.CurrentUpdate();


        if (dieWhenTouchingTarget)
        {
            return;
        }
        else
        {
            if (targetDistance > range)
            {
                myAgent.isStopped = false;
                (myIndividu as AI_Enemy_SandTracker).ChangeState(AI_Enemy_SandTracker.State.Chasing);

            }
            else if(!hasAlreadyAttack)
            {
                myAnim.SetTrigger("Attack");
                hasAlreadyAttack = true;
            }
            else if(timerBeforeEsquive >= 0.0f)
            {
                timerBeforeEsquive -= Time.deltaTime;
            }
            else
            {
                (myIndividu as AI_Enemy_SandTracker).ChangeState(AI_Enemy_SandTracker.State.Esquive);
                timerBeforeEsquive = 1.5f;
            }
        }

        // Final instruction of the global state update
        OnEndCurrentUpdate();
    }
    protected override void CurrentFixedUpdate()
    {
        base.CurrentFixedUpdate();
    }


    public override void OnEnd()
    {
        base.OnEnd();
    }


    public override void PropulseAgent(Vector3 _dir)
    {
        base.PropulseAgent(_dir);

        (myIndividu as AI_Enemy_SandTracker).ChangeState(AI_Enemy_SandTracker.State.IsHit);
    }
}

