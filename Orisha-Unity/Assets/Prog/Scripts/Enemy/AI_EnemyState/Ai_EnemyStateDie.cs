using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ai_EnemyStateDie : AI_EnemyState
{
    protected float timerBeforeDisappear = 8.0f;
    protected float distanceToTheGroundDisappear = 1.38f;
    protected float maxDistanceToTheGroundDisappear = 15.0f;
    protected Vector3 finalPos = Vector3.zero;

    public override void OnBegin()
    {
        base.OnBegin();
    }

    protected override void CurrentUpdate()
    {
        base.CurrentUpdate();
        // Enemy is dying here
        // Final instruction of the global state update
        OnEndCurrentUpdate();
    }

    protected void DeathAction()
    {
        myAnimCroco.SetTrigger("Die");
        myAnimWeapon.SetTrigger("Die");
        myAnimArmor.SetTrigger("Die");
        myIndividu.DmgBoxList[0].enabled = false;
        //myAnim.GetComponent<CapsuleCollider>().direction = 0;

        myAgent.updatePosition = false;
        myAgent.updateRotation = false;
        myAgent.updateUpAxis = false;

		timerBeforeDisappear = 5.0f;
        //myAnimCroco.GetComponent<CapsuleCollider>().height = 0.0f;

        GameObject.Destroy(myAgent);
    }

    protected override void CurrentFixedUpdate()
    {
		//Debug.Log (timerBeforeDisappear);
        if (timerBeforeDisappear >= 0.0f)
        {
            timerBeforeDisappear -= Time.deltaTime;
			//Debug.Log (timerBeforeDisappear);
        }
        else if(finalPos == Vector3.zero)
        {
            finalPos = myIndividu.transform.position;
        }
        else if (distanceToTheGroundDisappear < maxDistanceToTheGroundDisappear)
        {
            distanceToTheGroundDisappear += (Time.deltaTime * 0.8f);
            myIndividu.transform.position = new Vector3(finalPos.x, myIndividu.transform.position.y, finalPos.z);
        }
        else
        {
            GameObject.Destroy(myIndividu.gameObject);           
            GameObject.Destroy(myAnimWeapon.gameObject);
        }

        myAnimCroco.GetComponent<CapsuleCollider>().center = new Vector3(0.0f, distanceToTheGroundDisappear, 0.0f);
    }


    public override void OnEnd()
    {
        base.OnEnd();
    }
}
