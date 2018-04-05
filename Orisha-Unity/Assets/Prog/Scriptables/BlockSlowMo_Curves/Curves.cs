using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CurvesDataBase.asset", menuName = "Scriptables/Curves_DB", order = 1)]
public class Curves : ScriptableObject
{[SerializeField]
    private string[] slowMotionCurveNames;
[SerializeField]
    private AnimationCurve[] slowMotionCurves;
[SerializeField]
    public string[] SlowMotionCurveNames
    {
        get
        {
            return slowMotionCurveNames;
        }

        set
        {
            slowMotionCurveNames = value;
        }
    }
[SerializeField]
    public AnimationCurve[] SlowMotionCurves
    {
        get
        {
            return slowMotionCurves;
        }

        set
        {
            slowMotionCurves = value;
        }
    }
}
