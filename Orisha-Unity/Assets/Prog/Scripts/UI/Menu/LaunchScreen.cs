// LaunchScreen.cs
// Gestion de l'écran de lancement du jeu (titre, crédits, etc)
// Objectifs : 
// - Lancement de l'immersion dans le jeu (musique, transition smooth)
// - Infos : crédits, titre du jeu, démo, la manette
// Crée par Ambre LACOUR le 16/10/2017
// Dernière modification par  le 

// A faire : tout ? Pour le moment, il ne fait qu'apparaitre/disparaitre au lancement selon un timer.



using UnityEngine;

namespace vd_Menu
{

    public class LaunchScreen : MonoBehaviour
    {

        [SerializeField] private float timeBeforeLaunch = 2.0f;



        void Update()
        {
            if (Input.anyKey == true || timeBeforeLaunch <= 0.0f)
                gameObject.SetActive(false);

            timeBeforeLaunch -= Time.deltaTime;
        }
    }

}
