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
    [SerializeField] float fadeInDuration = 10;
    [SerializeField] float displayDuration = 20;
    [SerializeField] private bool isBeginDisplay = false; //Booléan nécessaire pour la cinématique
    [Header("Cinematic")]
    [SerializeField] UnityEngine.Playables.PlayableDirector playableDirector;
    [Header("Debug")]
    [SerializeField] private bool test;

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

            LayerMask l_mask = 1 << LayerMask.NameToLayer("Enemy");
            Collider[] results = Physics.OverlapSphere(transform.position, 75.0f, l_mask, QueryTriggerInteraction.Collide);

            if(results.Length > 0)
            {
                for (int i = 0; i < results.Length; i++)
                {
                    bool enemyIsAlive = false;

                    AI_Enemy_Frontal enFront = results[i].gameObject.GetComponent<AI_Enemy_Frontal>();
                    AI_Enemy_Harasser enHarass = results[i].gameObject.GetComponent<AI_Enemy_Harasser>();
                    AI_Enemy_SandTracker enSand = results[i].gameObject.GetComponent<AI_Enemy_SandTracker>();

                    if (enFront != null && enFront.myState != AI_Enemy_Frontal.State.Die)
                    {
                        enemyIsAlive = true;
                    }
                    else if (enHarass != null && enHarass.myState != AI_Enemy_Harasser.State.Die)
                    {
                        enemyIsAlive = true;
                    }
                    else if (enSand != null && enSand.myState != AI_Enemy_SandTracker.State.Die)
                    {
                        enemyIsAlive = true;
                    }


                    if (enemyIsAlive)
                        TimeManager.Instance.Block_Character_WithTimer(results[i].gameObject, (float)playableDirector.duration, true);                    
                }
            }

            col.enabled = false;
        }
    }

}
