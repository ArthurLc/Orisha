
using UnityEngine;
using vd_Player;

/*
* @ArthurLacour
* @PlayerAnimEvents.cs
* @30/10/2017
* @Le script s'attache là où se trouve l'animator du player
*   - ! Référence à/aux box collider(s) d'attaque du player nécessaire. !
*   - Le script contient les fonctions public nécessaire aux events de ses animations
*   
*   - NOUVEAU : ajout des IK pour la tête pendant le focus. NB : pour que la fonction d'IK soit appelée, nécessité d'un rig humanoïd
*   
*   Dernière modif : Ambre 10/12/2017 (IK tête qui marchent pas...)
*/

[RequireComponent(typeof(Animator))]
public class PlayerAnimEvents : MonoBehaviour
{
    public bool isActive = true;

    [SerializeField] private CharacterInitialization ci;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerFight playerFight;

    [Header("Damage boxes")]
    [SerializeField] private DamageBox box_RightArm;
    [SerializeField] private DamageBox box_LeftArm;
    [SerializeField] private DamageBox box_RightFoot;
    [SerializeField] private DamageBox box_LeftFoot;
    [SerializeField] private DamageBox box_Body;

    [Header("Bones")]
    [SerializeField] private PlayerBone bone_RightArm;
    [SerializeField] private PlayerBone bone_LeftArm;

    [Header("Particle systems")]
	[SerializeField] private ParticleSystem ps_DustShockwave;
    [SerializeField] private ParticleSystem ps_DoubleAttacks;

    [Header("sfx")]
    [SerializeField] private AudioClip swoosh;

    //Temporary
    [Header("Particle Tests")]
    [SerializeField] bool enableTest = true;
    [SerializeField] private GameObject particleTest;


    private Animator animator;

    // Sécus
    private void Start()
    {
        if (isActive)
        {
            if (box_RightArm == null || box_LeftArm == null
                || box_RightFoot == null || box_LeftFoot == null)
            {
                Debug.LogError("DamageBox manquante dans le PlayerAnimEvents, il va  avoir des erreurs !");
            }
            animator = GetComponent<Animator>();
        }
    }

    /// <summary>
    /// Fonction qui déclanche la prochaine attaque du joueur si elle doit avoir lieu.
    /// </summary>
    public void ChainAttack()
    {
        if (playerFight != null)
            playerFight.PlayChainAttack();
    }

    public void ResetAttack()
    {
        if (playerFight != null)
            playerFight.ClearListInputs();
    }
    //Temporary
    void OnEnableBoxes(Vector3 _position)
    {
        if (enableTest)
        {
            GameObject go = Instantiate(particleTest);
            ParticleSystem ps = go.GetComponent<ParticleSystem>();
            go.transform.position = _position;
            ps.Play();
        }
    }

    // 0 = Right, 1 = left, 2 = left + right
    public void LaunchRightParticles()
    {
        bone_RightArm.PsCurrent.Play();
    }
    public void LaunchLeftParticles()
    {
        bone_LeftArm.PsCurrent.Play();
    }
    public void LaunchBothParticles()
    {
        bone_RightArm.PsCurrent.Play();
        bone_LeftArm.PsCurrent.Play();
    }


    /// <summary> 
    /// Fonction qui active l'attaque box du bras droit du joueur
    /// </summary>
    public void EnableBox_RightArm(int _damageValue)
    {
        if (isActive)
        {
            box_RightArm.enabled = true;
            box_RightArm.damageValue = _damageValue;
            bone_RightArm.SpawnBone(true);
            SoundManager.instance.SFX_PlayAtPosition(swoosh, box_RightArm.transform.position);
            //OnEnableBoxes (box_RightArm.transform.position);
        }
    }

    /// <summary> 
    /// Fonction qui désactive l'attaque box du bras droit du joueur
    /// </summary>
    public void DisableBox_RightArm()
    {
        if (isActive)
        {
            box_RightArm.enabled = false;
        }
    }

    /// <summary> 
    /// Fonction qui active l'attaque box du bras gauche du joueur
    /// </summary>
    public void EnableBox_LeftArm(int _damageValue)
    {
        if (isActive)
        {
            box_LeftArm.enabled = true;
            box_LeftArm.damageValue = _damageValue;
            bone_LeftArm.SpawnBone(true);
            SoundManager.instance.SFX_PlayAtPosition(swoosh, box_LeftArm.transform.position);

            //OnEnableBoxes (box_LeftArm.transform.position);
        }
    }

    /// <summary> 
    /// Fonction qui désactive l'attaque box du bras gauche du joueur
    /// </summary>
    public void DisableBox_LeftArm()
    {
        if (isActive)
        {
            box_LeftArm.enabled = false;
        }
    }

    /// <summary> 
    /// Fonction qui active l'attaque box du pied droit du joueur
    /// </summary>
    public void EnableBox_RightFoot(int _damageValue)
    {
        if (isActive)
        {
            box_RightFoot.enabled = true;
            box_RightFoot.damageValue = _damageValue;

            //OnEnableBoxes (box_RightFoot.transform.position);
        }
    }

    /// <summary> 
    /// Fonction qui désactive l'attaque box du pied droit du joueur
    /// </summary>
    public void DisableBox_RightFoot()
    {
        if (isActive)
        {
            box_RightFoot.enabled = false;
        }
    }

