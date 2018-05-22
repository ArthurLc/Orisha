/*
* @ArthurLacour
* @LifeBar.cs
* @22/05/2018
* @ - Le Script s'attache dans la prefab du Canvas_InGame
*   - Manage le HUD InGame, la barre de vie et le masque équipé
*/

using UnityEngine;
using UnityEngine.UI;

public class LifeBar : MonoBehaviour {

    [Header("Links")]
    [SerializeField] GameObject emptyParent;

    [Header("LifeBar")]
    [SerializeField] Image lifeBar_In;
    [Header("MaskBar")]
    [SerializeField] Image lifeBarMask_MotifPlein;
    [SerializeField] Image lifeBarMask_MotifVide;
    [SerializeField] Image lifeBarMask_In;

    // Use this for initialization
    void Start () {
        emptyParent.SetActive(false);
        Potential_Enemy.enteringFight += SetActiveHUD;
    }

    public void UpdateLifeBar(float _playerLifePercent)
    {
        lifeBar_In.fillAmount = _playerLifePercent;
    }

    public void SetActiveHUD(bool _active)
    {


        emptyParent.SetActive(_active);
    }
}
