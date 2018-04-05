/*
* @ArthurLacour
* @StepSandElement.cs
* @28/03/2018
* @ - Le Script s'attache à un prefab de marche pour le SandBridge.
*   
* Contient :
*   - L'activation d'une marche à l'arrivée du joueur.
* 
* A faire:
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepSandElement : MonoBehaviour {
    
    private Transform playerRef;
    private MeshRenderer myMesh;

    private Vector3 realPos;
    private Vector3 realLocalPos;
    private Vector3 realScale;

    [SerializeField] [Range(1.0f, 10.0f)] float minActivateDistance = 5.0f;
    [SerializeField] [Range(1.0f, 10.0f)] float speedApparition = 3.0f;
    [SerializeField] [Range(1.0f, 10.0f)] float originDistanceApparition = 2.2f;

    // Use this for initialization
    void Start () {
        myMesh = GetComponent<MeshRenderer>();
        myMesh.enabled = false;

        playerRef = GameObject.FindObjectOfType<vd_Player.CharacterInitialization>().PlayerTr;

        realPos = transform.position;
        realLocalPos = transform.localPosition;
        realScale = transform.localScale;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (myMesh.enabled == false)
        {
            if (Vector3.Distance(realPos, playerRef.position) < minActivateDistance)
            {
                StartCoroutine(StepApparition());
            }
        }
	}

    private IEnumerator StepApparition()
    {
        float timer = 0.0f;
        Vector3 originLocalPos = realLocalPos - ((Vector3.forward * originDistanceApparition) * ((Vector3.Dot(playerRef.forward, transform.forward) > 0) ? -1 : 1) + (Vector3.up * originDistanceApparition));
        transform.localPosition = originLocalPos;
        myMesh.enabled = true;

        while (timer < 1.0f)
        {
            timer += Time.deltaTime * speedApparition;
            transform.localPosition = Vector3.Lerp(originLocalPos, realLocalPos, timer);
            transform.localScale = Vector3.Lerp(Vector3.zero, realScale, timer);
            yield return null;
        }
        while (Vector3.Distance(realPos, playerRef.position) < minActivateDistance)
        {
            //WaitForDisparition
            yield return null;
        }
        StartCoroutine(StepDisparition());

        yield return null;
    }
    private IEnumerator StepDisparition()
    {
        float timer = 0.0f;
        Vector3 originLocalPos = transform.localPosition;
        Vector3 originScale = transform.localScale;

        while (timer < 1.0f) {
            timer += Time.deltaTime * speedApparition;
            transform.localPosition = Vector3.Lerp(originLocalPos, originLocalPos - (Vector3.up * originDistanceApparition), timer);
            transform.localScale = Vector3.Lerp(originScale, Vector3.zero, timer);
            yield return null;
        }

        myMesh.enabled = false;
        yield return null;
    }
}