    /// <summary> 
    /// Fonction qui active l'attaque box du pied gauche du joueur
    /// </summary>
    public void EnableBox_LeftFoot(int _damageValue)
    {
        if (isActive)
        {
            box_LeftFoot.enabled = true;
            box_LeftFoot.damageValue = _damageValue;

            //OnEnableBoxes (box_LeftFoot.transform.position);
        }
    }

    /// <summary> 
    /// Fonction qui désactive l'attaque box du pied gauche du joueur
    /// </summary>
    public void DisableBox_LeftFoot()
    {
        if (isActive)
        {
            box_LeftFoot.enabled = false;
        }
    }


    /// <summary> 
    /// Fonction qui active l'attaque box du pied gauche du joueur
    /// </summary>
    public void EnableBox_Body(int _damageValue)
    {
        if (isActive)
        {
            box_Body.enabled = true;
            box_Body.damageValue = _damageValue;

        }
    }

    /// <summary> 
    /// Fonction qui désactive l'attaque box du pied gauche du joueur
    /// </summary>
    public void DisableBox_Body()
    {
        if (isActive)
        {
            box_Body.enabled = false;
        }
    }

    /// <summary> 
    ///Active toutes les box de dommage du joueur
    /// </summary>
    public void EnableAllBoxes()
    {
        if (isActive)
        {
            box_RightArm.enabled = true;
            box_LeftArm.enabled = true;
            box_RightFoot.enabled = true;
            box_LeftFoot.enabled = true;
            box_Body.enabled = true;

            bone_RightArm.SpawnBone(true);
            bone_LeftArm.SpawnBone(true);

            SoundManager.instance.SFX_PlayAtPosition(swoosh, box_Body.transform.position);
        }
    }

    /// <summary> 
    /// Désactive toutes les box de dommage du joueur
    /// </summary>
    public void DisableAllBoxes()
    {
        if (isActive)
        {
            box_RightArm.enabled = false;
            box_LeftArm.enabled = false;
            box_RightFoot.enabled = false;
            box_LeftFoot.enabled = false;
            box_Body.enabled = false;
        }
    }


    /// <summary>
    /// Active la slow motion pour le player et les ennemies qui vont etre touché
    /// </summary>
    public void SlightFoot_SlowMotion()
    {
        if (isActive)
        {
            Vector3 tempPosCeckFoe = (transform.position + transform.forward * 1.1f);
            LayerMask foesMask = 1 << LayerMask.NameToLayer("HittenBox_Enemy");
            Collider[] possibleTarget = Physics.OverlapSphere(tempPosCeckFoe, 1.0f, foesMask, QueryTriggerInteraction.Collide);

            if (possibleTarget.Length > 0)
            {
                foreach (Collider target in possibleTarget)
                {
                    Animator tempFoeAnimator = null;

                    //////////////////// A CLeaner quand hierarchie des ennemies sera plus définitive!
                    ////////////////////////////////
                    tempFoeAnimator = target.GetComponent<Animator>();
                    if (tempFoeAnimator == null)
                    {
                        tempFoeAnimator = target.GetComponentInParent<Animator>();
                        if (tempFoeAnimator == null)
                        {
                            tempFoeAnimator = target.GetComponentInChildren<Animator>();
                        }
                    }
                    ///////////////////////////////////////////
                    //////////////////////////////////////////                
                }
            }
        }
    }


    /// <summary>
    /// Désactive la collision Joueur/Ennemy
    /// </summary>
    public void DisableEnemyCollisions()
    {
        if (isActive)
        {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
            //Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("DamageBox_Enemy"), true);
            //Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("HittenBox_Enemy"), true);
        }
    }

    /// <summary>
    /// Réactive la collision Joueur/Ennemy
    /// </summary>
    public void EnableEnemyCollisions()
    {
        if (isActive)
        {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
            //Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("DamageBox_Enemy"), false);
            //Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("HittenBox_Enemy"), false);
        }
    }

    /// <summary>
    /// Désactive la collision Joueur/Little_Obstacle
    /// </summary>
    public void DisableLittle_ObstacleCollisions()
    {
        if (isActive)
        {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Little_Obstacle"), true);
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("BreakJar"), false);
        }
    }

    /// <summary>
    /// Réactive la collision Joueur/Little_Obstacle
    /// </summary>
    public void EnableLittle_ObstacleCollisions()
    {
        if (isActive)
        {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Little_Obstacle"), false);
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("BreakJar"), true);

        }
    }

    /// <summary>
    /// Désactive les rotations du personnage
    /// </summary>
    public void DisableRotations()
    {
        if (isActive)
        {
            playerController.CanRotate = false;
        }
    }

    /// <summary>
    /// Réactive les rotations du personnage
    /// </summary>
    public void EnableRotations()
    {
        if (isActive)
        {
            playerController.CanRotate = true;
        }
    }

    /// <summary>
    /// Active les particules de shockwave (sable soulevé l'atterrissage du personnage)
    /// </summary>
    public void DustShockWave()
    {
        if (isActive)
        {
            if (ps_DustShockwave.isPlaying == false)
                ps_DustShockwave.Play();
        }
    }

	/// <summary>
	/// Active les particules de doubleAttacks (sable soulevé l'atterrissage du personnage)
	/// </summary>
	public void DoubleAttacks()
	{
		if (isActive) {
			if (ps_DoubleAttacks.isPlaying == false)
				ps_DoubleAttacks.Play ();
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
