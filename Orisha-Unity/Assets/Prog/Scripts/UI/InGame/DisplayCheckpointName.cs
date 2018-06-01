/*
* @ArthurLacour
* @DisplayCheckpointName.cs
* @12/04/2018
* @ - Le Script s'attache à un checkpoint.
*   
* A l'entrée dans le trigger :
*   - Appel de DisplayZoneName pour afficher le nom du checkpoint
* 
* A faire:
*   - Possible lancement d'une cinématique avec camera qui présentent la scène
*/


using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Checkpoint))]
public class DisplayCheckpointName : MonoBehaviour
{
    private Checkpoint checkpoint;
    [Header("UI")]
    [SerializeField] float fadeInDuration = 10;
    [SerializeField] float displayDuration = 20;
    [Header("Debug")]
    [SerializeField] private bool test = false;

    [SerializeField] private Collider[] col;
    private DisplayZoneName zoneScript;

    void Start()
    {
        checkpoint = GetComponent<Checkpoint>();

        zoneScript = FindObjectOfType<DisplayZoneName>();
    }

    void Update()
    {
        if (test)
        {
            zoneScript.BeginDisplay(checkpoint.GetCheckpointName, fadeInDuration, displayDuration);
            test = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            zoneScript.BeginDisplay(checkpoint.GetCheckpointName, fadeInDuration, displayDuration);
            CheckpointsManager.checkpointList.Add(checkpoint);
            
            for(int i = 0; i < col.Length; i++)
                col[i].enabled = false;
        }
    }

}
