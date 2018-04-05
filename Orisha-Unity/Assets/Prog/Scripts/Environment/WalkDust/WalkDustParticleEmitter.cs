/*
* @Romain Seurot
* @WalkDustParticleEmitter.cs
* @13/02/2017
* @Le script s'attache a un acteur qui doit générer des particules de sables en marchant
* @Fonctionnnement actuel :
*   - Dans l'éditeur, lier le pied droit et le pied gauche de l'acteur, préviser le layer du sol sableux
*   - Des particules sont générées lorsque le pied arrive proche d'un mesh sur le layer de sable (RayCast)
*   - Les particules de sable sont récupérées dans la pool de particules WalkDustPool (pas d'instantiation), elles gèrent leurs lifetimes
*/


using UnityEngine;

public class WalkDustParticleEmitter : MonoBehaviour
{
    public GameObject LeftFoot;
    public GameObject RightFoot;

    public LayerMask groundMask;

    [SerializeField] private float particlesVisibleDistance = 25.0f;


    Ray rayLeft = new Ray();
    bool groundLeft = true;

    Ray rayRight = new Ray();
    bool groundRight = true;


    private void Start()
    {
        rayLeft.origin = LeftFoot.transform.position;
        rayLeft.direction = Vector3.down;
        rayRight.origin = RightFoot.transform.position;
        rayRight.direction = Vector3.down;

        if(FindObjectOfType<WalkDustPool>() == null)
        {
            Debug.LogWarning("La scène ne comporte pas de pool de particules de pas dans le sable, les scripts associés sont désactivés");
            enabled = false;
        }
    }


    void FixedUpdate()
    {
        // Si l'entité est trop éloignée de la camera, elle ne génère pas de particules
        if (Vector3.Distance(transform.position, Camera.main.transform.position) > particlesVisibleDistance)
            return;


        rayLeft.origin = LeftFoot.transform.position;
        rayRight.origin = RightFoot.transform.position;


        //Right
        if (Physics.Raycast(rayRight, 0.1f, groundMask))
        {
            if (groundRight == false)
            {
                WalkDustPool.SpawnObject(RightFoot.transform.position, transform.rotation); ;
                groundRight = true;
            }
        }
        else
        {
            groundRight = false;
        }

        //Left
        if (Physics.Raycast(rayLeft, 0.1f, groundMask))
        {
            if (groundLeft == false)
            {
                WalkDustPool.SpawnObject(LeftFoot.transform.position, transform.rotation); ;
                groundLeft = true;
            }
        }
        else
        {
            groundLeft = false;
        }
    }
}
