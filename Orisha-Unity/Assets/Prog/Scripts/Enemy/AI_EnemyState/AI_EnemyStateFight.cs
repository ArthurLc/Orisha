using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_EnemyStateFight : AI_EnemyState
{
    // combats skills
    protected float abandonDistance;
    protected float range;
    protected float minDistanceToTarget;
    protected bool dieWhenTouchingTarget;
    protected float attackTimer;


    public override void OnBegin()
    {
        base.OnBegin();
        attackTimer = 0.0f;
    }

    protected override void CurrentUpdate()
    {
        base.CurrentUpdate();
    }

    protected override void CurrentFixedUpdate()
    {
        base.CurrentFixedUpdate();

        // Management du timer pour les attaques
        attackTimer = attackTimer > 0.0f ? attackTimer - Time.deltaTime : 0.0f;
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

