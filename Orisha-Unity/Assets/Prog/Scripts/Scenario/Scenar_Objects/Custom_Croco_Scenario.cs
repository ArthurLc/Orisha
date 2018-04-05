using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Custom_Croco_Scenario : Scenar_Object
{
    AI_Enemy_Basic ai;
    SphereCollider trigger;

    bool ended = false;

    public override void On_Scenar_Begin(string scenarName)
    {
        switch (scenarName)
        {
            default:
                defaultBeginBehaviour();
                break;
        }
    }

    public override void On_Scenar_End(string scenarName)
    {
        switch (scenarName)
        {
            default:
                defaultEndBehaviour();
                break;
        }
    }

    // Use this for initialization
    public override void Start()
    {
        base.Start();

        ai = GetComponent<AI_Enemy_Basic>();
        trigger = GetComponent<SphereCollider>();
        ended = false;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if(!ended && ai.enabled == true)
        {
            ai.enabled = false;
            trigger.enabled = false;
        }
    }

    protected override void defaultBeginBehaviour()
    {
        if (requireTimeline)
        {
            timeline.Play();
        }
        ai.enabled = false;
        trigger.enabled = false;
        ended = false;
    }

    protected override void defaultEndBehaviour()
    {
        if (requireTimeline)
            timeline.Stop();

        ended = true;

        //Applique changements nécessaires à la fin du scénario
        ai.enabled = true;
        trigger.enabled = true;
    }
}
