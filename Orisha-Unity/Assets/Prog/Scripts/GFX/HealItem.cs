/*
* @ArthurLacour
* @HealItem.cs
* @23/05/2017
* @Le script s'attache sur le particle de l'Item de vie
*/

using System.Collections;
using UnityEngine;

public class HealItem : MonoBehaviour {

    [SerializeField] private int healPoints = 10;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            vd_Player.CharacterInitialization ci = other.GetComponentInParent<vd_Player.CharacterInitialization>();

            ci.HealPlayer(healPoints);
            ci.LifeBarHUD.StartCoroutine(ci.LifeBarHUD.ActiveHUD_WithTimer(2.0f));
            Destroy(this.gameObject);
        }
    }
}
