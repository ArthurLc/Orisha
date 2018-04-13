using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_EnemyStateAlert : AI_EnemyState
{
    // combats skills
    protected float abandonDistance;
    protected float range;
    protected float minDistanceToTarget;
    protected bool dieWhenTouchingTarget;

    protected Coroutine myCoroutine;

    public override void OnBegin()
    {
        base.OnBegin();
        LaunchAlert();
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _animCroco, Animator _animWeapon, NavMeshAgent _agent, Rigidbody _rb, Vector3 _startPosition)
    {
        base.OnBegin(_individu, _animCroco, _animWeapon, _agent, _rb, _startPosition);
        LaunchAlert();
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _animCroco, Animator _animWeapon, NavMeshAgent _agent, Rigidbody _rb, List<Transform> _patrolPositions)
    {
        base.OnBegin(_individu, _animCroco, _animWeapon, _agent, _rb, _patrolPositions);
        LaunchAlert();
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _animCroco, Animator _animWeapon, NavMeshAgent _agent, Rigidbody _rb, Vector3 _startPosition, Transform _myTarget)
    {
        base.OnBegin(_individu, _animCroco, _animWeapon, _agent, _rb, _startPosition, _myTarget);
        LaunchAlert();
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _animCroco, Animator _animWeapon, NavMeshAgent _agent, Rigidbody _rb, List<Transform> _patrolPositions, Transform _myTarget)
    {
        base.OnBegin(_individu, _animCroco, _animWeapon, _agent, _rb, _patrolPositions, _myTarget);
        LaunchAlert();
    }


    protected override void CurrentUpdate()
    {
        base.CurrentUpdate();

    }

    protected override void CurrentFixedUpdate()
    {
        base.CurrentFixedUpdate();
    }

    protected void LaunchAlert()
    {
        myCoroutine = null;
        myIndividu.transform.LookAt(currentTarget);
        myIndividu.transform.rotation = Quaternion.Euler(0.0f, myIndividu.transform.rotation.eulerAngles.y, 0.0f);
        myAnimCroco.SetTrigger("Alert");
        myAnimWeapon.SetTrigger("Alert");
        myAgent.destination = myIndividu.transform.position;
        myIndividu.AsCallReinforcement = true;
    }


    public override void OnEnd()
    {
        base.OnEnd();
    }

    public void InitCombat(float _abandonDistance, float _range, float _minDistanceToTarget, bool _dieWhenTouchingTarget)
    {
        abandonDistance = _abandonDistance;
        range = _range;
        minDistanceToTarget = _minDistanceToTarget;
        dieWhenTouchingTarget = _dieWhenTouchingTarget;
    }

}

