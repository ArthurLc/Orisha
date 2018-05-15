using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_EnemyStateChaseFrontal : AI_EnemyStateChase
{
    public override void OnBegin()
    {
        base.OnBegin();
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
		Debug.Log ("Chase Begin");
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _animCroco, Animator _animWeapon, NavMeshAgent _agent, Rigidbody _rb, Vector3 _startPosition)
    {
        base.OnBegin(_individu, _animCroco, _animWeapon, _agent, _rb, _startPosition);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
		Debug.Log ("Chase Begin");
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _animCroco, Animator _animWeapon, NavMeshAgent _agent, Rigidbody _rb, List<Transform> _patrolPositions)
    {
        base.OnBegin(_individu, _animCroco, _animWeapon, _agent, _rb, _patrolPositions);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
		Debug.Log ("Chase Begin");
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _animCroco, Animator _animWeapon, NavMeshAgent _agent, Rigidbody _rb, Vector3 _startPosition, Transform _myTarget)
    {
        base.OnBegin(_individu, _animCroco, _animWeapon, _agent, _rb, _startPosition, _myTarget);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
		Debug.Log ("Chase Begin");
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _animCroco, Animator _animWeapon, NavMeshAgent _agent, Rigidbody _rb, List<Transform> _patrolPositions, Transform _myTarget)
    {
        base.OnBegin(_individu, _animCroco, _animWeapon, _agent, _rb, _patrolPositions, _myTarget);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
		Debug.Log ("Chase Begin");
    }


    protected override void CurrentUpdate()
    {
        base.CurrentUpdate();

        if (targetDistance <= range && targetDistance >= minDistanceToTarget) // la cible est à portée : ni trop près, ni trop loin
        {
            myAgent.isStopped = true;
            (myIndividu as AI_Enemy_Frontal).ChangeState(AI_Enemy_Frontal.State.Fighting);
            return;
        }
        else if (targetDistance >= abandonDistance) // Si la cible est beaucoup trop loin
        {
            // Abandon de la poursuite de la cible si elle s'éloigne trop
            (myIndividu as AI_Enemy_Frontal).ChangeState(AI_Enemy_Frontal.State.Idle);
            currentTarget = null;
            (myIndividu as AI_Enemy_Frontal).SetCurrentTarget(null);
        }
        else if (targetDistance <= minDistanceToTarget) // Si la cible est trop proche
        {
            (myIndividu as AI_Enemy_Frontal).ChangeState(AI_Enemy_Frontal.State.ReplacingToFight);
        }
        else // si la cible est un peu trop loin
        {
            myAgent.SetDestination(currentTarget.position);
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

        (myIndividu as AI_Enemy_Frontal).ChangeState(AI_Enemy_Frontal.State.IsHit);
        //OnBeginHit();
    }

}