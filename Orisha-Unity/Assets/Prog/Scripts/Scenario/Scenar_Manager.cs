using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Scenar_Manager : MonoBehaviour
{
    public static Dictionary<string, Scenar> scenarios;

    public delegate void ScenarManagerVoidEvent();
    public static event ScenarManagerVoidEvent OnScenario_Initialize;
    public static bool initialized;

	// Use this for initialization
	void Start ()
	{
		scenarios = new Dictionary<string, Scenar>();
		SceneManager.sceneLoaded += InitMangerForCurrentScene;
		InitManager ();
	}

	void InitMangerForCurrentScene(Scene scene, LoadSceneMode mode)
	{
		InitManager ();
	}

	void InitManager()
	{
		initialized = false;
		Scenar[] scenars = GameObject.FindObjectsOfType<Scenar>();
		scenarios.Clear ();

		//sauvegarde les scenarios dans un dictionaire afin de les retrouver facilement par leurs noms
		for(int i = 0; i < scenars.Length; ++i)
		{
			if (!scenarios.ContainsKey(scenars[i].ScenarName))
				scenarios.Add(scenars[i].ScenarName, scenars[i]);
			else
				Debug.LogError("Another scenario with the same name Already Exist : " + scenars[i].ScenarName);
		}


		foreach (KeyValuePair<string, Scenar> entry in scenarios)
		{
			if (entry.Value.isPlaying)
			{
				Debug.Log(entry.Value.ScenarName);
			}
		}

		//Debug.Log ("OUIIIIIIIIIIIIIIIIIIIIIIIIIII");
		OnScenario_Initialize();
		initialized = true;
	}
	
	// Update is called once per frame
	void Update ()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.L))
            EndRunnigEvent();
        #endif
    }

    public static Scenar GetCurrentPlayingScenario()
    {
        foreach (KeyValuePair<string, Scenar> entry in scenarios)
        {
            if (entry.Value.isPlaying)
            {
                return entry.Value;
            }
        }
        return null;
    }

    public static void Remove_Scenario(Scenar to_Remove)
    {
        if (scenarios.ContainsKey(to_Remove.name))
            scenarios.Remove(to_Remove.name);
    }

    public static void EndRunnigEvent()
    {
        foreach (KeyValuePair<string, Scenar> entry in scenarios)
        {
            if(entry.Value.isPlaying)
            {
                entry.Value.EndScenar();
            }
        }
    }
}
