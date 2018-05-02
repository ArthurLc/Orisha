using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/*
 * 	
 * 
 * 
 */


public class SoundManager : MonoBehaviour
{
    public AudioSource sfxSource;
	public AudioSource musicSource;

	public static SoundManager instance = null;

	// Use this for initialization
	void Start ()
    {
		if (!sfxSource) 
		{
			Debug.Log ("No sfx source");
		}

		if (!musicSource) 
		{
			Debug.Log ("No music source");
		}
	}


	void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (this);

		DontDestroyOnLoad (gameObject);
	}

		
	public bool SFX_PlayOneShot(AudioClip _clipToPlay, bool is3DSound)
	{
		if (_clipToPlay) 
		{
			sfxSource.PlayOneShot (_clipToPlay);
			return true;
		}

		return false;
	}

	public bool SFX_PlayOneShot(AudioClip _clipToPlay, float _volumeScale, bool is3DSound)
	{
		if (_clipToPlay) 
		{
			sfxSource.PlayOneShot (_clipToPlay, _volumeScale);
			return true;
		}

		return false;
	}

	public bool SFX_PlayOneShot(AudioClip[] _clipToPlay, bool is3DSound)
	{
		if (_clipToPlay.Length > 0) 
		{			
			int randSfx = Random.Range (0, _clipToPlay.Length);
			sfxSource.PlayOneShot (_clipToPlay [randSfx]);
			return true;
		}

		return false;
	}

	public bool SFX_PlayOneShot(AudioClip[] _clipToPlay, float _volumeScale, bool is3DSound)
	{
		if (_clipToPlay.Length > 0) 
		{			
			int randSfx = Random.Range (0, _clipToPlay.Length);
			sfxSource.PlayOneShot (_clipToPlay [randSfx], _volumeScale);
			return true;
		}

		return false;
	}

	//

	public bool SFX_Play(AudioClip _clipToPlay, bool is3DSound)
	{
		if (_clipToPlay) 
		{
			sfxSource.clip = _clipToPlay;
			sfxSource.Play ();
			return true;
		}

		return false;
	}

	public bool SFX_Play(AudioClip _clipToPlay, Vector3 _position ,bool is3DSound)
	{
		if (_clipToPlay) 
		{
			sfxSource.clip = _clipToPlay;
			AudioSource.PlayClipAtPoint (_clipToPlay,_position);
			return true;
		}

		return false;
	}

	public bool SFX_Play(AudioClip[] _clipToPlay, bool is3DSound)
	{
		if (_clipToPlay.Length > 0) 
		{			
			int randSfx = Random.Range (0, _clipToPlay.Length);
			sfxSource.clip = _clipToPlay [randSfx];
			sfxSource.Play ();
			return true;
		}

		return false;
	}
	
	// Update is called once per frame
	void Update ()
    {

	}
}
