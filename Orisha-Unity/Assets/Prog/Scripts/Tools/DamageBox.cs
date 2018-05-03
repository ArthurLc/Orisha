using UnityEngine;
using vd_Player;

/*
* @AmbreLacour
* @DamageBox.cs
* @14/11/2017
* @Le script s'attache à un trigger de dégâts d'un acteur (joueur, ennemi ou pipoui)
*   - C'est le script lié à l'animator de l'acteur qui active/désactive le script et update ou non la collision et l'application de dégâts.
*   - Il contient les dommages infligés par la collision (qui peuvent être updaté par l'animator pour faire + de dégât selon les anims)
*   - Au moment de la collision, dégâts infligés en fonction des tags du collider et de l'objet heurté
*   - Possibilité de créer une interface "Damageable" avec une fonction virtual TakeDamages() qui permetrait d'éviter le switch
*/

[RequireComponent(typeof(Collider))]
public class DamageBox : MonoBehaviour
{
    [SerializeField]
    Transform entityTransform; //Ref vers "Player_Transform" (Transform de référence pour la direction dans laquelle le player fait face.)
    [SerializeField]
    vd_Player.PlayerFight playerDatas;
    [Space, Space]

    public int damageValue = 10;
    public bool debug = true;
    private Collider coll;
    private MeshRenderer mesh;


    public PlayerFight PlayerDatas
    {
        get
        {
            return playerDatas;
        }
    }

    public Transform EntityTransform
    {
        get
        {
            return entityTransform;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("je tape un truc");
    }
    public void HitSucceed(Vector3 hitPoint)
    {
        //je donne des dommages
        //si PlayerFightDatas
        if (playerDatas)
        {
            //Debug.Log("Un joueur a touché quelqu'un");
            if (playerDatas.PlayerFightDatas.psImpact)
            {
                ParticleSystem ps = Instantiate(playerDatas.PlayerFightDatas.psImpact);
                ps.transform.position = hitPoint;
                ps.transform.forward = (transform.position - hitPoint).normalized;
                ps.Play();
                Destroy(ps.gameObject, ps.main.duration);
            }
            else if (debug)
                Debug.Log("je n'ai pas de particle System");

            if (playerDatas.PlayerFightDatas.soundImpact)
            {
				SoundManager.instance.SFX_PlayAtPosition (playerDatas.PlayerFightDatas.soundImpact, hitPoint);
            }
            // Edit suppression de FMOD, appeler le son de dégats ici
            //FMODUnity.RuntimeManager.PlayOneShot("Event:/Character/PlayerHeavyHit");
        }
        //  je joue mes particules / hit sound
    }

    private void OnEnable()
    {
        //Le OnEnable se fais avant le Start on récupère donc le coll et le mesh ici
        if (coll != null)
            coll.enabled = true;
        else
        {
            coll = GetComponent<Collider>();
            coll.enabled = true;
        }
        if (mesh != null)
            mesh.enabled = debug;
        else
        {
            mesh = GetComponent<MeshRenderer>();
            mesh.enabled = debug;
        }
    }

    private void OnDisable()
    {
        coll.enabled = false;
        mesh.enabled = false;
        damageValue = 10;
    }
}
