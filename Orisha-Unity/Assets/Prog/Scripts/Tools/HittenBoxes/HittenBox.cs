using UnityEngine;
/*
* @AmbreLacour
* @HittenBox.cs
* @27/11/2017
* @Le script s'attache à un trigger pour prendre les dégâts
*   - Il tourne en boucle mais n'a pas d'update (il guette les collisions avec des damagebox)
*/


public interface HittenBox
{
    void TakeDamage(int _damage);
}
