using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPrediction : MonoBehaviour
{
    public bool isDebugOn = false;

    public bool WillTouchEnemy()
    {
        LayerMask mask = 1 << LayerMask.NameToLayer("HittenBox_Enemy");
        Collider[] collid = Physics.OverlapSphere(transform.position, 0.5f, mask, QueryTriggerInteraction.Collide);

        if (collid.Length > 0)
        {
            foreach (Collider col in collid)
            {
                AI_Enemy_Frontal enFront = col.GetComponent<AI_Enemy_Frontal>();
                if(enFront == null)
                {
                    enFront = col.GetComponentInParent<AI_Enemy_Frontal>();
                }
                if (enFront == null)
                {
                    enFront = col.GetComponentInChildren<AI_Enemy_Frontal>();
                }
                AI_Enemy_Harasser enHarass = col.GetComponent<AI_Enemy_Harasser>();
                if (enHarass == null)
                {
                    enHarass = col.GetComponentInParent<AI_Enemy_Harasser>();
                }
                if (enHarass == null)
                {
                    enHarass = col.GetComponentInChildren<AI_Enemy_Harasser>();
                }
                AI_Enemy_SandTracker enSand = col.GetComponent<AI_Enemy_SandTracker>();
                if (enSand == null)
                {
                    enSand = col.GetComponentInParent<AI_Enemy_SandTracker>();
                }
                if (enFront == null)
                {
                    enSand = col.GetComponentInChildren<AI_Enemy_SandTracker>();
                }

                if (enFront != null && enFront.myState != AI_Enemy_Frontal.State.Die)
                {
                    return true;
                }
                else if (enHarass != null && enHarass.myState != AI_Enemy_Harasser.State.Die)
                {
                    return true;
                }
                else if (enSand != null && enSand.myState != AI_Enemy_SandTracker.State.Die)
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
