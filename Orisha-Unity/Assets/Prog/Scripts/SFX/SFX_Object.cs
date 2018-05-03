﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFX_Object : MonoBehaviour 
{
	public AudioSource source;

	void Awake()
	{
		source = GetComponent<AudioSource> ();
		//source.Play ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!source.isPlaying)
			SFX_Pool.ReturnToPool (gameObject);
	}
}