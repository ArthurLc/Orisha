using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_EnemyStateIsHitFrontal : AI_EnemyStateIsHit
{
    public override void OnBegin()
    {
        base.OnBegin();
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
        myIndividu.agentIsControlledByOther = true;
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _animCroco, Animator _animWeapon, Animator _armorAnim, NavMeshAgent _agent, Rigidbody _rb, Vector3 _startPosition)
    {
        base.OnBegin(_individu, _animCroco, _animWeapon, _armorAnim, _agent, _rb, _startPosition);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
        myIndividu.agentIsControlledByOther = true;
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _animCroco, Animator _animWeapon, Animator _armorAnim, NavMeshAgent _agent, Rigidbody _rb, List<Transform> _patrolPositions)
    {
        base.OnBegin(_individu, _animCroco, _animWeapon, _armorAnim, _agent, _rb, _patrolPositions);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
        myIndividu.agentIsControlledByOther = true;
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _animCroco, Animator _animWeapon, Animator _armorAnim, NavMeshAgent _agent, Rigidbody _rb, Vector3 _startPosition, Transform _myTarget)
    {
        base.OnBegin(_individu, _animCroco, _animWeapon, _armorAnim, _agent, _rb, _startPosition, _myTarget);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
        myIndividu.agentIsControlledByOther = true;
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _animCroco, Animator _animWeapon, Animator _armorAnim, NavMeshAgent _agent, Rigidbody _rb, List<Transform> _patrolPositions, Transform _myTarget)
    {
        base.OnBegin(_individu, _animCroco, _animWeapon, _armorAnim, _agent, _rb, _patrolPositions, _myTarget);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
        myIndividu.agentIsControlledByOther = true;
    }


    protected override void CurrentUpdate()
    {
        base.CurrentUpdate();

        if (myIndividu.agentIsControlledByOther)
        {
            RaycastHit hit;
            ray.origin = myIndividu.transform.position + Vector3.up;
            ray.direction = Vector3.down;
            //Debug.DrawRay(ray.origin, ray.direction * 1.5f, Color.magenta);
            //Debug.Log("Velocity: " + myRb.velocity.magnitude);
            if (secureCount > 5 && myRb.velocity.magnitude < 2.0f && Physics.Raycast(ray.origin, ray.direction, out hit, 1.5f)) //Il est sur le sol
            {
                (myIndividu as AI_Enemy_Frontal).ChangeState(AI_Enemy_Frontal.State.Idle);
            }
        }
        else
        {
            (myIndividu as AI_Enemy_Frontal).ChangeState(AI_Enemy_Frontal.State.Idle);
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

        (myIndividu as AI_Enemy_Frontal).ChangeState(AI_Enemy_Frontal.State.IsHit, true);
    }
}
