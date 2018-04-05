/*
* @JulienLopez
* @NavMeshBaker.cs
* @12/02/2018
*   
* @ Fonctionnement : Bake les NavMeshSurface de la scène s'ils sont à une certaine distance du joueur
* 
* 
* 
* NB :
*   - Le Script s'attache à un GameObject dans la scene
*   - Le script se désactive s'il ne touve pas le joueur ou des surfaces à bake.
* 
*/



using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using vd_Player;

public class NavMeshBaker : MonoBehaviour
{
    [SerializeField] private float minBakeDistance = 150.0f;
    [SerializeField] private float maxBakeDistance = 300.0f;
    [SerializeField] private bool debugOn = false;

    private NavMeshSurface[] surfaces;
    private Transform playerTr;

    [SerializeField] private float bakeDelay = 1.0f;
    float bakeTimer;


    void Start ()
    {
        if (Initialization() == true)
        {
            StartCoroutine(UpdateNavMesh());
            bakeTimer = bakeDelay;
        }
        else
        {
            Debug.LogWarning("Pas de NavMeshSurface ou de CharacterInitialization dans la scène, le script NavMeshBaker a été désactivé.");
            enabled = false;
        }
	}

    private void FixedUpdate()
    {
        bakeTimer-=Time.deltaTime;

        if (bakeTimer < 0.0f)
        {
            StartCoroutine(UpdateNavMesh());
            bakeTimer = bakeDelay;
        }


    }

    /// <summary>
    /// Initialization : retourne vrai si tout est initialisé, faux si aucune NavMeshSurface ou si aucun joueur trouvé
    /// </summary>
    /// <returns></returns>
    private bool Initialization()
    {
        // Initialisation des NavMeshSurface
        surfaces = FindObjectsOfType<NavMeshSurface>();
        if (surfaces == null)
            return false;

        // Initialisation du joueur (on passe par CharacterInitialization pour récup le transform de son rigidbody)
        CharacterInitialization pl = FindObjectOfType<CharacterInitialization>();
        if (pl == null)
            return false;

        playerTr = pl.Rb.transform;

        return true;
    }
    

    /// <summary>
    /// Coroutine Build NavMesh Update
    /// </summary>
    /// <returns></returns>
    private IEnumerator UpdateNavMesh()
    {
        for (int i = 0; i < surfaces.Length; i++)
        {
            float distToPlayer = Vector3.Distance(playerTr.position, surfaces[i].transform.position);
            if (distToPlayer < maxBakeDistance && distToPlayer > minBakeDistance)
            {
                if (debugOn)
                    Debug.Log("NavMeshBaker: is baking " + surfaces[i].gameObject.name);
                //surfaces[i].BuildNavMesh();       // beaucoup trop de lag à bake en dynamique pour le moment

            }
            else
            {
                if (debugOn)
                    Debug.Log("NavMeshBaker: " + surfaces[i].gameObject.name + " isn't bake");
            }
            
            yield return 0;

        }

    }
}
