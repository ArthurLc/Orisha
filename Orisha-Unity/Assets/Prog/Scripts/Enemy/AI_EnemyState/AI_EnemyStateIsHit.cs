using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_EnemyStateIsHit : AI_EnemyState
{
    
    public override void OnBegin()
    {
        base.OnBegin();
        secureCount = 0;
        myAnim.SetTrigger("GetHit");
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _anim, NavMeshAgent _agent, Rigidbody _rb, Vector3 _startPosition)
    {
        base.OnBegin(_individu, _anim, _agent, _rb, _startPosition);
        secureCount = 0;
        myAnim.SetTrigger("GetHit");
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _anim, NavMeshAgent _agent, Rigidbody _rb, List<Transform> _patrolPositions)
    {
        base.OnBegin(_individu, _anim, _agent, _rb, _patrolPositions);
        secureCount = 0;
        myAnim.SetTrigger("GetHit");
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _anim, NavMeshAgent _agent, Rigidbody _rb, Vector3 _startPosition, Transform _myTarget)
    {
        base.OnBegin(_individu, _anim, _agent, _rb, _startPosition, _myTarget);
        secureCount = 0;
        myAnim.SetTrigger("GetHit");
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _anim, NavMeshAgent _agent, Rigidbody _rb, List<Transform> _patrolPositions, Transform _myTarget)
    {
        base.OnBegin(_individu, _anim, _agent, _rb, _patrolPositions, _myTarget);
        secureCount = 0;
        myAnim.SetTrigger("GetHit");
    }
    
    protected override void CurrentUpdate()
    {
        base.CurrentUpdate();
        myAgent.updateRotation = !myIndividu.agentIsControlledByOther;
        myAgent.updatePosition = !myIndividu.agentIsControlledByOther;
        myAgent.velocity = Vector3.zero;
        if (myIndividu.agentIsControlledByOther)
        {
            myAgent.nextPosition = myAgent.transform.position;
            ++secureCount;
        }


    }

    protected override void CurrentFixedUpdate()
    {
        base.CurrentFixedUpdate();
    }

    public override void PropulseAgent(Vector3 _dir)
    {
        base.PropulseAgent(_dir);
    }

    public override void OnEnd()
    {
        base.OnEnd();
        myRb.isKinematic = true;
        myAgent.updateRotation = true;
        myAgent.updatePosition = true;
        myIndividu.agentIsControlledByOther = false;
    }
}
