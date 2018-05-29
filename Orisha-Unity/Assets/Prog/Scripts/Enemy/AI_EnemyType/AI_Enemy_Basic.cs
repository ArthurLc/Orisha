using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class AI_Enemy_Basic : MonoBehaviour
{
    public enum State
    {
        Idle,
        Taunt,
        Patroling,
        Chasing,
        Alert,
        Fighting,
        Esquive,
        ReplacingToFight,
        ChasingOnFlank,
        Fleeing,
        IsHit,
        Die,
    }
    protected AI_EnemyState myCurrentState;
    public AI_EnemyState MyCurrentState
    {
        get
        {
            return myCurrentState;
        }
    }

    protected Transform currentTarget;
    protected Rigidbody rb;

    public Transform CurrentTarget { get { return currentTarget; } }

    protected bool isFreeze;
    protected bool isFreezeByScript;


    public Material SandMaterial;
    protected Material CrocoMaterial;

    [SerializeField] protected bool debugEnabled = true;
    public bool agentIsControlledByOther;

    // Listes de box qui peuvent attaquer le personnage
    protected List<DamageBox> dmgBoxList = new List<DamageBox>();
    public List<DamageBox> DmgBoxList
    {
        get { return dmgBoxList; }
        set { dmgBoxList = value; }
    }

    protected List<Transform> targetsInReach = new List<Transform>(); // entités à portée à taper
    public List<Transform> TargetsInReach
    {
        get
        {
            return targetsInReach;
        }
    }

    [Header("Stats")]
    [SerializeField]
    protected float baseHealth = 50;
    public float BaseHealth { get { return baseHealth; } }
    [SerializeField] protected float sprintSpeed = 4;
    [SerializeField] protected float walkSpeed = 2;
    [SerializeField] protected float attackDamages = 10;
    [SerializeField] protected float attackSpeed = 1; // Minimal time between every attacks (In seconds)

    [Header("Sounds")]
    [SerializeField] public AudioClip[] screamsSfx;
    [SerializeField] public AudioClip[] attacksSfx;
    [SerializeField] public AudioClip[] deathSfx;
    [SerializeField] public AudioClip[] hitSfx;
    public float AttackSpeed
    {
        get { return attackSpeed; }
    }
    [SerializeField] protected float health;
    public float Health
    {
        get { return health; }
        set { health = value; }
    }

    [Header("Combat")]
    [SerializeField]
    protected float abandonDistance = 10.0f;
    [SerializeField] protected float range = 2.0f;
    [SerializeField] protected bool dieWhenTouchingTarget = false;
    [SerializeField] ParticleSystem shield;
    //Alert
    protected bool asCallReinforcement = false;
    public bool AsCallReinforcement
    {
        get { return asCallReinforcement; }
        set { asCallReinforcement = value; }
    }

    [Header("Debug")]
    [SerializeField] protected bool debugLog = false;

    [Header("AI")]
    [SerializeField] protected State state;
    [SerializeField] public bool IsBoss { get { return isBoss; } }

    [Header("Links")]
    [SerializeField] protected Animator crocoAnim;
    [SerializeField] protected Animator weaponAnim;
    [SerializeField] protected Animator armorAnim;

    public State myState
    {
        get
        {
            return state;
        }

        set
        {
            state = value;
        }
    }

    protected NavMeshAgent myAgent;
    public NavMeshAgent MyAgent { get { return myAgent; } }

    protected bool isInvincible = false;
    [SerializeField] protected bool isBoss;

    public DamageBox mydb;
    public bool IsInvincible
    {
        get { return isInvincible; }
        set
        {
            isInvincible = value;
            if (value == true)
            {
                shield.gameObject.SetActive(true);
                shield.Play();
            }
            else
            {
                shield.Stop();
                shield.gameObject.SetActive(false);
            }

        }
    }

    private void Awake()
    {
        isInvincible = false;
        isFreeze = false;
        asCallReinforcement = false;
        CrocoMaterial = new Material(SandMaterial);
        Renderer tempRend = GetComponentInChildren<SandShaderPositionner>().gameObject.GetComponent<Renderer>();
        Material[] RememberMat = tempRend.materials;

        mydb = weaponAnim.GetComponentsInChildren<DamageBox>()[0];

        for (int i = 0; i < RememberMat.Length; i++)
        {
            RememberMat[i] = CrocoMaterial;
        }

        if (tempRend.material != null)
        {
            Destroy(tempRend.material);
        }
        tempRend.materials = RememberMat;

        GetComponentInChildren<SandShaderPositionner>().myMat = CrocoMaterial;
        myAgent = GetComponent<NavMeshAgent>();
    }

    protected void OnBegin()
    {
        GetComponentInChildren<SandShaderPositionner>().Activate(false);
    }

    public void SetCurrentTarget(Transform _newTarget)
    {
        currentTarget = _newTarget;
    }

    virtual public void TakeDamage(int damages)
    {
        health -= damages;
    }
    
    public void FreezeStates()
    {
        isFreeze = true;
    }

    public void UnfreezeStates()
    {
        isFreeze = false;
    }

    public void HardFreezeStates()
    {
        isFreezeByScript = true;
    }

    public void HardUnfreezeStates()
    {
        isFreezeByScript = false;
    }


    public void FreezePosRot()
    {
		if (myAgent != null && myAgent.isOnNavMesh) 
		{
			myAgent.updatePosition = false;
			myAgent.updateRotation = false;
			myAgent.isStopped = true;
		}
    }

    public void UnfreezePosRot()
    {
        if (myAgent != null && myAgent.isOnNavMesh)
        {
            myAgent.updatePosition = true;
            myAgent.updateRotation = true;
            myAgent.isStopped = false;
        }
	}

	public float FreezeAnim()
	{
		float toReturn = crocoAnim.speed;
		crocoAnim.speed = 0f;
		weaponAnim.speed = 0f;
		armorAnim.speed = 0f;
        return toReturn;
	}

	public void UnfreezeAnim(float _speed)
	{
		crocoAnim.speed = _speed;
		weaponAnim.speed = _speed;
        armorAnim.speed = _speed;
    }

	public virtual void AttackFail()
	{

	}

    public void DisablePlayerCollision()
    {
        //pipoui layer;
        gameObject.layer = 17;
        crocoAnim.gameObject.layer = 17;
    }

    public virtual void Reset()
    {
        health = baseHealth;
        currentTarget = null;
    }

    public void CallEnemies()
    {
        crocoAnim.SetTrigger("CallForAlly");
        weaponAnim.SetTrigger("CallForAlly");
        armorAnim.SetTrigger("CallForAlly");

        crocoAnim.SetFloat("Velocity", 0);
        weaponAnim.SetFloat("Velocity", 0);
        armorAnim.SetFloat("Velocity", 0);
    }

    public void GoTo(Vector3 dest)
    {
        if(myAgent)
            myAgent.SetDestination(dest);

        crocoAnim.SetFloat("Velocity", 5);
        weaponAnim.SetFloat("Velocity", 5);
        armorAnim.SetFloat("Velocity", 5);
    }

    public void Heal(float amount)
    {
        if(health > 0)
            health = Mathf.Clamp(health + amount, 0, BaseHealth);
    }
}
