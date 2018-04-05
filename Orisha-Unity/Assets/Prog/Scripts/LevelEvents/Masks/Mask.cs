/*
* @ArthurLacour
* @Mask.cs
* @12/03/2018
* @ - Le Script s'attache sur un objet quelconque.
*   
* Détails :
*   - Le script regroupe toutes les informations concernant le masque.
*   - Il est voué à se retrouvé dans la liste de masques que le joueur possède. (Une fois ramassé par celui-ci.)
*   - Le "colTrigger" constitue une partie de code nécessaire. (C'est lorsque que se dernier est disabled que le masque s'ajoute à la liste.)
*/


using UnityEngine;
using UnityEngine.UI;

public class Mask : MonoBehaviour {

    // Informations
    [SerializeField] private string name = "MaskName";
    [SerializeField] [Range(0.5f, 2.0f)] private float healthFactor = 1.0f;
    [SerializeField][Range(0.5f, 2.0f)] private float speedFactor = 1.0f;
    [SerializeField] [Range(0.5f, 2.0f)] private float strengthFactor = 1.0f;
    [SerializeField] private MeshRenderer mesh;
    [SerializeField] private Sprite sprite2D;

    //
    private Collider colTrigger;

    #region Getteurs
    public string Name
    {
        get { return name; }
    }
    public float HealthFactor
    {
        get { return healthFactor; }
    }
    public float SpeedFactor
    {
        get { return speedFactor; }
    }
    public float StrengthFactor
    {
        get { return strengthFactor; }
    }
    public Color GetColor
    {
        get { return mesh.sharedMaterial.color; }
    }
    public Sprite Sprite2D
    {
        get { return sprite2D; }
    }
    #endregion

    private void Start() {
        colTrigger = GetComponent<Collider>();
    }
    private void Update()
    {
        if (colTrigger.enabled == false)
            AddThisMaskToTheList();
    }

    /// <summary>
    /// Ajout du mask à la liste des masques.
    /// </summary>
    private void AddThisMaskToTheList()
    {
        MaskInventory.Instance.AddAMask(this);
        Destroy(this);
    }
}
