/*
* @ArthurLacour
* @MaskInventory.cs
* @12/03/2018
* @ - Le Script s'attache au joueur.
*   
* Contient :
*   - La liste static des masques que le joueur possède. (Initialisé au start.)
*   - Les fonctions de gestion des masques.
*   - Emissive du joueur en fonction du masque équipé.
*   - Statistiques du joueur en fonction du masque équipé.
* 
* A faire:
*/


using System.Collections.Generic;
using UnityEngine;

public class MaskInventory : MonoBehaviour {

    private List<Mask> listMasks; //Liste des masques que le joueur possède.
    [SerializeField] private Mask defaultMask; //Masque par défault...
    private Mask equipedMask = null; //Masque actuellement sur le visage du personnage principal qui possède au moins 1 masque, ni moins, mais peut-être plus encore...

    public Mask EquipedMask
    {
        get { return equipedMask; }
    }

    [Header("LinksForEmissive")]
    [SerializeField] private SkinnedMeshRenderer skin;
    [SerializeField] private MeshFilter mask_MeshFilter;
    [SerializeField] private MeshRenderer mask_MeshRenderer;
    [SerializeField] private ParticleSystem dashEffect;
    [SerializeField] public ParticleSystem maskEffect;

    [Header("LinksForFactor")]
    [SerializeField] private Animator animator;
    [SerializeField] private vd_Player.PlayerFight fightDatas;
    private vd_Player.CharacterInitialization characterInit;
    [Header("BasicEmissiveSkin")]
    [SerializeField] private string hexEmissive = "#12110F";

    [Header("Bones")]
    [SerializeField] PlayerBone leftBone;
    [SerializeField] PlayerBone rightBone;

    public static MaskInventory Instance;
    private LifeBar lifeBarHUD;

    ParticleSystem.MainModule mainModule;
    
    public List<Mask> ListMasks
    {
        get { return listMasks; }
    }

    // Use this for initialization
    private void Start () {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        //dashEffect = dashEffect.gameObject.GetComponent<ParticleSystem>();
        //maskEffect = dashEffect.gameObject.GetComponent<ParticleSystem>();
        characterInit = GetComponent<vd_Player.CharacterInitialization>();
        lifeBarHUD = FindObjectOfType<LifeBar>();
        listMasks = new List<Mask>();
        EquipDefaultMask();


    }

    public void EquipDefaultMask()
    {
        equipedMask = defaultMask;

        //Changement de la couleur emissive
        Color tatooColor = new Color();
        ColorUtility.TryParseHtmlString(hexEmissive, out tatooColor);
        skin.sharedMaterial.SetColor("_EmissionColor", tatooColor);
        mask_MeshRenderer.sharedMaterial = defaultMask.GetMaterial;
        mask_MeshFilter.sharedMesh = defaultMask.GetMesh;

        if (dashEffect)
        {
            var mainPs = dashEffect.main;
            mainPs.startColor = defaultMask.GetColor;
        }

        // A FAIRE: Modification du jeu en fonction du nouveau masque.
        animator.speed = defaultMask.SpeedFactor;
        fightDatas.StrengthFactor = defaultMask.StrengthFactor;
        characterInit.ChangeHealthFactor(defaultMask.HealthFactor);

        lifeBarHUD.UpdateHUD(equipedMask);

        var mainPS = maskEffect.main;
        mainPS.startColor = defaultMask.GetEmissiveColor;
        maskEffect.Play();

        leftBone.CurrentMask = PlayerBone.EquipedMask.Default;
        rightBone.CurrentMask = PlayerBone.EquipedMask.Default;
    }
    public void EquipAMask(Mask _newMask)
    {
        equipedMask = _newMask;

        //Changement de la couleur emissive
        skin.sharedMaterial.SetColor("_EmissionColor", _newMask.GetEmissiveColor);
        mask_MeshRenderer.sharedMaterial = _newMask.GetMaterial;
        mask_MeshFilter.sharedMesh = _newMask.GetMesh;

        if(dashEffect != null)
        {
            var mainPs = dashEffect.main;
            mainPs.startColor = _newMask.GetEmissiveColor;
        }


        // A FAIRE: Modification du jeu en fonction du nouveau masque.
        animator.speed = equipedMask.SpeedFactor;
        fightDatas.StrengthFactor = equipedMask.StrengthFactor;
        characterInit.ChangeHealthFactor(equipedMask.HealthFactor);

        lifeBarHUD.UpdateHUD(equipedMask);

        var mainPS = maskEffect.main;
        mainPS.startColor = defaultMask.GetEmissiveColor;
        maskEffect.Play();

        if(_newMask.StrengthFactor > 1)
        {
            leftBone.CurrentMask = PlayerBone.EquipedMask.Strenght;
            rightBone.CurrentMask = PlayerBone.EquipedMask.Strenght;
        }
        else if(_newMask.SpeedFactor > 1)
        {
            leftBone.CurrentMask = PlayerBone.EquipedMask.Speed;
            rightBone.CurrentMask = PlayerBone.EquipedMask.Speed;
        }
        else
        {
            leftBone.CurrentMask = PlayerBone.EquipedMask.Health;
            rightBone.CurrentMask = PlayerBone.EquipedMask.Health;
        }
    }

    public void AddAMask(Mask _newMask)
    {
        listMasks.Add(_newMask);
        lifeBarHUD.SetActiveHUD(true);
        lifeBarHUD.SetActiveNewMaskFB(true);
    }
    public void RemoveAMask(Mask _newMask)
    {
        listMasks.Remove(_newMask);
    }
    public void ResetListMask()
    {
        listMasks.RemoveRange(0, listMasks.Count);
    }

}
