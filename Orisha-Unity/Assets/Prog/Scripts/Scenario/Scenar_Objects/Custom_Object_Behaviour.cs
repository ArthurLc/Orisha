using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Custom_Object_Behaviour : Scenar_Object
{
    public override void On_Scenar_Begin(string scenarName)
    {
        switch(scenarName)
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
        //Appliquer changements nécessaires à la fin du scénario
    }

    // Use this for initialization
    public override void Awake ()
    {
        base.Awake();
    }

    // Update is called once per frame
    public override void Update () 
    {
        base.Update();
	}

    protected override void defaultBeginBehaviour()
    {
        if (requireTimeline)
        {
            timeline.Play();
        }
    }

    protected override void defaultEndBehaviour()
    {
        if (requireTimeline)
            timeline.Stop();
    }
}
