using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_EnemyStatePatrolHarasser : AI_EnemyStatePatrol
{
    int patrolIndex = 0;
    bool init = false;

    public override void OnBegin()
    {
        base.OnBegin();
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _animCroco, Animator _animWeapon, Animator _armorAnim, NavMeshAgent _agent, Rigidbody _rb, Vector3 _startPosition)
    {
        base.OnBegin(_individu, _animCroco, _animWeapon, _armorAnim, _agent, _rb, _startPosition);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _animCroco, Animator _animWeapon, Animator _armorAnim, NavMeshAgent _agent, Rigidbody _rb, List<Transform> _patrolPositions)
    {
        base.OnBegin(_individu, _animCroco, _animWeapon, _armorAnim, _agent, _rb, _patrolPositions);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _animCroco, Animator _animWeapon, Animator _armorAnim, NavMeshAgent _agent, Rigidbody _rb, Vector3 _startPosition, Transform _myTarget)
    {
        base.OnBegin(_individu, _animCroco, _animWeapon, _armorAnim, _agent, _rb, _startPosition, _myTarget);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _animCroco, Animator _animWeapon, Animator _armorAnim, NavMeshAgent _agent, Rigidbody _rb, List<Transform> _patrolPositions, Transform _myTarget)
    {
        base.OnBegin(_individu, _animCroco, _animWeapon, _armorAnim, _agent, _rb, _patrolPositions, _myTarget);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
    }

    protected override void CurrentUpdate()
    {
        base.CurrentUpdate();

        if (init == false)
        {
            patrolIndex = 0;
            myAgent.SetDestination(patrolPositions[patrolIndex].position);
            patrolIndex = (patrolIndex + 1) % patrolPositions.Count;
            init = true;
        }

        // Patrol : when reaching a patrol point, go to the next one
        if (myAgent.remainingDistance < minDistanceToTarget)
        {
            myAgent.SetDestination(patrolPositions[patrolIndex].position);
            patrolIndex = (patrolIndex + 1) % patrolPositions.Count;
        }

        if (currentTarget != null)
            (myIndividu as AI_Enemy_Harasser).ChangeState(AI_Enemy_Harasser.State.ChasingOnFlank);

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

        (myIndividu as AI_Enemy_Harasser).ChangeState(AI_Enemy_Harasser.State.IsHit);
    }
}
