using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_EnemyStateIdle : AI_EnemyState
{
    public override void OnBegin()
    {
        base.OnBegin();

        myAnimCroco.SetFloat("Velocity", 0);
        myAnimWeapon.SetFloat("Velocity", 0);
        myAnimArmor.SetFloat("Velocity", 0);

        myAnimCroco.Play(0, -1, 0);
        myAnimWeapon.Play(0, -1, 0);
        myAnimWeapon.Play(0, -1, 0);
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
}
