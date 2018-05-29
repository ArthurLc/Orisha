using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPredictionAttack : MonoBehaviour
{
    [SerializeField]
    private List<AttackPrediction> X_predictPoints;
    [SerializeField]
    private List<AttackPrediction> XX_predictPoints;
    [SerializeField]
    private List<AttackPrediction> XXX_predictPoints;
    [SerializeField]
    private List<AttackPrediction> XXXX_predictPoints;
    [SerializeField]
    private List<AttackPrediction> Y_predictPoints;
    [SerializeField]
    private List<AttackPrediction> XXY_predictPoints;
    [SerializeField]
    private List<AttackPrediction> XXYY_predictPoints;
    [SerializeField]
    private List<AttackPrediction> XXXXY_predictPoints;


    /// <summary>
    /// If the current attack will touch an enemy => return true
    /// </summary>
    /// <param name="_attackName">name of the attack (ex: X, XX, XXXXY,...)</param>
    /// <param name="playSlowMo">if true => if the attack will tuch an enemy, play slow motion</param>
    /// <returns></returns>
    public bool PredictAttack(FightScriptable.PlayerFightDatas _fightDatas, bool _playSlowMo = true)
    {
        if(_fightDatas.slowMoCurve != "")
        {
            return CurveSlowMo(_fightDatas, _playSlowMo);
        }
        else
        {
            return ClassicSlowMo(_fightDatas, _playSlowMo);
        }
    }

    private bool CurveSlowMo(FightScriptable.PlayerFightDatas _fightDatas, bool _playSlowMo)
    {
        bool willTuch = false;
        float duration = 0.0f;

        switch (_fightDatas.name)
        {
            case "X":
                duration = _fightDatas.slowMoDuration;
                if (X_predictPoints.Count > 0)
                {
                    foreach (AttackPrediction ap in X_predictPoints)
                    {
						willTuch = ap.WillTouchEnemy(_fightDatas.damages);
                        if (willTuch)
                        {
                            break;
                        }
                    }
                }
                break;
            case "XX":
                duration = _fightDatas.slowMoDuration;
                if (XX_predictPoints.Count > 0)
                {
                    foreach (AttackPrediction ap in XX_predictPoints)
                    {
					willTuch = ap.WillTouchEnemy(_fightDatas.damages);
                        if (willTuch)
                        {
                            break;
                        }
                    }
                }
                break;
            case "XXX":
                duration = _fightDatas.slowMoDuration;
                if (XXX_predictPoints.Count > 0)
                {
                    foreach (AttackPrediction ap in XXX_predictPoints)
                    {
						willTuch = ap.WillTouchEnemy(_fightDatas.damages);
                        if (willTuch)
                        {
                            break;
                        }
                    }
                }
                break;
            case "XXXX":
                duration = _fightDatas.slowMoDuration;
                if (XXXX_predictPoints.Count > 0)
                {
                    foreach (AttackPrediction ap in XXXX_predictPoints)
                    {
						willTuch = ap.WillTouchEnemy(_fightDatas.damages);
                        if (willTuch)
                        {
                            break;
                        }
                    }
                }
                break;
            case "Y":
                duration = _fightDatas.slowMoDuration;
                if (Y_predictPoints.Count > 0)
                {
                    foreach (AttackPrediction ap in Y_predictPoints)
                    {
						willTuch = ap.WillTouchEnemy(_fightDatas.damages);
                        if (willTuch)
                        {
                            break;
                        }
                    }
                }
                break;
            case "XXY":
                duration = _fightDatas.slowMoDuration;
                if (XXY_predictPoints.Count > 0)
                {
                    foreach (AttackPrediction ap in XXY_predictPoints)
                    {
						willTuch = ap.WillTouchEnemy(_fightDatas.damages);
                        if (willTuch)
                        {
                            break;
                        }
                    }
                }
                break;
            case "XXYY":
                duration = _fightDatas.slowMoDuration;
                if (XXYY_predictPoints.Count > 0)
                {
                    foreach (AttackPrediction ap in XXYY_predictPoints)
                    {
						willTuch = ap.WillTouchEnemy(_fightDatas.damages);
                        if (willTuch)
                        {
                            break;
                        }
                    }
                }
                break;
            case "XXXXY":
                duration = _fightDatas.slowMoDuration;
                if (XXXXY_predictPoints.Count > 0)
                {
                    foreach (AttackPrediction ap in XXXXY_predictPoints)
                    {
						willTuch = ap.WillTouchEnemy(_fightDatas.damages);
                        if (willTuch)
                        {
                            break;
                        }
                    }
                }
                break;
            default:
                Debug.LogWarning("SlowMo': Aucune attaque n'a était trouvé pour lancer la SlowMo'.");
                break;
        }

        if (_playSlowMo && willTuch && duration > 0.0f)
        {
            //Debug.Log("SlowMo");
            TimeManager.Instance.Slow_OneCharacter_WithCurve(this.gameObject, duration, _fightDatas.slowMoCurve);

            LayerMask l_mask = 1 << LayerMask.NameToLayer("Enemy");
            Collider[] results = Physics.OverlapSphere(transform.position, 10.0f, l_mask, QueryTriggerInteraction.Collide);

            if (results.Length > 0)
            {
                for (int i = 0; i < results.Length; i++)
                {
                    if (results[i].gameObject.transform.parent != null)
                        TimeManager.Instance.Slow_OneCharacter_WithCurve(results[i].gameObject.transform.parent.gameObject, duration, _fightDatas.slowMoCurve);
                    //Debug.Log (results [i].name);
                }
            }
        }

        return willTuch;
    }
    private bool ClassicSlowMo(FightScriptable.PlayerFightDatas _fightDatas, bool _playSlowMo)
    {
        bool willTuch = false;
        float duration = 0.0f;

        switch (_fightDatas.name)
        {
            case "X":
                duration = _fightDatas.slowMoDuration;
                if (X_predictPoints.Count > 0)
                {
                    foreach (AttackPrediction ap in X_predictPoints)
                    {
						willTuch = ap.WillTouchEnemy(_fightDatas.damages);
                        if (willTuch)
                        {
                            break;
                        }
                    }
                }
                break;
            case "XX":
                duration = _fightDatas.slowMoDuration;
                if (XX_predictPoints.Count > 0)
                {
                    foreach (AttackPrediction ap in XX_predictPoints)
                    {
						willTuch = ap.WillTouchEnemy(_fightDatas.damages);
                        if (willTuch)
                        {
                            break;
                        }
                    }
                }
                break;
            case "XXX":
                duration = _fightDatas.slowMoDuration;
                if (XXX_predictPoints.Count > 0)
                {
                    foreach (AttackPrediction ap in XXX_predictPoints)
                    {
						willTuch = ap.WillTouchEnemy(_fightDatas.damages);
                        if (willTuch)
                        {
                            break;
                        }
                    }
                }
                break;
            case "XXXX":
                duration = _fightDatas.slowMoDuration;
                if (XXXX_predictPoints.Count > 0)
                {
                    foreach (AttackPrediction ap in XXXX_predictPoints)
                    {
						willTuch = ap.WillTouchEnemy(_fightDatas.damages);
                        if (willTuch)
                        {
                            break;
                        }
                    }
                }
                break;
            case "Y":
                duration = _fightDatas.slowMoDuration;
                if (Y_predictPoints.Count > 0)
                {
                    foreach (AttackPrediction ap in Y_predictPoints)
                    {
						willTuch = ap.WillTouchEnemy(_fightDatas.damages);
                        if (willTuch)
                        {
                            break;
                        }
                    }
                }
                break;
            case "XXY":
                duration = _fightDatas.slowMoDuration;
                if (XXY_predictPoints.Count > 0)
                {
                    foreach (AttackPrediction ap in XXY_predictPoints)
                    {
						willTuch = ap.WillTouchEnemy(_fightDatas.damages);
                        if (willTuch)
                        {
                            break;
                        }
                    }
                }
                break;
            case "XXYY":
                duration = _fightDatas.slowMoDuration;
                if (XXYY_predictPoints.Count > 0)
                {
                    foreach (AttackPrediction ap in XXYY_predictPoints)
                    {
						willTuch = ap.WillTouchEnemy(_fightDatas.damages);
                        if (willTuch)
                        {
                            break;
                        }
                    }
                }
                break;
            case "XXXXY":
                duration = _fightDatas.slowMoDuration;
                if (XXXXY_predictPoints.Count > 0)
                {
                    foreach (AttackPrediction ap in XXXXY_predictPoints)
                    {
						willTuch = ap.WillTouchEnemy(_fightDatas.damages);
                        if (willTuch)
                        {
                            break;
                        }
                    }
                }
                break;
            default:
                Debug.LogWarning("SlowMo': Aucune attaque n'a était trouvé pour lancer la SlowMo'.");
                break;
        }

        if (_playSlowMo && willTuch && duration > 0.0f)
        {
            //Debug.Log("SlowMo");
            TimeManager.Instance.Slow_OneCharacter_WithTimer(this.gameObject, duration, _fightDatas.slowMoValue);

            LayerMask l_mask = 1 << LayerMask.NameToLayer("Enemy");
            Collider[] results = Physics.OverlapSphere(transform.position, 10.0f, l_mask, QueryTriggerInteraction.Collide);

            if (results.Length > 0)
            {
                for (int i = 0; i < results.Length; i++)
                {
               
                    if(results[i].gameObject.transform.parent != null)
                        TimeManager.Instance.Slow_OneCharacter_WithTimer(results[i].gameObject.transform.parent.gameObject, duration, _fightDatas.slowMoValue);
                    //Debug.Log (results [i].name);
                }
            }
        }

        return willTuch;
    }

}
