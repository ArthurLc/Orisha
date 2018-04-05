/*
* @ArthurLacour
* @DeadZone.cs
* @19/03/2018
*   
* Contient :
*   - La liste static des masques que le joueur possède. (Initialisé au start.)
*   - Les fonctions de gestion des masques.
*   - Emissive du joueur en fonction du masque équipé.
*   - Statistiques du joueur en fonction du masque équipé.
* 
* A faire:
*/

using UnityEngine;

public class DeadZone : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            Destroy(other.gameObject);
        }
        else if (other.tag == "Player")
        {
            //Faire ce qu'il faut ici !
        }
    }
}
