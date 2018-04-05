/*
* @AmbreLacour
* @CameraRabbit.cs
* @18/01/2017
* @Le script s'attache sur un navMeshAgent
*   - L'agent va aller un peu partout au pif, l'objectif pour le joueur est de le suivre
*   - Permet de tester la camera
*   
*/

using UnityEngine;
using UnityEngine.AI;

public class CameraRabbit : MonoBehaviour
{
    private NavMeshAgent agent;
    private float nextDestRadius = 10.0f;
    private Vector3 nextDest;
    private NavMeshHit hit;
    private bool hasNextDest = false;

    private Animator anim;

    short secu = 0;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        Debug.Assert(agent != null, "Rabbit can't move: no nav mesh agent !");
        FindNextDest();
    }

    void Update()
    {
        if (agent.remainingDistance < 1.0f)
        {
            FindNextDest();
        }


        if (anim != null)
            anim.SetFloat("Velocity", agent.speed);
    }


    void FindNextDest()
    {
        while (hasNextDest == false)
        {
            nextDest = transform.position + new Vector3(Random.Range(-nextDestRadius, nextDestRadius), Random.Range(-nextDestRadius * 0.5f, nextDestRadius * 0.5f), Random.Range(-nextDestRadius, nextDestRadius));
            if(hasNextDest = NavMesh.SamplePosition(nextDest, out hit, nextDestRadius*2, NavMesh.AllAreas))
                agent.SetDestination(hit.position);
            secu++;

            if (secu > 1000)
            {
                Debug.Log("Rabbit: new destination not found");
                return;
            }
        }
        hasNextDest = false;
        secu = 0;
        Debug.Log("Rabbit : new destination");
    }
}
