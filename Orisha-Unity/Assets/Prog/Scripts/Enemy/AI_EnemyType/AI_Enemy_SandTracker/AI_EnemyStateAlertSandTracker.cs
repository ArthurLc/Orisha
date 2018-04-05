using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_EnemyStateAlertSandTracker : AI_EnemyStateAlert
{
    public override void OnBegin()
    {
        base.OnBegin();
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _anim, NavMeshAgent _agent, Rigidbody _rb, Vector3 _startPosition)
    {
        base.OnBegin(_individu, _anim, _agent, _rb, _startPosition);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _anim, NavMeshAgent _agent, Rigidbody _rb, List<Transform> _patrolPositions)
    {
        base.OnBegin(_individu, _anim, _agent, _rb, _patrolPositions);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _anim, NavMeshAgent _agent, Rigidbody _rb, Vector3 _startPosition, Transform _myTarget)
    {
        base.OnBegin(_individu, _anim, _agent, _rb, _startPosition, _myTarget);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
    }
    public override void OnBegin(AI_Enemy_Basic _individu, Animator _anim, NavMeshAgent _agent, Rigidbody _rb, List<Transform> _patrolPositions, Transform _myTarget)
    {
        base.OnBegin(_individu, _anim, _agent, _rb, _patrolPositions, _myTarget);
        UpdateState = CurrentUpdate;
        FixedUpdateState = CurrentFixedUpdate;
    }


    protected override void CurrentUpdate()
    {
        base.CurrentUpdate();

        if (myCoroutine == null && myAnim.GetCurrentAnimatorStateInfo(0).IsName("Alert"))
        {
            myCoroutine = myIndividu.StartCoroutine(AlertDuration(myAnim.GetCurrentAnimatorStateInfo(0).length));
        }

        OnEndCurrentUpdate();
    }
    protected override void CurrentFixedUpdate()
    {
        base.CurrentFixedUpdate();
    }

    protected IEnumerator AlertDuration(float _duration)
    {
        float timer = 0.0f;

        while(timer < _duration)
        {
            timer += Time.unscaledDeltaTime;

            yield return 0;
        }

        //Si y a des bugs a cause du stop all coroutines, ajouter un test de vie ici !!
        ((AI_Enemy_SandTracker)myIndividu).ChangeState(AI_Enemy_SandTracker.State.Chasing);
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

