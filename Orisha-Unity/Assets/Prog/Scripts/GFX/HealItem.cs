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
    [SerializeField] AudioClip heal;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            vd_Player.CharacterInitialization ci = other.GetComponentInParent<vd_Player.CharacterInitialization>();

            ci.HealPlayer(healPoints);
            ci.LifeBarHUD.StopAllCoroutines();
            ci.LifeBarHUD.SetActiveHUD(true);
            ci.LifeBarHUD.Invoke("DisableHUD", 2.0f);
            Destroy(this.gameObject);
            SoundManager.instance.SFX_PlayOneShot(heal);
        }
    }
}
