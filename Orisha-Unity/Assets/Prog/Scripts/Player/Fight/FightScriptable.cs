using UnityEngine;
using System.Collections.Generic;

/*
* @ArthurLacour
* @FightScriptable.cs
* @10/11/2017
* @Le script contient les informations nécessaire à la réalisation d'attaques & combos.
*/

[CreateAssetMenu(fileName = "FightAnimator", menuName = "Scriptables/FightAnimator", order = 1)]
public class FightScriptable : ScriptableObject
{
    [SerializeField] bool debugConsole = false;

    public enum InputRequire //Enum qui permet de savoir quels sont les combinaisons requisent pour les attaques
    {
        AttackSlight,
        AttackHeavy
    }
    public enum ExpulsionLevel //Enum qui détermine le niveau de propulsion d'une attaque à l'impact
    {
        Null, // Ne fait rien
        Low = 2, // Fait bouger l'ennemi
        Medium = 5, // Propulse l'ennemi
        High = 10 // Le fait s'envoler
    }
    public enum ExpulsionDir //Enum qui détermine la direction de propulsion d'une attaque à l'impact
    {
        Forward,
        ForwardUp,
        Left,
        LeftUp,
        Right,
        RightUp,
        Up,
        Down
    }
    public enum SlowMoLevel //Enum qui détermine la vitesse de ralentissement d'une attaque (SlowMotion)
    {
        VerySlow = 2, // Vraiment lent
        Slow = 5, // Lent
        LittleSlow = 7 // Un peu lent
    }


    [System.Serializable]
    public struct AttackStruct
    {
        [Header("Basics")]
        public string name; //Nom de l'attaque (Important qu'il soit identique à celui dans l'animator.)
        public int damages; //Dégâts l'attaque.
        public List<InputRequire> inputsRequire; //Liste d'inputs nécessaire au lancement de cette attaque.
        [Header("Expulsion")]
        public ExpulsionLevel expulsionLevel; //Niveau d'expulsion qu'emet une attaque à l'impact.
        public ExpulsionDir expulsionDir; //Direction d'expulsion que suit la victime de l'attaque à l'impact.
        [Header("SlowMotion")]
        public SlowMoLevel slowMoLevel; //Vitesse de ralentissement d'une attaque.
        [Range(0.0f, 10.0f)] public float slowMoDuration; //Durée du ralentissement d'une attaque.
        public string slowMoCurve; //Nom de la curve de SlowMo' correspondante. (TimeManager)
        [Header("Feedbacks")]
        public AudioClip soundImpact; //Song joué à l'impact de l'attaque.
        public ParticleSystem psImpact; //ParticleSystem joué à l'impact de l'attaque.
    }

    public struct PlayerFightDatas
    {
        public string name; //Nom de l'attaque.
        public int damages; //Dégâts l'attaque.
        public string name_anim; //Nom de l'attaque/animation correspondante.
        public ExpulsionLevel expulsionLevel; //Niveau d'expulsion qu'emet une attaque à l'impact.
        public ExpulsionDir expulsionDir; //Direction d'expulsion que suit la victime de l'attaque à l'impact.
        public float slowMoValue; //Vitesse de ralentissement d'une attaque.
        public float slowMoDuration; //Durée du ralentissement d'une attaque.
        public string slowMoCurve; //Nom de la curve de SlowMo' correspondante. (TimeManager)
        public AudioClip soundImpact; //Song joué à l'impact de l'attaque.
        public ParticleSystem psImpact; //ParticleSystem joué à l'impact de l'attaque.
    }

    [SerializeField] public AttackStruct[] attackList;

    List<InputRequire> tempList = new List<InputRequire>();
    PlayerFightDatas tempPFD = new PlayerFightDatas();


    /// <summary> Renvois l'attaque correspondant à une liste d'inputs.
    /// 
    /// </summary>
    /// <param name="_listInputs">Liste d'inputs dont je veux récupérer la struct.</param>
    /// <returns></returns>
    public AttackStruct GetAttackStruct(List<InputRequire> _listInputs)
    {
        for (int i = 0; i < attackList.Length; i++) //Parcours toute les coups
        {
            if (AreListsEquals(attackList[i].inputsRequire, _listInputs))
                return attackList[i]; //Renvoit l'attaque correspondante.
        }

        return default(AttackStruct);
    }
    /// <summary> Retourne le nombre d'inputs requit pour l'animation qui en requière le plus.
    /// 
    /// </summary>
    /// <returns></returns>
    public int GetNumInputsMax()
    {
        int iNumMaxActions = 0;
        for(int i = 0; i < attackList.Length; i++) //Parcours toute les coups
        {
            if(attackList[i].inputsRequire.Count > iNumMaxActions) //Si le nombre de coups de ce coup > stock
                iNumMaxActions = attackList[i].inputsRequire.Count; //Alors je stock
        }

        return iNumMaxActions;
    }
    
