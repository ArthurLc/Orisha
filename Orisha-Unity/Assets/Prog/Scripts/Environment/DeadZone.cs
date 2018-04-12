/*
* @ArthurLacour
* @DeadZone.cs
* @19/03/2018
*   
* Contient :
*   - Destroy les AI.
*   - La mort du Player qui tombent.
* 
* A faire:
*/

using System.Collections;
using UnityEngine;

public class DeadZone : MonoBehaviour {
    
    private DisplayZoneName zoneScript;
    [Header("UI")]
    [SerializeField] float fadeInDuration = 1.5f;
    [SerializeField] float displayDuration = 1.5f;
    [Header("Debug")]
    [SerializeField] private bool test = false;

    private static vd_Player.CharacterInitialization player;

    void Start()
    {
        zoneScript = FindObjectOfType<DisplayZoneName>();
        player = FindObjectOfType<vd_Player.CharacterInitialization>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            Destroy(other.gameObject);
        }
        else if (other.tag == "Player")
        {
            if (player.IsPlayerDying == false)
            {
                zoneScript.BeginDisplay("GameOver", fadeInDuration, displayDuration);
                // Le player fait "HAAAAAAAAA" parce qu'il tombe.
                StartCoroutine(player.WaitToRepop(fadeInDuration + displayDuration));
                player.IsPlayerDying = true;
            }
        }
    }
}
