using UnityEngine;

/*
* @ArthurLacour
* @AnimInfo_Enemy.cs
* @29/03/2018
* @Le script s'attache là où se trouve l'animator de l'ennemi
*   - ! Référence à/aux box collider(s) d'attaque de l'ennemi nécessaire. !
*   - Le script contient les fonctions public nécessaire aux events de ses animations
*   
*   Dernière modif : Arthur 29/03/2018 (Structure...)
*/

[RequireComponent(typeof(Animator))]
public class AnimInfo_Enemy : MonoBehaviour
{
    public bool isActive = true;

    [SerializeField] private AI_Enemy_Basic enemyBasic;

    [Header("Damage boxes")]
    [SerializeField] private DamageBox box_Weapon;
    
    private Animator animator;

    // Use this for initialization
    void Start ()
    {
        if (isActive)
        {
            if (box_Weapon == null)
            {
                Debug.LogError("DamageBox manquante dans le PlayerAnimEvents, il va  avoir des erreurs !");
            }

            animator = GetComponent<Animator>();
        }
    }

    /// <summary> 
    /// Fonction qui active l'attaque box de l'arme de l'ennemi
    /// </summary>
    public void EnableBox_Weapon(int _damageValue)
    {
        if (isActive)
        {
            box_Weapon.enabled = true;
            box_Weapon.damageValue = _damageValue;
            /*SoundManager.source.clip = swoosh;
            SoundManager.source.Play();*/
        }
    }

    /// <summary> 
    /// Fonction qui désactive l'attaque box de l'arme de l'ennemi
    /// </summary>
    public void DisableBox_Weapon()
    {
        if (isActive)
        {
            box_Weapon.enabled = false;
        }
    }


    public void PlaySound()
    {
        if (isActive)
        {
            //SoundManager.source.PlayOneShot(swoosh);
        }
    }


    /// <summary> 
    ///Active toutes les box de dommage de l'ennemi
    /// </summary>
    public void EnableAllBoxes()
    {
        if (isActive)
        {
            box_Weapon.enabled = true;

            /*if (!SoundManager.source.isPlaying)
            {
                SoundManager.source.clip = swoosh;
                SoundManager.source.Play();
            }*/
        }
    }

    /// <summary> 
    /// Désactive toutes les box de dommage de l'ennemi
    /// </summary>
    public void DisableAllBoxes()
    {
        if (isActive)
        {
            box_Weapon.enabled = false;
        }
    }


    /// <summary>
    /// Désactive les rotations de l'ennemi
    /// </summary>
    public void DisableRotations()
    {
        if (isActive)
        {
            enemyBasic.FreezePosRot();
        }
    }

    /// <summary>
    /// Réactive les rotations de l'ennemi
    /// </summary>
    public void EnableRotations()
    {
        if (isActive)
        {
            enemyBasic.UnfreezePosRot();
        }
    }

    /// <summary>
    /// Gestion des IK
    /// </summary>
    /// <param name="layerIndex"></param>
    void OnAnimatorIK(int layerIndex)
    {
        if (isActive)
        {
            // IK de la tête influencée par la camera (en mode explo)
            //if(ci.PlayerCam.IsLandmarkNearby())
            //{
            //    animator.SetLookAtPosition(ci.PlayerCam.GetNearLandmarkPosition());
            //    animator.SetLookAtWeight(0.5f);
            //}
            //else
            //    animator.SetLookAtWeight(0.0f);
        }
    }
}
