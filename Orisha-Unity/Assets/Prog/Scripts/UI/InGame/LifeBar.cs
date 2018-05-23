/*
* @ArthurLacour
* @LifeBar.cs
* @22/05/2018
* @ - Le Script s'attache dans la prefab du Canvas_InGame
*   - Manage le HUD InGame, la barre de vie et le masque équipé
*/

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LifeBar : MonoBehaviour {

    [Header("Links")]
    [SerializeField] GameObject emptyParent;
    [SerializeField] GameObject newMaskFB;
    [SerializeField] Image maskUI;

    [Header("LifeBar")]
    [SerializeField] Image lifeBar_In;
    [Header("MaskBar")]
    [SerializeField] Image lifeBarMask_MotifPlein;
    [SerializeField] Image lifeBarMask_MotifVide;
    [SerializeField] Image lifeBarMask_In;

    private RectTransform lifeBar_main;
    private RectTransform lifeBar_mask;

    private bool isHealthMask = false;
    private float lifeBarRatio = 1.0f;
    private bool isNewMaskAdded = false;

    // Use this for initialization
    void Start () {
        emptyParent.SetActive(false);
        SetActiveHUD(false);
        SetActiveNewMaskFB(false);
        Potential_Enemy.enteringFight += SetActiveHUD;

        lifeBar_main = lifeBar_In.GetComponent<RectTransform>();
        lifeBar_mask = lifeBarMask_In.GetComponent<RectTransform>();
    }

    public void UpdateLifeBar(float _playerLifePercent)
    {
        lifeBarRatio = _playerLifePercent;

        if (isHealthMask)
        {
            if (lifeBarRatio >= (lifeBar_mask.rect.width / lifeBar_main.rect.width))
            {
                lifeBarMask_MotifPlein.fillAmount = (lifeBarRatio - (lifeBar_mask.rect.width / lifeBar_main.rect.width)) * 2;
                lifeBarMask_MotifVide.fillAmount = (lifeBarRatio - (lifeBar_mask.rect.width / lifeBar_main.rect.width)) * 2;
                lifeBarMask_In.fillAmount = (lifeBarRatio - (lifeBar_mask.rect.width / lifeBar_main.rect.width)) * 2;
                lifeBar_In.fillAmount = 1.0f;
            }
            else
            {
                lifeBarMask_MotifPlein.fillAmount = 0.0f;
                lifeBarMask_MotifVide.fillAmount = 0.0f;
                lifeBarMask_In.fillAmount = 0.0f;
                lifeBar_In.fillAmount = lifeBarRatio / (lifeBar_mask.rect.width / lifeBar_main.rect.width);
            }
        }
        else
            lifeBar_In.fillAmount = lifeBarRatio;
    }

    /// <summary>
    /// Active/Desactive le HUD du masque et de la vie
    /// </summary>
    /// <param name="_active"></param>
    /// <param name="_duration"> Durée d'activation du l'UI. (0.0f == active l'UI sans timer)</param>
    public void SetActiveHUD(bool _active)
    {
        if (isNewMaskAdded && _active == false) //Si l'UI veut se désactiver mais que l'on a un nouveau masque
        {
            //On ne fait rien
        }
        else
            emptyParent.SetActive(_active);
    }
    /// <summary>
    /// Active/Desactive le Feedback de la touche d'un nouveau mask
    /// </summary>
    /// <param name="_active"></param>
    public void SetActiveNewMaskFB(bool _active)
    {
        isNewMaskAdded = _active;
        newMaskFB.SetActive(_active);
    }

    public IEnumerator ActiveHUD_WithTimer(float _timer)
    {
        float localTimer = _timer;

        SetActiveHUD(true);
        while (localTimer > 0)
        {
            localTimer -= Time.fixedDeltaTime;
            yield return null;
        }
        
        if(isNewMaskAdded == false && Potential_Enemy.IsOnFight == false)
            emptyParent.SetActive(false);
        yield return null;
    }

    /// <summary>
    /// Mise à jour des éléments dans le HUD
    /// </summary>
    /// <param name="_mask"></param>
    public void UpdateHUD(Mask _mask)
    {
        maskUI.sprite = _mask.Sprite2D;

        lifeBarMask_MotifPlein.enabled = _mask.HealthFactor > 1;
        lifeBarMask_MotifVide.enabled = _mask.HealthFactor > 1;
        lifeBarMask_In.enabled = _mask.HealthFactor > 1;

        isHealthMask = _mask.HealthFactor > 1;
        UpdateLifeBar(lifeBarRatio);
    }
}
