using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_EnemyStatePatrolSandTracker : AI_EnemyStatePatrol
{
    private Vector3 destination;

    public override void OnBegin()
    {
        base.OnBegin();
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _animCroco, Animator _animWeapon, NavMeshAgent _agent, Rigidbody _rb, Vector3 _startPosition)
    {
        base.OnBegin(_individu, _animCroco, _animWeapon, _agent, _rb, _startPosition);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _animCroco, Animator _animWeapon, NavMeshAgent _agent, Rigidbody _rb, List<Transform> _patrolPositions)
    {
        base.OnBegin(_individu, _animCroco, _animWeapon, _agent, _rb, _patrolPositions);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _animCroco, Animator _animWeapon, NavMeshAgent _agent, Rigidbody _rb, Vector3 _startPosition, Transform _myTarget)
    {
        base.OnBegin(_individu, _animCroco, _animWeapon, _agent, _rb, _startPosition, _myTarget);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _animCroco, Animator _animWeapon, NavMeshAgent _agent, Rigidbody _rb, List<Transform> _patrolPositions, Transform _myTarget)
    {
        base.OnBegin(_individu, _animCroco, _animWeapon, _agent, _rb, _patrolPositions, _myTarget);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
    }


    protected override void CurrentUpdate()
    {
        base.CurrentUpdate();

        SearchTarget();

        // if enemy found, chase it
        if (currentTarget != null)
        {
            if (myIndividu.AsCallReinforcement == false)
                (myIndividu as AI_Enemy_SandTracker).ChangeState(AI_Enemy_SandTracker.State.Alert);
            else
                (myIndividu as AI_Enemy_SandTracker).ChangeState(AI_Enemy_SandTracker.State.Chasing);
        }
        else
        {
            if (myAgent.remainingDistance < minDistanceToTarget)
            {
                Vector3 direction = new Vector3(Random.Range(-100.0f, 100.0f), myIndividu.transform.position.y, Random.Range(-100.0f, 100.0f));
                direction = Vector3.Normalize(direction);
				myAnimCroco.SetTrigger ("Search");
				myAnimWeapon.SetTrigger ("Search");
                destination = PositionOnNavMesh(startTransform + (direction * Random.Range(5.0f, 20.0f)));
                myAgent.SetDestination(destination);
            }            
        }

        // Final instruction of the global state update
        OnEndCurrentUpdate();
    }

    protected override void CurrentFixedUpdate()
    {
        base.CurrentFixedUpdate();
    }

    public override void InitCombat(float _abandonDistance, float _range, float _minDistanceToTarget, bool _dieWhenTouchingTarget)
    {
        base.InitCombat(_abandonDistance, _range, _minDistanceToTarget, _dieWhenTouchingTarget);
        destination = myIndividu.transform.position;
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