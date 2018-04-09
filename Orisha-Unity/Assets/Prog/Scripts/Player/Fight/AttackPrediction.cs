using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPrediction : MonoBehaviour
{
    public bool isDebugOn = false;

	public bool WillTouchEnemy(float damages)
    {
        LayerMask mask = 1 << LayerMask.NameToLayer("HittenBox_Enemy");
        Collider[] collid = Physics.OverlapSphere(transform.position, 0.5f, mask, QueryTriggerInteraction.Collide);

        if (collid.Length > 0)
        {
            foreach (Collider col in collid)
            {
				AI_Enemy_Basic enBase = col.GetComponent<AI_Enemy_Basic>();
				if(enBase == null)
                {
					enBase = col.GetComponentInParent<AI_Enemy_Basic>();
                }
				if (enBase == null)
                {
					enBase = col.GetComponentInChildren<AI_Enemy_Basic>();
                }
				if (enBase != null && enBase.myState != AI_Enemy_Basic.State.Die && enBase.Health <= damages) 
				{
					return true;
				} 
            }
        }
        return false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if(isDebugOn)
            Gizmos.DrawSphere(transform.position, 0.1f);
    }

}
