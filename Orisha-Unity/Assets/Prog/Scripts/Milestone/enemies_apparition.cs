/* enemies_apparition.cs
 * Fonction milestone : fait apparaitre des ennemis quand le trigger est enclenché
 * 
 * Crée par Ambre LACOUR le 18/12/2017
 */


using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Collider))]
public class enemies_apparition : MonoBehaviour
{
    Collider trigger;
    [SerializeField] private AI_Enemy_Basic[] enemiesToSpawn;
    [SerializeField] private SandShaderPositionner[] enemiesShaders;
    [SerializeField] private float spawnSpeed = 2.0f;
    private bool areSpawning = false;

    private float timer;
    private Vector3[] initPos;
    private Vector3[] finalPos;

	void Start ()
    {
        enemiesShaders = new SandShaderPositionner[enemiesToSpawn.Length];

        for (int i = 0; i < enemiesShaders.Length; i++)
        {
            enemiesShaders[i] = enemiesToSpawn[i].GetComponentInChildren<SandShaderPositionner>();
            enemiesShaders[i].OnBegin();
        }

        trigger = GetComponent<Collider>();
        trigger.isTrigger = true;

        initPos = new Vector3[enemiesToSpawn.Length];
        finalPos = new Vector3[enemiesToSpawn.Length];


        for (int i = 0; i < enemiesToSpawn.Length; i++)
        {
            enemiesToSpawn[i].gameObject.SetActive(false);

            initPos[i] = enemiesToSpawn[i].transform.position + Vector3.down * 2.0f;         
            finalPos[i] = enemiesToSpawn[i].transform.position;

            enemiesToSpawn[i].transform.position = initPos[i];
        }
	}


    void Update ()
    {
		if(areSpawning)
        {
            timer += Time.deltaTime * spawnSpeed;
            for(int i = 0; i < enemiesToSpawn.Length; i++)
            {
                enemiesToSpawn[i].transform.position = Vector3.Lerp(initPos[i], finalPos[i], timer);
            }

            if(timer > 1.0f)
            {
                EndSpawn();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (areSpawning == false && other.tag == "Player")
        {
            areSpawning = true;
            timer = 0.0f;
            for (int i = 0; i < enemiesToSpawn.Length; i++)
            {
                enemiesToSpawn[i].gameObject.SetActive(true);
                enemiesToSpawn[i].enabled = false;
                enemiesToSpawn[i].GetComponent<NavMeshAgent>().enabled = false;
            }
        }
    }


    private void EndSpawn()
    {
        for(int i = 0; i < enemiesToSpawn.Length; i++)
        {
            //enemiesShaders[i].SpawnFinished();
            enemiesToSpawn[i].enabled = true;
            enemiesToSpawn[i].GetComponent<NavMeshAgent>().enabled = true;
        }
        Destroy(gameObject);
    }
}
