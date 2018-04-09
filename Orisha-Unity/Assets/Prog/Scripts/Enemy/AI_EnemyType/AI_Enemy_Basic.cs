using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class AI_Enemy_Basic : MonoBehaviour
{
	public enum State
	{
		Idle,
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

    protected bool isFreeze;

    public Material SandMaterial;

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
    protected float Basehealth = 50;
    [SerializeField] protected float speed = 2;
    [SerializeField] protected float attackDamages = 10;
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
    [SerializeField] protected float minDistanceToTarget = 1.0f;
    [SerializeField] protected bool dieWhenTouchingTarget = false;

    [Header("Debug")]
    [SerializeField]
    protected bool debugLog = false;

	[Header("AI")]
	[SerializeField]
	protected State state;
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

    private void Awake()
    {
        isFreeze = false;
        Material mat = new Material(SandMaterial);
        Renderer tempRend = GetComponentInChildren<SandShaderPositionner>().gameObject.GetComponent<Renderer>();
        Material[] RememberMat = tempRend.materials;

        for (int i = 0; i < RememberMat.Length; i++)
        {
            RememberMat[i] = mat;
        }

        if (tempRend.material != null)
        {
            Destroy(tempRend.material);
        }
        tempRend.materials = RememberMat;

        GetComponentInChildren<SandShaderPositionner>().myMat = mat;
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
    
    public void FreezePosRot()
    {
        isFreeze = true;

        if (GetComponent<NavMeshAgent>() != null)
        {
            GetComponent<NavMeshAgent>().destination = transform.position;
            GetComponent<NavMeshAgent>().updatePosition = false;
            GetComponent<NavMeshAgent>().updateRotation = false;
        }
    }
    public void UnfreezePosRot()
    {
        isFreeze = false;

        if (GetComponent<NavMeshAgent>() != null)
        {
            GetComponent<NavMeshAgent>().updatePosition = true;
            GetComponent<NavMeshAgent>().updateRotation = true;
        }
    }
}
