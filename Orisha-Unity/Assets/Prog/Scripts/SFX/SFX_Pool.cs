using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX_Pool : MonoBehaviour 
{
	static List<SFX_Object> objects;

	static SFX_Pool instance = null;
	public static SFX_Pool Instance
	{
		get
		{
			if (instance == null) 
			{
				GameObject go = new GameObject ();
				go.name = "SFX_Pool";
				instance = go.AddComponent (typeof(SFX_Pool)) as SFX_Pool;
				instance.InitPool ();
			}
			
			return instance;
		}
	}

	void Start ()
	{
		InitPool ();

		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);
	}

	void InitPool()
	{
		objects = new List<SFX_Object>();

		bool isDone = InitwithChilds ();
		if (!isDone) 
			InitByCreatingChilds (20);
	}

	bool InitwithChilds()
	{
		foreach (SFX_Object obj in GetComponentsInChildren<SFX_Object>(true))
		{
			objects.Add(obj);
		}

		return objects.Count != 0;
	}

	void InitByCreatingChilds(int childNumber)
	{
		for (int i = 0; i < childNumber; i++) 
		{
			if (SoundManager.instance.sfx_Object_Model) 
			{
				SFX_Object obj = Instantiate (SoundManager.instance.sfx_Object_Model).GetComponent<SFX_Object> ();
				obj.transform.parent = transform;
				obj.gameObject.SetActive (false);
				objects.Add (obj);
			}
		}
	}

	public SFX_Object GetSFXObject(Vector3 _position, AudioClip _clip)
	{
		SFX_Object toReturn = null;
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
		
	SFX_Object CreateSFX_Object(Vector3 _position)
	{
		SFX_Object go = Instantiate (objects[0].gameObject).GetComponent<SFX_Object>();
		go.transform.parent = objects [0].transform.parent;
		objects.Add (go);
		go.transform.position = _position;
		return go;
	}

	SFX_Object SelectSFX_Object(SFX_Object _sfx, Vector3 _position, AudioClip _clip)
	{
		if(_sfx.gameObject.activeInHierarchy == false)
		{
			ActivateSFX_object (_sfx, _position, _clip);
			return _sfx;
		}
		return null;
	}

	void ActivateSFX_object(SFX_Object _sfx, Vector3 _position, AudioClip _clip)
	{
		_sfx.transform.position = _position;
		if (_sfx.source)
			_sfx.source.clip = _clip;
		else 
			InitSource(_sfx, _clip);
		
		_sfx.gameObject.SetActive(true);
	}
		
	void InitSource(SFX_Object _sfx, AudioClip _clip)
	{
		_sfx.source = _sfx.GetComponent<AudioSource> ();
		_sfx.source.clip = _clip;
	}

	//Return the object to his pool by desabling it.
	public void ReturnToPool(SFX_Object _sfx)
	{
		//teste si l'objet appartient bien à la pool
		if (objects.Contains (_sfx))
			_sfx.gameObject.SetActive (false);
		else
			Debug.LogWarning ("SFX_Object doesn't match with pool objects");
	}
}
