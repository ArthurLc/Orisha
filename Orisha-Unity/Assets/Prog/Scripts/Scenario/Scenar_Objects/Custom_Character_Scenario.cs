using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using vd_Player;

public class Custom_Character_Scenario : Scenar_Object
{
    PlayerController controller;
    PlayerDash dash;
    [SerializeField]PlayerFight fight;

    public override void On_Scenar_Begin(string scenarName)
    {
        //Appliquer changements nécessaires au début du scénario
        //Possibilité de faire des actions différentes selon le scenario via son nom
        switch (scenarName)
        {
            case("Surrounded"):
                defaultBeginBehaviour();
                Potential_Enemy.IsOnFight = true;
                break;
            default:
                defaultBeginBehaviour();
                break;
        }
    }

    public override void On_Scenar_End(string scenarName)
    {
        //Appliquer changements nécessaires à la fin du scénario
        //Possibilité de faire des actions différentes selon le scenario via son nom
        switch (scenarName)
        {
            default:
                    defaultEndBehaviour();
                    break;
        }
    }

    /*
     * Changements par defaut appliqué au lancement d'un scénario
     * Est appellé de bas si aucun changement spécifique n'a été prévu
     * 
     */
    protected override void defaultBeginBehaviour()
    {
        if (requireTimeline)
            timeline.Play();

        //Désactive les controlles
        if (controller)
            controller.enabled = false;
        if (dash)
            dash.enabled = false;
        if (fight)
            fight.enabled = false;

        //Donne une direction de départ au joueur si cela est demandé dans le scénario
        Scenar currentScenar = Scenar_Manager.GetCurrentPlayingScenario();
        if (currentScenar && currentScenar.RedirectPlayerOnPlay)
        {
            transform.LookAt(currentScenar.PlayerBeginDest);
        }
    }


    /*
    * Changements par defaut appliqué à la fin d'un scénario
    * Est appellé de bas si aucun changement spécifique n'a été prévu
    */
    protected override void defaultEndBehaviour()
    {
        if (requireTimeline)
            timeline.Stop();

        //Réactive les controlles
        if (controller)
            controller.enabled = true;
        if (dash)
            dash.enabled = true;
        if (fight)
            fight.enabled = true;
    }

    // Use this for initialization
    public override void Start()
    {
        base.Start();

        controller = GetComponentInParent<PlayerController>();
        dash = GetComponentInParent<PlayerDash>();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public override void Init()
    {
        //Inscit le player à tous les scenarios Présents dans la scène
        foreach (KeyValuePair<string, Scenar> entry in Scenar_Manager.scenarios)
        {
            entry.Value.OnScenario_Begin += On_Scenar_Begin;
            entry.Value.OnScenario_End += On_Scenar_End;
            Debug.Log("Player Binded to scenario : " + entry.Key);
        }

		//Debug.Log ("PUTAIN DE TA RACE");
    }
}
