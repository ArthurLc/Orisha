using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* GroundChecker.cs
* Gestion de la détection du sol pour le player
*       
* Crée par Arthur LACOUR le 30/05/2018
*/


public class GroundChecker : MonoBehaviour {

    [SerializeField] Transform tr;
    private bool isGroundDetected = true;
    public bool IsGroundDetected
    {
        get { return isGroundDetected;  }
    }

    private void Start()
    {
        Vector3 begin = new Vector3(tr.position.x, tr.position.y + 0.05f, tr.position.z);
        isGroundDetected = Physics.Raycast(begin, -Vector3.up, 0.5f);
    }

    private void OnTriggerStay(Collider other)
    {
        isGroundDetected = true;
    }
    private void OnTriggerExit(Collider other)
    {
        isGroundDetected = false;
    }
}
