using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_EnemyStateFleeTutorial : AI_EnemyStateFlee
{
    Transform playerTr;

    public override void OnBegin(AI_Enemy_Basic _individu, Animator _animCroco, Animator _animWeapon, NavMeshAgent _agent, Rigidbody _rb, List<Transform> _patrolPositions, Transform _myTarget, ref int _patrolIndex)
    {
        base.OnBegin(_individu, _animCroco, _animWeapon, _agent, _rb, _patrolPositions, _myTarget, ref _patrolIndex);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
        myAgent.SetDestination(patrolPositions[_patrolIndex].position);
        _patrolIndex = (_patrolIndex + 1) % patrolPositions.Count;
        playerTr = _myTarget;
    }


    protected override void CurrentUpdate()
    {
        base.CurrentUpdate();

        if (myAgent.remainingDistance < minDistanceToTarget)
        {
            (myIndividu as AI_Enemy_Tutorial).ChangeState(AI_Enemy_Tutorial.State.Taunt);
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
    }

    public override void OnEnd()
    {
        base.OnEnd();
        (myIndividu as AI_Enemy_Tutorial).LookAtPlayer = playerTr;
    }

    public override void PropulseAgent(Vector3 _dir)
    {
        base.PropulseAgent(_dir);

        (myIndividu as AI_Enemy_Tutorial).ChangeState(AI_Enemy_Tutorial.State.IsHit);
    }
}