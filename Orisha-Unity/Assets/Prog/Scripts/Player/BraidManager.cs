
/*
* @ArthurLacour
* @BraidManager.cs
* @28/05/2018
* @Le script s'attache sur l'objet rig avec l'animation qui a en enfant tous les noeuds de la tresse
*   - Penser à bien links les noeuds
* 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BraidManager : MonoBehaviour {

    [SerializeField] private List<Rigidbody> braids;
    private Vector3 braidOriginalPos;
    private List<Vector3> braidsPreviousRot;
    private List<Vector3> braidsOriginPos;
    private List<Vector3> braidsOriginRot;
    [SerializeField][Range(1.0f,360.0f)] private float permissivity = 10.0f;

    // Use this for initialization
    void Start () {
        braidOriginalPos = transform.localPosition;

        braidsPreviousRot = new List<Vector3>();
        braidsOriginPos = new List<Vector3>();
        braidsOriginRot = new List<Vector3>();
        for (int i = 0; i < braids.Count; i++) {
            braidsOriginPos.Add(braids[i].transform.localPosition);
            braidsOriginRot.Add(braids[i].transform.localEulerAngles);
            braidsPreviousRot.Add(braids[i].transform.localEulerAngles);
        }

        Debug.Log(braidsOriginPos[0].x);
	}
	
	// Update is called once per frame
	void Update ()
    {
        for (int i = 0; i < braids.Count; i++)
        {
            if ((Mathf.Abs((braids[i].transform.localEulerAngles.y - 180.0f)) < Mathf.Abs((braidsPreviousRot[i].y - 180.0f)) - permissivity))
            {
                //Debug.Log("ReplaceBraids");
                //Debug.Log("Current: " + Mathf.Abs((braids[i].transform.localEulerAngles.y - 180.0f)));
                //Debug.Log("Previous: " + (Mathf.Abs((braidsPreviousRot[i].y - 180.0f)) - permissivity));
                for (int y = 0; y < braids.Count; y++)
                {
                    braids[y].velocity = Vector3.zero;
                    braids[y].angularVelocity = Vector3.zero;
                    braids[y].transform.localPosition = braidsOriginPos[y];
                    braids[y].transform.localEulerAngles = braidsOriginRot[y];
                }
            }
            
            braidsPreviousRot[i] = braids[i].transform.localEulerAngles;
        }

        transform.localPosition = braidOriginalPos;
    }
}