    /// <summary> Retourne vraie si une liste d'inputs existe ou pas.
    /// 
    /// </summary>
    /// <param name="_listInputs">Actuel liste d'inputs.</param>
    /// <param name="_nextInput">Nouveau Input potentiel à la chaîne.</param>
    /// <returns></returns>
    public bool IsNextChainExist(List<InputRequire> _listInputs, InputRequire _nextInput)
    {
        //Simulation d'une liste
        tempList.Clear();
        tempList.Capacity = 0;
        for(int i = 0; i < _listInputs.Count; i++) 
            tempList.Add(_listInputs[i]); //Copy de la liste
        tempList.Add(_nextInput); //Ajout de l'input suivant
        //

        for (int i = 0; i < attackList.Length; i++) //Parcours toute les coups
        {
            if (AreListsEquals(attackList[i].inputsRequire, tempList))
                return true;
        }

        if (debugConsole) {
            Debug.Log("Chain doesn't exist");
        }
        return false;
    }
    /// <summary> Retourne le nom de l'animation correspondant à une liste d'inputs.
    /// 
    /// </summary>
    /// <param name="_listInputs">Liste d'inputs correspondant à l'animation dont je veux récupérer le nom.</param>
    /// <returns></returns>
    public string GetAttackNameOnDataList(List<InputRequire> _listInputs)
    {
        for (int i = 0; i < attackList.Length; i++) //Parcours toute les coups
        {
            if (attackList[i].inputsRequire == _listInputs)
                return attackList[i].name; //Renvoit le nom si la liste d'inputs correspond
        }

        return null;
    }

    /// <summary> Set un PlayerFightDatas en fonction d'une animation.
    /// 
    /// </summary>
    /// <param name="_animClip">Animation actuel.</param>
    /// <param name="_fightType">Id du joueur.</param>
    public PlayerFightDatas SetCurrentPFD(List<InputRequire> _listInputs)
    {
        AttackStruct tempAttackStruct = GetAttackStruct(_listInputs);
        tempPFD.name = tempAttackStruct.name;
        tempPFD.damages = tempAttackStruct.damages;
        tempPFD.name_anim = tempAttackStruct.name;
        tempPFD.expulsionLevel = tempAttackStruct.expulsionLevel;
        tempPFD.expulsionDir = tempAttackStruct.expulsionDir;
        tempPFD.slowMoValue = ((int)tempAttackStruct.slowMoLevel / 10.0f);
        tempPFD.slowMoDuration = tempAttackStruct.slowMoDuration;
        tempPFD.slowMoCurve = tempAttackStruct.slowMoCurve;
        tempPFD.soundImpact = tempAttackStruct.soundImpact;
        tempPFD.psImpact = tempAttackStruct.psImpact;

        return tempPFD;
    }
    /// <summary> Erase un PlayerFightDatas.
    /// 
    /// </summary>
    /// <param name="_pfd">PlayerFightDatas à modifier.</param>
    public PlayerFightDatas EraseCurrentPFD()
    {
        return default(PlayerFightDatas);
    }


    /// <summary> Retourne vraie si 2 listes sont égales.
    /// 
    /// </summary>
    /// <param name="_l1">Première liste.</param>
    /// <param name="_l2">Seconde liste.</param>
    /// <returns></returns>
    public bool AreListsEquals(List<InputRequire> _l1, List<InputRequire> _l2)
    {
        if (_l1.Count != _l2.Count) //Sécuritée, si les listes n'ont pas le même nombre des coups
            return false;
        for (int i = 0; i < _l1.Count; i++) //Parcours tous les coups.
        {
            if (_l1[i] != _l2[i]) //Si au moins 1 coup est différent...
                return false; //...les listes le sont.
        }

        return true; //Sinon, les listes sont identiques.
    }
}
