using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_EnemyStateChaseHarasser : AI_EnemyStateChase
{

    enum SubState
    {
        waitAround,
        chase
    }
    SubState subState;

    public override void OnBegin()
    {
        base.OnBegin();
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;

        // Si le joueur me tourne le dos, je le chase, sinon je lui tourne autour
        if (Vector3.Dot(currentTarget.forward, myRb.transform.forward) < 0)
            BeginChase();
        else
           BeginWaitAround();

    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _animCroco, Animator _animWeapon, NavMeshAgent _agent, Rigidbody _rb, Vector3 _startPosition)
    {
        base.OnBegin(_individu, _animCroco, _animWeapon, _agent, _rb, _startPosition);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;

        // Si le joueur me tourne le dos, je le chase, sinon je lui tourne autour
        if (Vector3.Dot(currentTarget.forward, myRb.transform.forward) < 0)
            BeginChase();
        else
            BeginWaitAround();

    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _animCroco, Animator _animWeapon, NavMeshAgent _agent, Rigidbody _rb, List<Transform> _patrolPositions)
    {
        base.OnBegin(_individu, _animCroco, _animWeapon, _agent, _rb, _patrolPositions);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;

        // Si le joueur me tourne le dos, je le chase, sinon je lui tourne autour
        if (Vector3.Dot(currentTarget.forward, myRb.transform.forward) < 0)
            BeginChase();
        else
            BeginWaitAround();

    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _animCroco, Animator _animWeapon, NavMeshAgent _agent, Rigidbody _rb, Vector3 _startPosition, Transform _myTarget)
    {
        base.OnBegin(_individu, _animCroco, _animWeapon, _agent, _rb, _startPosition, _myTarget);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;

        // Si le joueur me tourne le dos, je le chase, sinon je lui tourne autour
        if (Vector3.Dot(currentTarget.forward, myRb.transform.forward) < 0)
            BeginChase();
        else
            BeginWaitAround();

    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _animCroco, Animator _animWeapon, NavMeshAgent _agent, Rigidbody _rb, List<Transform> _patrolPositions, Transform _myTarget)
    {
        base.OnBegin(_individu, _animCroco, _animWeapon, _agent, _rb, _patrolPositions, _myTarget);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;

        // Si le joueur me tourne le dos, je le chase, sinon je lui tourne autour
        if (Vector3.Dot(currentTarget.forward, myRb.transform.forward) < 0)
            BeginChase();
        else
            BeginWaitAround();

    }


    protected override void CurrentUpdate()
    {
        base.CurrentUpdate();

        switch(subState)
        {
            case SubState.waitAround:
                WaitAround();
                break;
            case SubState.chase:
                Chase();
                break;
        }

        // Final instruction of the global state update
        OnEndCurrentUpdate();
    }


    protected override void CurrentFixedUpdate()
    {
        base.CurrentFixedUpdate();
    }

    void BeginWaitAround()
    {

    }
    void WaitAround()
    {

    }

    void BeginChase()
    {
        subState = SubState.chase;

    }
    void Chase()
    {
        subState = SubState.waitAround;
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