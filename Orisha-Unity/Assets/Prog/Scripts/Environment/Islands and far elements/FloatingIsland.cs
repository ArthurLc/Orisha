/* FloatingIsland
 * Julien Lopez
 * 
 * Script attaché au transform qui contiendra en enfant une île et tout ce qu'elle contient pour la faire bouger légèrement
 * 
 * 
 * NB :
 *      - Il faut linker les entrées de ponts sur l'île pour qu'elle bougent avec l'île
 *      - La vitesse de déplacement est paramétrable
 *      - L'île arrête tout mouvement lorsque le player entre dans son trigger
 * 
 */
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Collider))]
public class FloatingIsland : MonoBehaviour
{

    float StartValue = 0.0f;

    [SerializeField] private Transform[] relatedBridges;
    [SerializeField] private float speed = 0.5f;


    private bool isPlayerHere = false;

    private void Start()
    {        
        StartValue = Random.Range(0.0f, 360.0f);
    }

    void Update()
    {
        // L'île est immobile si le player est dans son trigger
        if (isPlayerHere)
            return;


        // Sinon elle bouge doucement
        transform.position = new Vector3(transform.position.x,
            transform.position.y + Mathf.Sin(Time.time + StartValue) * Time.deltaTime * speed,
            transform.position.z);

        // Et fait bouger chacun de ses ponts
        foreach (Transform tr in relatedBridges)
        {
            tr.position = new Vector3(tr.position.x,
                tr.position.y + Mathf.Sin(Time.time + StartValue) * Time.deltaTime * speed,
                tr.position.z);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            isPlayerHere = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            isPlayerHere = false;
    }
}
