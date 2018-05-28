using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthRegen : MonoBehaviour
{
    AI_Enemy_Basic me;

    [SerializeField]float hpPerSec;
    [SerializeField] ParticleSystem healSfx;

	// Use this for initialization
	void Start ()
    {
        me = GetComponent<AI_Enemy_Basic>();

    }
	
	// Update is called once per frame
	void Update ()
    {
        if(me.Health > 0)
        me.Heal(hpPerSec * Time.deltaTime);


        //Le scale avant de le faire Ddisparaitre !
        if (me.Health <= 0 && healSfx.isPlaying)
            healSfx.Stop();
    }
}
