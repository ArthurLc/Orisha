/*
 * Script servent de tag pour comptabiliser les groupes de particles system de sand dans la pool
 * 
 */

using UnityEngine;

public class WalkDustObject : MonoBehaviour
{
    [HideInInspector] public float lifespan = 10.0f;


    private void OnEnable()
    {
        Invoke("DisableMe", lifespan);
    }

    private void DisableMe()
    {
        gameObject.SetActive(false);
    }

}
