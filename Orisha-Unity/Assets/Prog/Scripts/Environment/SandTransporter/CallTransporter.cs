/*
* @ArthurLacour
* @Scenar_Skip.cs
* @23/05/2018
* @ - Le Script s'attache à une cinematic
*   
*   Y ajoute un bouton de skip
*/

using UnityEngine;

public class CallTransporter : MonoBehaviour {

    [SerializeField] private SandTransporter sandTransporter;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //Call the platform
            Debug.Log("Viens à moi !");
            sandTransporter.CallTransporter(other);
        }
    }
}
