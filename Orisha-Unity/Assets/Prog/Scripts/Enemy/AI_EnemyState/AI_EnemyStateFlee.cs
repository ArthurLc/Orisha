using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_EnemyStateFlee : AI_EnemyState
{

    // combats skills
    protected Vector3 destination;
    protected float abandonDistance;
    protected float range;
    protected float minDistanceToTarget;
    protected bool dieWhenTouchingTarget;


    public override void OnBegin()
    {
        base.OnBegin();
        //targetPos = ;
    }

    protected override void CurrentUpdate()
    {
        base.CurrentUpdate();
    }

    protected override void CurrentFixedUpdate()
    {
        base.CurrentFixedUpdate();
    }


    public override void OnEnd()
    {
        base.OnEnd();
    }

    public virtual void InitCombat(float _abandonDistance, float _range, float _minDistanceToTarget, bool _dieWhenTouchingTarget)
    {
        abandonDistance = _abandonDistance;
        range = _range;
        minDistanceToTarget = _minDistanceToTarget;
        dieWhenTouchingTarget = _dieWhenTouchingTarget;
    }
}
