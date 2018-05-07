using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_EnemyStateEsquiveSandTracker : AI_EnemyStateReplaceToFight
{
    public enum EsquiveState
    {
        GoesUnderSand,
        InDeplacement,
        GoesUpperSand
    }
    private EsquiveState myEsquiveState;

    private float moveSpeed = 3.5f;
    private float enemySize = 3.0f;
    private float currentYPos = 0.0f;
    private float initialeYdeltaPos;

    private Vector3 destination;

    private HittenBox_Enemy[] allHittenBox;

    public override void OnBegin()
    {
        base.OnBegin();
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
        myEsquiveState = EsquiveState.GoesUnderSand;
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _animCroco, Animator _animWeapon, NavMeshAgent _agent, Rigidbody _rb, Vector3 _startPosition)
    {
        base.OnBegin(_individu, _animCroco, _animWeapon, _agent, _rb, _startPosition);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
        myEsquiveState = EsquiveState.GoesUnderSand;
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _animCroco, Animator _animWeapon, NavMeshAgent _agent, Rigidbody _rb, List<Transform> _patrolPositions)
    {
        base.OnBegin(_individu, _animCroco, _animWeapon, _agent, _rb, _patrolPositions);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
        myEsquiveState = EsquiveState.GoesUnderSand;
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _animCroco, Animator _animWeapon, NavMeshAgent _agent, Rigidbody _rb, Vector3 _startPosition, Transform _myTarget)
    {
        base.OnBegin(_individu, _animCroco, _animWeapon, _agent, _rb, _startPosition, _myTarget);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
        myEsquiveState = EsquiveState.GoesUnderSand;
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _animCroco, Animator _animWeapon, NavMeshAgent _agent, Rigidbody _rb, List<Transform> _patrolPositions, Transform _myTarget)
    {
        base.OnBegin(_individu, _animCroco, _animWeapon, _agent, _rb, _patrolPositions, _myTarget);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
        myEsquiveState = EsquiveState.GoesUnderSand;
    }


    protected override void CurrentUpdate()
    {
        base.CurrentUpdate();

        // Final instruction of the global state update
        OnEndCurrentUpdate();
    }

	void WeaponSandShader_GoesUnderSand()
	{
		myAnimWeapon.transform.position -= Vector3.up * (Time.deltaTime * moveSpeed);

		//((AI_Enemy_SandTracker)myIndividu).WeaponSandShaderPos.HightUpdate(((currentYPos > 0.1f) ? currentYPos : 0.1f) * 1.0f);
	}

	void WeaponSandShader_GoesUpperSand()
	{
		myAnimWeapon.transform.position += Vector3.up * (Time.deltaTime * moveSpeed);

		//((AI_Enemy_SandTracker)myIndividu).WeaponSandShaderPos.HightUpdate(((currentYPos > 0.1f) ? currentYPos : 0.1f) * 1.0f);
	}

    protected override void CurrentFixedUpdate()
    {
        base.CurrentFixedUpdate();

        if (myEsquiveState == EsquiveState.GoesUnderSand)
        {
			WeaponSandShader_GoesUnderSand ();

            myAgent.isStopped = true;
            myAnimCroco.transform.position -= Vector3.up * (Time.deltaTime * moveSpeed);
            currentYPos += (Time.deltaTime * moveSpeed);
            ((AI_Enemy_SandTracker)myIndividu).SandShaderPos.HightUpdate(((currentYPos > 0.1f) ? currentYPos : 0.1f) * 1.0f);

            if (currentYPos >= enemySize)
            {
                foreach (HittenBox_Enemy hbe in allHittenBox)
                {
                    hbe.gameObject.SetActive(false);
                }

                myEsquiveState = EsquiveState.InDeplacement;

                Vector3 tempDir = currentTarget.transform.position - myIndividu.transform.position;
                tempDir = Vector3.Normalize(tempDir);
                destination = currentTarget.transform.position + (tempDir * minDistanceToTarget * 1.5f);

                destination = PositionOnNavMesh(destination);

                myAgent.destination = destination;
                
            }
        }
        else if (myEsquiveState == EsquiveState.InDeplacement)
        {
            myAgent.isStopped = false;
            
            if (Vector3.Distance(myIndividu.transform.position, destination) < minDistanceToTarget/* * 0.75f*/)
            {
                myEsquiveState = EsquiveState.GoesUpperSand;
                myIndividu.transform.LookAt(currentTarget);
                myIndividu.transform.rotation = Quaternion.Euler(0.0f, myIndividu.transform.rotation.eulerAngles.y, 0.0f);
            }
        }
        else
        {
			WeaponSandShader_GoesUpperSand ();
            myAgent.isStopped = true;
            myAnimCroco.transform.position += Vector3.up * (Time.deltaTime * moveSpeed);
            currentYPos -= (Time.deltaTime * moveSpeed);
            myIndividu.transform.LookAt(currentTarget);
            myIndividu.transform.rotation = Quaternion.Euler(0.0f, myIndividu.transform.rotation.eulerAngles.y, 0.0f);
            ((AI_Enemy_SandTracker)myIndividu).SandShaderPos.HightUpdate(((currentYPos > 0.1f) ? currentYPos : 0.1f) * 1.0f);
            if (currentYPos <= 0.0f)
            {
                ((AI_Enemy_SandTracker)myIndividu).SandShaderPos.Activate(false);
                myAnimCroco.transform.position = new Vector3(myAnimCroco.transform.position.x, myIndividu.transform.position.y + initialeYdeltaPos, myAnimCroco.transform.position.z);
                myAgent.isStopped = false;

                foreach (HittenBox_Enemy hbe in allHittenBox)
                {
                    hbe.gameObject.SetActive(true);
                }


                (myIndividu as AI_Enemy_SandTracker).ChangeState(AI_Enemy_SandTracker.State.Fighting);
            }
        }
    }

    public override void InitCombat(float _abandonDistance, float _range, float _minDistanceToTarget, bool _dieWhenTouchingTarget)
    {
        base.InitCombat(_abandonDistance, _range, _minDistanceToTarget, _dieWhenTouchingTarget);
        initialeYdeltaPos = myAnimCroco.transform.position.y - myIndividu.transform.position.y;

        allHittenBox = myIndividu.GetComponentsInChildren<HittenBox_Enemy>();        
    }

    public override void OnEnd()
    {
        base.OnEnd();
    }

    public override void PropulseAgent(Vector3 _dir)
    {
        myAnimCroco.transform.position = new Vector3(myAnimCroco.transform.position.x, myIndividu.transform.position.y + initialeYdeltaPos, myAnimCroco.transform.position.z);
		myAnimWeapon.transform.position = new Vector3(myAnimWeapon.transform.position.x, myIndividu.transform.position.y + initialeYdeltaPos, myAnimWeapon.transform.position.z);

        ((AI_Enemy_SandTracker)myIndividu).SandShaderPos.Activate(false);
        foreach (HittenBox_Enemy hbe in allHittenBox)
        {
            hbe.gameObject.SetActive(true);
        }

        base.PropulseAgent(_dir);

        (myIndividu as AI_Enemy_SandTracker).ChangeState(AI_Enemy_SandTracker.State.IsHit);
    }
}
