using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_EnemyStatePatrolSandTracker : AI_EnemyStatePatrol
{
    private Vector3 destination;
	float timerBeforeSearchAnim;
	float searchAnimReload = 5f;
    public override void OnBegin()
    {
        base.OnBegin();
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
		timerBeforeSearchAnim = 0.0f;
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _animCroco, Animator _animWeapon, Animator _armorAnim, NavMeshAgent _agent, Rigidbody _rb, Vector3 _startPosition)
    {
        base.OnBegin(_individu, _animCroco, _animWeapon, _armorAnim, _agent, _rb, _startPosition);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
		timerBeforeSearchAnim = 0.0f;
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _animCroco, Animator _animWeapon, Animator _armorAnim, NavMeshAgent _agent, Rigidbody _rb, List<Transform> _patrolPositions)
    {
        base.OnBegin(_individu, _animCroco, _animWeapon, _armorAnim, _agent, _rb, _patrolPositions);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
		timerBeforeSearchAnim = 0.0f;
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _animCroco, Animator _animWeapon, Animator _armorAnim, NavMeshAgent _agent, Rigidbody _rb, Vector3 _startPosition, Transform _myTarget)
    {
        base.OnBegin(_individu, _animCroco, _animWeapon, _armorAnim, _agent, _rb, _startPosition, _myTarget);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
		timerBeforeSearchAnim = 0.0f;
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _animCroco, Animator _animWeapon, Animator _armorAnim, NavMeshAgent _agent, Rigidbody _rb, List<Transform> _patrolPositions, Transform _myTarget)
    {
        base.OnBegin(_individu, _animCroco, _animWeapon, _armorAnim, _agent, _rb, _patrolPositions, _myTarget);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
		timerBeforeSearchAnim = 0.0f;
    }


    protected override void CurrentUpdate()
    {
        base.CurrentUpdate();

        SearchTarget();

		timerBeforeSearchAnim -= Time.deltaTime;
        // if enemy found, chase it
        if (currentTarget != null)
        {
			if (myIndividu.AsCallReinforcement == false)
            {
                myAnimCroco.SetTrigger("Detected");
                myAnimWeapon.SetTrigger("Detected");
                myAnimArmor.SetTrigger("Detected");
                (myIndividu as AI_Enemy_SandTracker).ChangeState (AI_Enemy_SandTracker.State.Alert);
            }
			else 
			{
				(myIndividu as AI_Enemy_SandTracker).ChangeState(AI_Enemy_SandTracker.State.Chasing);
				myAnimCroco.SetTrigger ("Detected");
				myAnimWeapon.SetTrigger ("Detected");
				myAnimArmor.SetTrigger ("Detected");
            }
        }
        else
        {
            if (myAgent.remainingDistance < minDistanceToTarget)
            {
                Vector3 direction = new Vector3(Random.Range(-100.0f, 100.0f), myIndividu.transform.position.y, Random.Range(-100.0f, 100.0f));
                direction = Vector3.Normalize(direction);

				if (timerBeforeSearchAnim < 0f) 
				{
					myAnimCroco.SetTrigger ("Search");
					myAnimWeapon.SetTrigger ("Search");
					myAnimArmor.SetTrigger ("Search");

                    timerBeforeSearchAnim = searchAnimReload;
				}
            
                
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