/*
* @AmbreLacour
* @PlayerBone.cs
* @07/02/2017
* @Le script s'attache là au gameobject qui contient le mesh et le collider de l'arme du player (l'os géant)
*   - Il est appelé par le script qui gère les events liés à 'lanimation du joueur (via lui, il fait apparaitre/disparaitre les os du joueur)
*   - C'est ici qu'on peut gérer l'esthétique d'apparition/disparition des armes (particle system & co)
*   
*/

using UnityEngine;
using vd_Inputs;
using vd_Player;

[RequireComponent(typeof(MeshRenderer), typeof(Collider))]
public class PlayerBone : MonoBehaviour
{
    private MeshRenderer rend;
    private Collider coll;

    private ParticleSystem psCurrent;
    public ParticleSystem PsCurrent
    {
        get{ return psCurrent; }
    }
    [SerializeField] private ParticleSystem psDefault;
    [SerializeField] private ParticleSystem psStrength;
    [SerializeField] private ParticleSystem psSpeed;
    [SerializeField] private ParticleSystem psHealth;

    public enum EquipedMask
    {
        Default,
        Strenght,
        Speed,
        Health,
    }
    EquipedMask currentMask;
    public EquipedMask CurrentMask
    {
        get
        {
            return currentMask;
        }
        set
        {
            currentMask = value;
            switch(value)
            {
                case EquipedMask.Strenght:
                    psCurrent = psStrength;
                    break;
                case EquipedMask.Health:
                    psCurrent = psHealth;
                    break;
                case EquipedMask.Speed:
                    psCurrent = psSpeed;
                    break;
                case EquipedMask.Default:
                    psCurrent = psDefault;
                    break;
            }


        }
    }

    float inactivityTimer = 0.0f;
    bool isVisible = false;

	void Start ()
    {
        rend = GetComponent<MeshRenderer>();
        coll = GetComponent<Collider>();
        psCurrent = psDefault;
        rend.enabled = false;
        coll.enabled = false;
    }

    private void Update()
    {
        if (isVisible)
        {
            inactivityTimer += Time.deltaTime;

            if (Input.GetButtonDown(InputManager.AttackHeavy) || Input.GetButtonDown(InputManager.AttackSlight))
            {
                inactivityTimer = 0.0f;
            }

            else if (inactivityTimer >= 5.0f)
            {
                SpawnBone(false);
                inactivityTimer = 0.0f;
            }
        }
    }

    /// <summary>
    /// Fait apparaitre ou disparaitre l'arme du joueur (avec un effet de particle system)
    /// </summary>
    /// <param name="isVisible"></param>
    public void SpawnBone(bool _isVisible)
    {
        rend.enabled = _isVisible;
        coll.enabled = _isVisible;
        isVisible = _isVisible;

        if(_isVisible == false)
        {
            //psCurrent.Play();

        }
    }
}
