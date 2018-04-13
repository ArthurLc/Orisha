using UnityEngine;

/*
* @AmbreLacour
* @HittenBox_Player.cs
* @27/11/2017
* @Le script s'attache à un trigger pour prendre les dégâts
*   - Il contient la réaction de l'ennemi quand il est touché
*   - Il tourne en boucle mais n'a pas d'update (il guette les collisions avec des damagebox)
*   - Il hérite de l'interface HittenBox
*/
public enum E_HittenBodyPart
{
    head,
    main_Body,
    Arm_Right,
    Arm_Left,
    Leg_Right,
    Leg_Left
}

public class HittenBox_Enemy : MonoBehaviour, HittenBox
{
    [SerializeField] private AI_Enemy_Basic enemy;
    [SerializeField] private E_HittenBodyPart partOfBody;
    private CameraShaker camShaker = null;


    public virtual void TakeDamage(int _damage)
    {
        if (enemy != null)
            enemy.TakeDamage(_damage);
        //Debug.Log("Enemy hitten");

        // Mettre le code de l'ennemi qui prend cher ici
    }
    private void PropulseAgent(DamageBox _box)
    {
        int currentExpulsionLvl = (int)_box.PlayerDatas.PlayerFightDatas.expulsionLevel;

        //Debug.Log("J'ai pris " + box.PlayerDatas.PlayerFightDatas.damages + " degats d'un joueur avec l'attaque " + box.PlayerDatas.PlayerFightDatas.name);
        Vector3 PropulseDir = Vector3.zero;
        switch (_box.PlayerDatas.PlayerFightDatas.expulsionDir)
        {
            case FightScriptable.ExpulsionDir.Forward:
                PropulseDir = new Vector3(_box.EntityTransform.forward.x, 0.5f, _box.EntityTransform.forward.z);
                break;
            case FightScriptable.ExpulsionDir.ForwardUp:
                PropulseDir = new Vector3(_box.EntityTransform.forward.x, 4.0f, _box.EntityTransform.forward.z);
                break;
            case FightScriptable.ExpulsionDir.Left:
                PropulseDir = new Vector3(-_box.EntityTransform.right.x, 0.5f, -_box.EntityTransform.right.z);
                break;
            case FightScriptable.ExpulsionDir.LeftUp:
                PropulseDir = new Vector3(-_box.EntityTransform.right.x, 4.0f, -_box.EntityTransform.right.z);
                break;
            case FightScriptable.ExpulsionDir.Right:
                PropulseDir = new Vector3(_box.EntityTransform.right.x, 0.5f, _box.EntityTransform.right.z);
                break;
            case FightScriptable.ExpulsionDir.RightUp:
                PropulseDir = new Vector3(_box.EntityTransform.right.x, 4.0f, _box.EntityTransform.right.z);
                break;
            case FightScriptable.ExpulsionDir.Up:
                PropulseDir = Vector3.up;
                break;
            case FightScriptable.ExpulsionDir.Down:
                PropulseDir = Vector3.zero;
                break;
            default:
                break;
        }

        enemy.MyCurrentState.PropulseAgent(PropulseDir * currentExpulsionLvl);
        Debug.DrawRay(transform.position, PropulseDir, Color.blue, currentExpulsionLvl);
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
            if(camShaker == null)
                camShaker = other.GetComponentInParent<CameraShaker>();

            // je prends mes dégats
            //TakeDamage(box.damageValue);
            //Temp pour la millestone Applique les damages de l'anim du joueur si c'est un player qui a frappé
            if (enemy.DmgBoxList != null && enemy.DmgBoxList.Contains(box) == false)
            {
                enemy.DmgBoxList.Add(box); //Ajout de la box à la liste.

                //Debug.Log("player makes dammages");
                if (box.PlayerDatas)
                {
                    int currentAttackDmg = (int)(box.PlayerDatas.PlayerFightDatas.damages * box.PlayerDatas.StrengthFactor);
                    TakeDamage(currentAttackDmg == 0 ? box.damageValue : currentAttackDmg);
                    PropulseAgent(box);
                    float vibrationLevel = (int)box.PlayerDatas.PlayerFightDatas.expulsionLevel / 20.0f;
                    vd_Inputs.InputManager.GamePad_StartVibration(0, vibrationLevel, vibrationLevel, vibrationLevel);
                    
					if(box.PlayerDatas.PlayerFightDatas.expulsionLevel == FightScriptable.ExpulsionLevel.High)
                    	camShaker.ShakeActualCam(vibrationLevel, vibrationLevel, vibrationLevel / 20.0f);
                }
                else
                    TakeDamage(box.damageValue);

                // je dis à l'autre de prendre les dégats            
                box.HitSucceed(other.ClosestPointOnBounds(transform.position));
            }
        }
        else
        {
            //peut s'afficher quand un Ennemi projeté touche un élément du décors
            //Debug.LogWarning("Je tape un truc qui a pas de DamageBox : cépa normal");
            //Debug.Log(other.ToString());
        }
    }
}
