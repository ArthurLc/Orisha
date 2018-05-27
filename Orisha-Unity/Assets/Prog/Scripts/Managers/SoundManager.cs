using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/*
  @Gauthier Pierre
  @SoundManager.cs
  @03/05/2018
  @Le Script s'atache au gameObject Managers. Il gère la possibilitée de jouer des sons en 
  @2D simultanés ou non et en 3D en lui donnant une position en faisant appel à SFX_Pool pour placer des Audio sources dans le monde.
 */

public class SoundManager : MonoBehaviour
{
	//2D sounds will be played by sfxSource
    public AudioSource sfxSource;
	public GameObject sfx_Object_Model;
	public static SoundManager instance = null;

	// Use this for initialization
	void Start ()
    {
		if (!sfxSource) 
		{
			Debug.Log ("No sfx source");
		}
	}

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (this);

		DontDestroyOnLoad (gameObject);
	}

	/// <summary>
	/// Play a given clip once as a 2d sound.
	/// allow sound superposition.
	/// </summary>	
	public bool SFX_PlayOneShot(AudioClip _clipToPlay)
	{
		if (_clipToPlay) 
		{
			sfxSource.PlayOneShot (_clipToPlay);
			return true;
		}

		return false;
	}

	/// <summary>
	/// Play a given clip at a given volume once as a 2d sound.
	/// allow sound superposition.
	/// </summary>	
	public bool SFX_PlayOneShot(AudioClip _clipToPlay, float _volumeScale)
	{
		if (_clipToPlay) 
		{
			sfxSource.PlayOneShot (_clipToPlay, _volumeScale);
			return true;
		}

		return false;
	}

	/// <summary>
	/// Play randomly a clip, contained in a given array, once as a 2d sound.
	/// allow sound superposition.
	/// </summary>	
	public bool SFX_PlayOneShot(AudioClip[] _clipToPlay)
	{
		if (_clipToPlay.Length > 0) 
		{			
			int randSfx = Random.Range (0, _clipToPlay.Length);
			sfxSource.PlayOneShot (_clipToPlay [randSfx]);
			return true;
		}

		return false;
	}

	/// <summary>
	/// Play randomly a clip, contained in a given array, at a given volume once as a 2d sound.
	/// allow sound superposition.
	/// </summary>
	public bool SFX_PlayOneShot(AudioClip[] _clipToPlay, float _volumeScale)
	{
		if (_clipToPlay.Length > 0) 
		{			
			int randSfx = Random.Range (0, _clipToPlay.Length);
			sfxSource.PlayOneShot (_clipToPlay [randSfx], _volumeScale);
			return true;
		}

		return false;
	}

	/// <summary>
	/// Play a given clip once as a 2d sound.
	/// will not allow sound superposition.
	/// </summary>	
	public bool SFX_Play(AudioClip _clipToPlay)
	{
		if (_clipToPlay) 
		{
			sfxSource.clip = _clipToPlay;
            sfxSource.Play ();
			return true;
		}

		return false;
	}

	/// <summary>
	/// Play a clip, contained in a given array, once as a 2d sound.
	/// will not allow sound superposition.
	/// </summary>	
	public bool SFX_Play(AudioClip[] _clipToPlay)
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


	/// <summary>
	/// set an audio source that will play your clip at a given position.
	/// The audio source will be selected by SFX_Pool.
	/// </summary>
	/// <returns><c>true</c>, if clip was initialized, <c>false</c> otherwise.</returns>
	/// <param name="_clipToPlay">Clip to play.</param>
	/// <param name="_position">Position.</param>
	public bool SFX_PlayAtPosition(AudioClip _clipToPlay, Vector3 _position)
	{
		if (_clipToPlay) 
		{
            SFX_Object sfx = SFX_Pool.Instance.GetSFXObject ( _position,_clipToPlay);
            sfx.source.loop = false;
            //SFX_Pool.GetSFXObject ( _position,_clipToPlay);
            return true;
		}

		return false;
	}

    public bool SFX_PlayAtPosition(AudioClip _clipToPlay, Vector3 _position, float _minDistance, float _maxDistance)
    {
        if (_clipToPlay)
        {
            SFX_Object sfx =  SFX_Pool.Instance.GetSFXObject(_position, _clipToPlay);
            sfx.source.minDistance = _minDistance;
            sfx.source.maxDistance = _maxDistance;
            sfx.source.loop = false;
            return true;
        }

        return false;
    }

    public SFX_Object SFX_LoopAtPosition(AudioClip _clipToPlay, Vector3 _position)
    {
        if (_clipToPlay)
        {
            SFX_Object sfx = SFX_Pool.Instance.GetSFXObject(_position, _clipToPlay);
            sfx.source.loop = true;
            return sfx;
        }

        return null;
    }

    public SFX_Object SFX_LoopAtPosition(AudioClip _clipToPlay, Vector3 _position, float _minDistance, float _maxDistance)
    {
        if (_clipToPlay)
        {
            SFX_Object sfx = SFX_Pool.Instance.GetSFXObject(_position, _clipToPlay);
            sfx.source.loop = true;
            sfx.source.minDistance = _minDistance;
            sfx.source.maxDistance = _maxDistance;
            return sfx;
        }

        return null;
    }
}
