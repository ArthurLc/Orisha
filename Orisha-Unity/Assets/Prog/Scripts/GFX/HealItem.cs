/*
* @ArthurLacour
* @HealItem.cs
* @23/05/2017
* @Le script s'attache sur le particle de l'Item de vie
*/

using UnityEngine;

public class HealItem : MonoBehaviour {

    [SerializeField] private int healPoints = 10;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            other.GetComponentInParent<vd_Player.CharacterInitialization>().HealPlayer(healPoints);
            Destroy(this.gameObject);
        }
    }
}
