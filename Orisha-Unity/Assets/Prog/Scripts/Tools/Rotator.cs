/*
* @ArthurLacour
* @Rotator.cs
* @12/03/2018
* @ - Le Script s'attache à un objet.
*   
* Détails :
*   - Fait tourner un objet sur place.
*/


using UnityEngine;

public class Rotator : MonoBehaviour {

    [SerializeField] Vector3 axisRotation;

	// Update is called once per frame
	void FixedUpdate () {
        transform.localRotation *= Quaternion.Euler(axisRotation);
    }
}
