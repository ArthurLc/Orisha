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

    [Header("LinksForEmissive")]
    [SerializeField] private SkinnedMeshRenderer skin;
    [SerializeField] private MeshFilter mask_MeshFilter;
    [SerializeField] private MeshRenderer mask_MeshRenderer;
    [SerializeField] private ParticleSystem dashEffect;
    [Header("LinksForFactor")]
    [SerializeField] private Animator animator;
    [SerializeField] private vd_Player.PlayerFight fightDatas;
    private vd_Player.CharacterInitialization characterInit;

    public static MaskInventory Instance;

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

        characterInit = GetComponent<vd_Player.CharacterInitialization>();
        listMasks = new List<Mask>();
        EquipDefaultMask();
    }

    public void EquipDefaultMask()
    {
        equipedMask = defaultMask;

        //Changement de la couleur emissive
        skin.sharedMaterial.SetColor("_EmissionColor", defaultMask.GetColor);
        mask_MeshRenderer.sharedMaterial = defaultMask.GetMaterial;
        mask_MeshFilter.sharedMesh = defaultMask.GetMesh;
        dashEffect.startColor = Color.white;

        // A FAIRE: Modification du jeu en fonction du nouveau masque.
        animator.speed = defaultMask.SpeedFactor;
        fightDatas.StrengthFactor = defaultMask.StrengthFactor;
        characterInit.ChangeHealthFactor(defaultMask.HealthFactor);
    }
    public void EquipAMask(Mask _newMask)
    {
        equipedMask = _newMask;

        //Changement de la couleur emissive
        skin.sharedMaterial.SetColor("_EmissionColor", _newMask.GetColor);
        mask_MeshRenderer.sharedMaterial = _newMask.GetMaterial;
        mask_MeshFilter.sharedMesh = _newMask.GetMesh;
        dashEffect.startColor = _newMask.GetColor;

        // A FAIRE: Modification du jeu en fonction du nouveau masque.
        animator.speed = equipedMask.SpeedFactor;
        fightDatas.StrengthFactor = equipedMask.StrengthFactor;
        characterInit.ChangeHealthFactor(equipedMask.HealthFactor);
    }

    public void AddAMask(Mask _newMask)
    {
        listMasks.Add(_newMask);
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
