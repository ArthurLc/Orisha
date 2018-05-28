using System.Collections;
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

    [SerializeField] ParticleSystem boss_Estoc_Particle; 
    [SerializeField] ParticleSystem boss_Hammer_Particle;

    [SerializeField] Animator[] toSync;

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


    void LaunchParticlesBoss()
    {
        if (boss_Estoc_Particle)
            boss_Estoc_Particle.Play();
    }


    void LaunchHammerParticlesBoss()
    {
        if(boss_Hammer_Particle)
            boss_Hammer_Particle.Play();
    }

    void DisablePlayerCollision()
    {
        enemyBasic.DisablePlayerCollision();
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

	public void FailAttack()
	{
		float chance = Random.Range (0f, 1f);
		if (chance < 0.5f)
			enemyBasic.AttackFail ();
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

    public void LockState()
    {
        enemyBasic.FreezeStates();
    }

    public void UnlockState()
    {
        enemyBasic.UnfreezeStates();
    }


    public void ScreamSound()
    {
        if(enemyBasic.screamsSfx != null)
        {
            int soundId = Random.Range(0, enemyBasic.screamsSfx.Length);
            SoundManager.instance.SFX_PlayAtPosition(enemyBasic.screamsSfx[soundId], enemyBasic.transform.position, 1f, 100f);
        }
    }

    public void DeathSound()
    {
        if (enemyBasic.deathSfx != null)
        {
            int soundId = Random.Range(0, enemyBasic.deathSfx.Length);
            SoundManager.instance.SFX_PlayAtPosition(enemyBasic.deathSfx[soundId], enemyBasic.transform.position, 1f, 100f);
        }
    }

    public void AttackSound()
    {
        if (enemyBasic.attacksSfx != null)
        {
            int soundId = Random.Range(0, enemyBasic.attacksSfx.Length);
            SoundManager.instance.SFX_PlayAtPosition(enemyBasic.attacksSfx[soundId], enemyBasic.transform.position, 1f, 100f);
        }
    }

    public void HitSound()
    {
        if (enemyBasic.hitSfx != null)
        {
            int soundId = Random.Range(0, enemyBasic.hitSfx.Length);
            SoundManager.instance.SFX_PlayAtPosition(enemyBasic.hitSfx[soundId], enemyBasic.transform.position, 1f, 100f);
        }
    }

    public void BossDeath()
    {
        //faire tomber/voler le masque ici (coroutine tt ça)
        //enemyBasic.SandMaterial.SetFloat("_EmissiveIntensity", 0.0f);
        StartCoroutine(Fade(GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterial, 10, 0));
    }


    IEnumerator Fade(Material mat, float lossPercentage, int iteratons)
    {
        for (float f = 1f; f >= 0; f -= (lossPercentage/100))
        {
            mat.SetFloat("_EmissiveIntensity", f);
            yield return new WaitForSeconds(.1f);
        }

        if(iteratons > 0)
            StartCoroutine(Fade(mat, 10, iteratons - 1));
    }

    void SpawnEnemies()
    {
        StrengthInvoc si = GetComponentInParent<StrengthInvoc>();
        if(si)
            si.SpawnEnemies();
        
        Albinos a = GetComponentInParent<Albinos>();
        if (a)
            a.SpawnEnemies();
    }

    void ActivateInvincibility()
    {
        enemyBasic.IsInvincible = true;
    }

    void DesactivateInvincibility()
    {
        enemyBasic.IsInvincible = false;
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
