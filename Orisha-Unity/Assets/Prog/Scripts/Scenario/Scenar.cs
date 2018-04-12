using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Events;


/*
 * Le scenario doit avoir une timeline, même vide 
 * 
 */

[RequireComponent(typeof(PlayableDirector))]
public class Scenar : MonoBehaviour
{
    [SerializeField]private string scenarName;
    public string ScenarName { get { return scenarName; } }

    public delegate void onStateChange(string ScenarName);
    public event onStateChange OnScenario_Begin;
    [SerializeField]Transform playerBeginDest;
    public event onStateChange OnScenario_End;


    PlayableDirector mainTimeline;
    public PlayableDirector MainTimeline { get { return mainTimeline; } }

    [HideInInspector]public bool isPlaying;

    [Space,Space]
    [SerializeField]bool playOneTimeOnly;
    bool hasBeenPlayed;

    [SerializeField]bool endWithTheTimeline = true;


    [SerializeField]bool redirectPlayerOnPlay;
    public bool RedirectPlayerOnPlay { get { return redirectPlayerOnPlay; } }
    public Transform PlayerBeginDest { get { return playerBeginDest; } }

	private FightWall wall;

    // Use this for initialization
    void Start()
    {
        isPlaying = false;
        mainTimeline = GetComponent<PlayableDirector>();
        hasBeenPlayed = false;

		wall = FindObjectOfType<FightWall>();
    }
	
	// Update is called once per frame
	void Update ()
    {
		if(mainTimeline.state != PlayState.Playing && endWithTheTimeline)
        {
            EndScenar();
        }
	}

   public void EndScenar()
    {
        if(isPlaying)
        {
            mainTimeline.Stop();
            isPlaying = false;
            OnScenario_End(scenarName);
            hasBeenPlayed = true;
        }         
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if ((playOneTimeOnly && !hasBeenPlayed && !isPlaying) || !playOneTimeOnly && !isPlaying)
            {
				wall.SetTrWall (playerBeginDest.position);
				wall.SetSizeWall (30.0f);
                mainTimeline.Play();
                isPlaying = true;
                OnScenario_Begin(scenarName);
            }     
        }
    }
}
