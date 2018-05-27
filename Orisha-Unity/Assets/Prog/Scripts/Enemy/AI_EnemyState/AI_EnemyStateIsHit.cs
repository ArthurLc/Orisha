using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_EnemyStateIsHit : AI_EnemyState
{
    
    public override void OnBegin()
    {
        base.OnBegin();
		initState ();
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _animCroco, Animator _animWeapon, Animator _animArmor, NavMeshAgent _agent, Rigidbody _rb, Vector3 _startPosition)
    {
        base.OnBegin(_individu, _animCroco, _animWeapon, _animArmor, _agent, _rb, _startPosition);
        initState();
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _animCroco, Animator _animWeapon, Animator _animArmor, NavMeshAgent _agent, Rigidbody _rb, List<Transform> _patrolPositions)
    {
        base.OnBegin(_individu, _animCroco, _animWeapon, _animArmor, _agent, _rb, _patrolPositions);
		initState ();
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _animCroco, Animator _animWeapon, Animator _animArmor, NavMeshAgent _agent, Rigidbody _rb, Vector3 _startPosition, Transform _myTarget)
    {
        base.OnBegin(_individu, _animCroco, _animWeapon, _animArmor, _agent, _rb, _startPosition, _myTarget);
		initState ();
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _animCroco, Animator _animWeapon, Animator _animArmor, NavMeshAgent _agent, Rigidbody _rb, List<Transform> _patrolPositions, Transform _myTarget)
    {
        base.OnBegin(_individu, _animCroco, _animWeapon, _animArmor, _agent, _rb, _patrolPositions, _myTarget);
		initState ();
    }
    

	void initState()
	{
		if (myIndividu.Health > 0)
		{
            myIndividu.mydb.enabled = false;
			secureCount = 0;
			myAnimCroco.SetTrigger ("GetHit");
			myAnimWeapon.SetTrigger ("GetHit");
			myAnimArmor.SetTrigger ("GetHit");
        }
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
