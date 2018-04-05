using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
* @Romain Seurot
* @AI_EnemyState.cs
* @17/01/2018
* @ -Class parente de chaque state de l'ia enemy
*
*/


public delegate void ptr_Update();
public delegate void ptr_FixedUpdate();

/// <summary>
/// Class parente de chaque state de l'AI enemy
/// </summary>
public class AI_EnemyState
{
    private ptr_Update updateState;
    private ptr_Update fixedUpdateState;
    public ptr_Update UpdateState
    {
        get
        {
            return updateState;
        }

        set
        {
            updateState = value;
        }
    }
    public ptr_Update FixedUpdateState
    {
        get
        {
            return fixedUpdateState;
        }

        set
        {
            fixedUpdateState = value;
        }
    }

    protected AI_Enemy_Basic myIndividu;
    protected Animator myAnim;
    protected NavMeshAgent myAgent;
    protected Rigidbody myRb;

    // Cible(s) du monstre
    protected Transform currentTarget = null;
    protected float targetDistance;
    // Navigation
    protected Ray ray = new Ray();
    // Lieu d'idle
    protected Vector3 startTransform;
    //Lieux de patrouille
    protected List<Transform> patrolPositions;

    protected int secureCount = 0; //Compteur pour ne pas tester les premières frames.


    /// <summary>
    /// Methode appelé une seule fois au début de la state (à l'appel de changement de state)
    /// </summary>
    public virtual void OnBegin()
    {
        
    }
    public virtual void OnBegin(AI_Enemy_Basic _individu, Animator _anim, NavMeshAgent _agent, Rigidbody _rb, Vector3 _startPosition)
    {
        myIndividu = _individu;
        myAnim = _anim;
        myAgent = _agent;
        myRb = _rb;
        startTransform = _startPosition;
    }
    public virtual void OnBegin(AI_Enemy_Basic _individu, Animator _anim, NavMeshAgent _agent, Rigidbody _rb, List<Transform> _patrolPositions)
    {
        myIndividu = _individu;
        myAnim = _anim;
        myAgent = _agent;
        myRb = _rb;
        patrolPositions = _patrolPositions;
    }
    public virtual void OnBegin(AI_Enemy_Basic _individu, Animator _anim, NavMeshAgent _agent, Rigidbody _rb, Vector3 _startPosition, Transform _myTarget)
    {
        myIndividu = _individu;
        myAnim = _anim;
        myAgent = _agent;
        myRb = _rb;
        startTransform = _startPosition;
        currentTarget = _myTarget;
    }
    public virtual void OnBegin(AI_Enemy_Basic _individu, Animator _anim, NavMeshAgent _agent, Rigidbody _rb, List<Transform> _patrolPositions, Transform _myTarget)
    {
        myIndividu = _individu;
        myAnim = _anim;
        myAgent = _agent;
        myRb = _rb;
        patrolPositions = _patrolPositions;
        currentTarget = _myTarget;
    }

    /// <summary>
    /// Update de la state
    /// </summary>
    protected virtual void CurrentUpdate()
    {
        // Update de distance à la cible
        if (currentTarget != null)
        {
            targetDistance = Vector3.Distance(myIndividu.transform.position, currentTarget.position);
            myIndividu.transform.LookAt(currentTarget);
            myIndividu.transform.rotation = Quaternion.Euler(0.0f, myIndividu.transform.rotation.eulerAngles.y, 0.0f);
        }
    }
    /// <summary>
    /// Update de la state (se joue à la fin de l'update)
    /// </summary>
    protected virtual void OnEndCurrentUpdate()
    {
        // Update des animations

        if(myAgent != null)
            myAnim.SetFloat("Velocity", myAgent.velocity.magnitude);
    }

    protected virtual void CurrentFixedUpdate()
    {

    }
    protected virtual void OnEndCurrentFixedUpdate()
    {

    }

    /// <summary>
    /// Methode appelé une seule fois à la fin de la state (à l'appel de changement de state)
    /// </summary>
    public virtual void OnEnd()
    {
        if(myAgent)
            myAgent.isStopped = false;
    }
    
    //public virtual void Init(AI_Enemy_Basic _individu, Animator _anim, NavMeshAgent _agent, Rigidbody _rb, Vector3 _startPosition)
    //{
    //    myIndividu = _individu;
    //    myAnim = _anim;
    //    myAgent = _agent;
    //    myRb = _rb;
    //    startTransform = _startPosition;
    //}
    //public virtual void Init(AI_Enemy_Basic _individu, Animator _anim, NavMeshAgent _agent, Rigidbody _rb, List<Transform> _patrolPositions)
    //{
    //    myIndividu = _individu;
    //    myAnim = _anim;
    //    myAgent = _agent;
    //    myRb = _rb;
    //    patrolPositions = _patrolPositions;
    //}

    //public void InitTarget(Transform _myTarget)
    //{
    //    currentTarget = _myTarget;
    //}
    public virtual void PropulseAgent(Vector3 _dir)
    {
        myRb.isKinematic = false;
        myIndividu.agentIsControlledByOther = true;
        myRb.AddForce(_dir, ForceMode.Impulse);
    }

    /// <summary>
    /// Choix de la cible du mob (par défaut, prend le PJ le + proche à sa portée, si 0 PJ, pipoui le + proche à sa portée)
    /// </summary>
    protected virtual int SearchTarget()
    {
        for (int i = 0; i < myIndividu.TargetsInReach.Count; i++)
        {
            if (myIndividu.TargetsInReach[i].tag == "Player")
            {
                currentTarget = myIndividu.TargetsInReach[i];
                myIndividu.SetCurrentTarget(currentTarget);
                return 0;
            }
        }
        for (int i = 0; i < myIndividu.TargetsInReach.Count; i++)
        {
            if (myIndividu.TargetsInReach[i].tag == "Pipoui")
            {
                currentTarget = myIndividu.TargetsInReach[i];
                myIndividu.SetCurrentTarget(currentTarget);
                return 0;
            }
        }

        return 0;
    }

    protected Vector3 PositionOnNavMesh(Vector3 positionWanted)
    {
        LayerMask mask = LayerMask.NameToLayer("Ground");
        NavMeshHit hit;

        bool tempValid = NavMesh.SamplePosition(positionWanted, out hit, 20.0f, mask);

        Vector3 posToReturn;
        if (tempValid)
        {
            posToReturn = hit.position;
        }
        else
        {
            posToReturn = myIndividu.transform.position;
        }
        return posToReturn;
    }

}
