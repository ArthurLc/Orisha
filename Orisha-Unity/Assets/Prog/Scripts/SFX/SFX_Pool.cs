using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX_Pool : MonoBehaviour 
{
	static List<SFX_Object> objects;

	void Start ()
	{
		objects = new List<SFX_Object>();
		foreach (SFX_Object obj in GetComponentsInChildren<SFX_Object>(true))
		{
			objects.Add(obj);
			Debug.Log ("one more");
		}
	}

	/// <summary>
	/// Get an sfx.
	/// </summary>
	/// <param name="_position">Position.</param>
	/// <param name="_clip">Clip.</param>
	public static SFX_Object GetSFXObject(Vector3 _position, AudioClip _clip)
	{
		SFX_Object toReturn = null;
		bool isOnePickable = false;
		foreach (SFX_Object sfx in objects) 
		{
			toReturn = SelectSFX_Object (sfx, _position, _clip);
			if(toReturn)
				return toReturn;
		}
		
		if (!toReturn) 
		{
			toReturn = CreateSFX_Object (_position);
			return toReturn;
		}
		return null;
	}

	static SFX_Object CreateSFX_Object(Vector3 _position)
	{
		SFX_Object go = Instantiate (objects[0].gameObject).GetComponent<SFX_Object>();
		go.transform.parent = objects [0].transform.parent;
		objects.Add (go);
		go.transform.position = _position;
		return go;
	}

	static SFX_Object SelectSFX_Object(SFX_Object _sfx, Vector3 _position, AudioClip _clip)
	{
		if(_sfx.gameObject.activeInHierarchy == false)
		{
			ActivateSFX_object (_sfx, _position, _clip);
			return _sfx;
		}
		return null;
	}

	static void ActivateSFX_object(SFX_Object _sfx, Vector3 _position, AudioClip _clip)
	{
		_sfx.transform.position = _position;
		if (_sfx.source)
			_sfx.source.clip = _clip;
		else 
			InitSource(_sfx, _clip);
		
		_sfx.gameObject.SetActive(true);
	}

	static void InitSource(SFX_Object _sfx, AudioClip _clip)
	{
		_sfx.source = _sfx.GetComponent<AudioSource> ();
		_sfx.source.clip = _clip;
	}

	public static void ReturnToPool(GameObject _obj)
	{
		_obj.SetActive (false);
	}
}
