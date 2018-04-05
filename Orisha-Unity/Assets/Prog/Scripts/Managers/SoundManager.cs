using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Est sujet à trnsformation en vrai manager, 
 * sert pour l'instant à intégrer rapidement les sons pour la milestone
 * 
 * 
 * 
 */


public class SoundManager : MonoBehaviour
{
    public static AudioSource source;

	// Use this for initialization
	void Start ()
    {
        source = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update ()
    {

	}
}
