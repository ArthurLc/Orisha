/*
* @AmbreLacour
* @ZoneDiscovery.cs
* @05/02/2018
* @ - Le Script s'attache à un trigger
*   
* A l'entrée dans le trigger :
*   - Appel de DisplayZoneName pour afficher le nom de la zone
*   - Possible lancement d'une cinématique avec camera qui présentent la scène
* 
* A faire:
*   - Blocage des inputs du joueur ??
*   - Empêche les ennemis d'attaquer le joueur pendant la cinématique.
*/


using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ZoneDiscovery : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private string zoneName = "Fading Island";
    [SerializeField] float fadeInDuration = 0.5f;
    [SerializeField] float displayDuration = 5.0f;
    [SerializeField] private bool isBeginDisplay = false; //Booléan nécessaire pour la cinématique
    [Header("Cinematic")]
    [SerializeField] UnityEngine.Playables.PlayableDirector playableDirector;
    [Header("Debug")]
    [SerializeField] private bool test = false;

    private Collider col;
    private DisplayZoneName zoneScript;

    void Start()
    {
        col = GetComponent<Collider>();
        isBeginDisplay = false;

        zoneScript = FindObjectOfType<DisplayZoneName>();
    }

    void Update()
    {
        if (test)
        {
            //zoneScript.BeginDisplay(zoneName, fadeInDuration, displayDuration);
            playableDirector.Play();
            test = false;
        }
        if(isBeginDisplay) { //Booléan activer par la Timeline de la cinématique
            zoneScript.BeginDisplay(zoneName, fadeInDuration, displayDuration);
            isBeginDisplay = false;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //zoneScript.BeginDisplay(zoneName, fadeInDuration, displayDuration);
            playableDirector.Play();

            TimeManager.Instance.Block_Player_WithTimer((float)playableDirector.duration);
            TimeManager.Instance.Block_Ennemies_WithTimer((float)playableDirector.duration, true);

            col.enabled = false;
        }
    }

}
