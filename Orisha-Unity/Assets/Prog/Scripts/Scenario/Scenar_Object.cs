using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using System;

/*
 * Les objets se lient à leurs scénarios une fois le Scenar_Manarger Initialisé,
 * Peut poser des problèmes si on a plusieurs scènes 
 */

public abstract class Scenar_Object : MonoBehaviour
{
    [SerializeField]protected string[]  relatedScenarioName;
    bool initialized;

    protected PlayableDirector timeline;
    [SerializeField]protected bool requireTimeline;

    [SerializeField] bool spawnMidScenario;
    bool initSuccess;


	public virtual void Awake()
	{
		Scenar_Manager.OnScenario_Initialize += Init;
		initialized = false;
		initSuccess = false;
	}
    // Update is called once per frame
    public virtual void Update()
    {

    }

    public void OnEnable()
    {
       
    }

    public abstract void On_Scenar_Begin(string scenarName);
    public abstract void On_Scenar_End(string scenarName);


    protected abstract void defaultBeginBehaviour();

    protected abstract void defaultEndBehaviour();

    public virtual void Init()
    {
		//Debug.Log ("Init Called");

        for (int i = 0; i < relatedScenarioName.Length; i++)
            if (Scenar_Manager.scenarios.ContainsKey(relatedScenarioName[i]))
            {
                Scenar_Manager.scenarios[relatedScenarioName[i]].OnScenario_Begin += On_Scenar_Begin;
                Scenar_Manager.scenarios[relatedScenarioName[i]].OnScenario_End += On_Scenar_End;

                if (requireTimeline)
                {
                    timeline = GetComponent<PlayableDirector>();
					if (!timeline)
						Debug.LogError ("No timeline found on " + gameObject.name + ", please add a timeline or uncheck requireTimeline option.");
					else if (timeline.duration > Scenar_Manager.scenarios [relatedScenarioName [i]].MainTimeline.duration)
						Debug.LogError ("Object timeline is longer than it related scenario");
					else 
					{
						initSuccess = true;
						//Debug.Log ("Init Succesfull");
					}
                }
            }
            else
            {
                Debug.LogError("There is no Scnerio called : " + relatedScenarioName[i] + " as asked on " + gameObject.name);
            }

		
    }
}
