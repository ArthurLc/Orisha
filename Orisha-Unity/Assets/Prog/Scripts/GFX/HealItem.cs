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
    private Collider col;

    private void OnEnable()
    {
        col = GetComponent<Collider>();
        col.enabled = false;

        StartCoroutine(HealTimerApparition(1.0f));
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            vd_Player.CharacterInitialization ci = other.GetComponentInParent<vd_Player.CharacterInitialization>();

            ci.HealPlayer(healPoints);
            ci.LifeBarHUD.StopAllCoroutines();
            ci.LifeBarHUD.SetActiveHUD(true);
            ci.LifeBarHUD.Invoke("DisableHUD", 2.0f);
            SoundManager.instance.SFX_PlayOneShot(heal);
            Destroy(this.gameObject);
        }
    }

    private IEnumerator HealTimerApparition(float _timer)
    {
        float currentTimer = 0.0f;

        while (currentTimer < _timer)
        {
            currentTimer += Time.unscaledDeltaTime;
            yield return null;
        }
        col.enabled = true;
    }
}
