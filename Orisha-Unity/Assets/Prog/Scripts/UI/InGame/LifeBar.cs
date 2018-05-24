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
    [SerializeField] Image maskBG;
    [SerializeField] Image maskUI;

    [Header("LifeBar")]
    [SerializeField] Image lifeBar_Fond;
    [SerializeField] Image lifeBar_In;
    [SerializeField] Image lifeBar_Cadre;
    [Header("MaskBar")]
    [SerializeField] Image lifeBarMask_MotifPlein;
    [SerializeField] Image lifeBarMask_MotifVide;
    [SerializeField] Image lifeBarMask_In;

    [Header("Fade")]
    [SerializeField] float fadeSpeed = 1.5f;

    private RectTransform lifeBar_main;
    private RectTransform lifeBar_mask;

    private bool isHealthMask = false;
    private float lifeBarRatio = 1.0f;
    private bool isNewMaskAdded = false;

    enum CURRENT_FADE
    {
        FadeOut,
        FadeIn
    }
    CURRENT_FADE currentFade;

    // Use this for initialization
    void Start () {
        currentFade = CURRENT_FADE.FadeOut;
        FadeAllUIElements(0.0f);

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
    public void SetActiveHUD(bool _active)
    {
        if ((isNewMaskAdded || Potential_Enemy.IsOnFight) && _active == false) //Si l'UI veut se désactiver mais que l'on a un nouveau masque
        {
            //On ne fait rien
        }
        else
        {
            if (_active ? currentFade != CURRENT_FADE.FadeIn : currentFade != CURRENT_FADE.FadeOut)
            {
                StopAllCoroutines();
                StartCoroutine(FadeHUD_Loop(_active));
            }
        }
    }
    /// <summary>
    /// Désactive le HUD du masque et de la vie (Utile pour Invoke)
    /// </summary>
    public void DisableHUD()
    {
        if (isNewMaskAdded || Potential_Enemy.IsOnFight) //Si l'UI veut se désactiver mais que l'on a un nouveau masque
        {
            //On ne fait rien
        }
        else
        {
            if (currentFade != CURRENT_FADE.FadeOut)
            {
                StopAllCoroutines();
                StartCoroutine(FadeHUD_Loop(false));
            }
        }
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
    
    private IEnumerator FadeHUD_Loop(bool _active)
    {
        float localTimer = _active ? maskBG.color.a : 1 - maskBG.color.a;
        currentFade = _active ? CURRENT_FADE.FadeIn : CURRENT_FADE.FadeOut;

        if (_active)
            emptyParent.SetActive(true);
        while (localTimer < 1.0f)
        {
            localTimer = Mathf.Clamp(localTimer + Time.fixedDeltaTime * fadeSpeed, 0.0f, 1.0f);
            FadeAllUIElements(_active ? localTimer : 1 - localTimer);
            yield return null;
        }
        
        if (_active == false)
            emptyParent.SetActive(false);
        yield return null;
    }

    private void FadeAllUIElements(float _alpha)
    {
        maskBG.color = new Color(maskBG.color.r, maskBG.color.g, maskBG.color.b, _alpha);
        maskUI.color = new Color(maskUI.color.r, maskUI.color.g, maskUI.color.b, _alpha);
        lifeBar_In.color = new Color(lifeBar_In.color.r, lifeBar_In.color.g, lifeBar_In.color.b, _alpha);
        lifeBarMask_MotifPlein.color = new Color(lifeBarMask_MotifPlein.color.r, lifeBarMask_MotifPlein.color.g, lifeBarMask_MotifPlein.color.b, _alpha);
        lifeBarMask_MotifVide.color = new Color(lifeBarMask_MotifVide.color.r, lifeBarMask_MotifVide.color.g, lifeBarMask_MotifVide.color.b, _alpha);
        lifeBar_Fond.color = new Color(lifeBar_Fond.color.r, lifeBar_Fond.color.g, lifeBar_Fond.color.b, _alpha);
        lifeBarMask_In.color = new Color(lifeBarMask_In.color.r, lifeBarMask_In.color.g, lifeBarMask_In.color.b, _alpha);
        lifeBar_Cadre.color = new Color(lifeBar_Cadre.color.r, lifeBar_Cadre.color.g, lifeBar_Cadre.color.b, _alpha);
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
