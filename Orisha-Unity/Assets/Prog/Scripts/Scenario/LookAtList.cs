using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

/* 
* @ArthuLacour
* @LookAtList.cs
* @12/02/2018
* @Le script s'attache à une MC_Cam.
*   - Permet d'avoir un lien de targets à LookAt qui ne sautera pas.
*/
[RequireComponent(typeof(Cinemachine.CinemachineVirtualCamera))]
public class LookAtList : MonoBehaviour
{
    [System.Serializable]
    struct TargetLists
    {
        public Transform target;
        public float transitionSpeed;
    }

    Cinemachine.CinemachineVirtualCamera virtualCam;
    [SerializeField] Transform theTarget;
    [SerializeField] TargetLists[] targetList;
    [SerializeField] bool PingPongTarget; // Enable/Disable it to change the target. 
    int idTarget;
    bool localPingPong;

    private void Start()
    {
        virtualCam = GetComponent<Cinemachine.CinemachineVirtualCamera>();
        virtualCam.m_LookAt = theTarget;
        PingPongTarget = false;
        localPingPong = PingPongTarget;
        idTarget = -1;
    }

    private void Update()
    {
        if (localPingPong != PingPongTarget)
        {
            ++idTarget;
            StartCoroutine(moveTarget(targetList[idTarget].target.position, targetList[idTarget].transitionSpeed));
            localPingPong = PingPongTarget;
        }
    }
    
    IEnumerator moveTarget(Vector3 _newTargetPosition, float _time)
    {
        float elapsedTime = 0;
        Vector3 previousTargetPos = theTarget.position;
        
        while (elapsedTime < _time)
        {
            theTarget.position = Vector3.Lerp(previousTargetPos, _newTargetPosition, (elapsedTime / _time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}