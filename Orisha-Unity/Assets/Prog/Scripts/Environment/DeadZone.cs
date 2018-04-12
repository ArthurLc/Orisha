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

    void Start()
    {
        zoneScript = FindObjectOfType<DisplayZoneName>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            Destroy(other.gameObject);
        }
        else if (other.tag == "Player")
        {
            zoneScript.BeginDisplay("GameOver", fadeInDuration, displayDuration);
            // Le player fait "HAAAAAAAAA" parce qu'il tombe.
            StartCoroutine(WaitToRepop(fadeInDuration + displayDuration));
        }
    }

    private IEnumerator WaitToRepop(float _timer)
    {
        float localTimer = _timer;

        while (localTimer > 0) {
            localTimer -= Time.deltaTime;
            yield return null;
        }

        CheckpointsManager.RepopPlayerToCloserCheckpoint();
        yield return null;
    }
}
