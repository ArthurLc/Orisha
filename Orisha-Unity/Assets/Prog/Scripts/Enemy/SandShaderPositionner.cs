using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandShaderPositionner : MonoBehaviour
{
    public Material myMat;
  

    public void OnBegin()
    {
        
    }

    public void HightUpdate (float _value)
    {
        if (myMat != null)
        {
            myMat.SetFloat("_MeltY", SampleHeight());

            myMat.SetFloat("_MeltDistance", ((_value * 4.0f) > 2.0f) ? 2.0f : (_value * 4.0f));
        }
    }
    public void Activate(bool _value)
    {
        if (myMat != null)
        {
            if (_value)
            {
                myMat.SetFloat("_MeltY", SampleHeight() - 0.2f);
            }
            else
            {
                myMat.SetFloat("_MeltY", transform.position.y - 100.0f);
            }
        }
    }
    private float SampleHeight()
    {
        RaycastHit hitInfo;
        LayerMask onlyGround = 1 << LayerMask.NameToLayer("Ground");
        bool findPoint = Physics.Raycast(transform.position + Vector3.up * 10.0f, Vector3.down, out hitInfo, 15.0f, onlyGround, QueryTriggerInteraction.Ignore);

        if(findPoint)
        {
            if(Vector3.Angle(Vector3.up, hitInfo.normal) > 20.0f)
            {
                return transform.position.y-100.0f;
            }
            return hitInfo.point.y;
        }

        return transform.position.y - 100.0f;
    }
}
