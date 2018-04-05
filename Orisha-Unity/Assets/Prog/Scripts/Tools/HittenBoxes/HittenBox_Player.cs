using UnityEngine;

/*
* @AmbreLacour
* @HittenBox_Player.cs
* @27/11/2017
* @Le script s'attache à un trigger pour prendre les dégâts
*   - Il contient la réaction du player quand il est touché
*   - Il tourne en boucle mais n'a pas d'update (il guette les collisions avec des damagebox)
*   - Il hérite de l'interface HittenBox
*/


public class HittenBox_Player : MonoBehaviour, HittenBox
{
    [SerializeField] private vd_Player.CharacterInitialization player;

    public void TakeDamage(int _damage)
    {
        if (player != null)
            player.TakeDamage(_damage);
    }
    private void PropulsePlayer(DamageBox _box, int _damage)
    {
        Vector3 PropulseDir = (_box.EntityTransform.position + Vector3.up) - transform.position;
        PropulseDir.Normalize();
        player.PropulsePlayer(PropulseDir * _damage * 100);
        Debug.DrawRay(transform.position, PropulseDir, Color.red, _damage * 100);
    }

    /// <summary>
    /// Gestion de la collision avec un "ennemi"
    /// Le comportement dépend du tag du collider et du tag de ce qu'il heurte
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        DamageBox box = other.GetComponent<DamageBox>();
        //Debug.Log("je suis tapé par un truc");
        if (box != null)
        {
            // je prends mes dégats
            TakeDamage(box.damageValue);
            // Propulsion
            //PropulsePlayer(box, box.damageValue);
            // je dis à l'autre de prendre les dégats
            box.HitSucceed(other.ClosestPointOnBounds(transform.position));
        }
        else
        {
            Debug.LogWarning("Je tape un truc qui a pas de DamageBox : cépa normal");
        }
    }
}
