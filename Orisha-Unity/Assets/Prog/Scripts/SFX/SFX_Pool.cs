using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX_Pool : MonoBehaviour 
{
	static List<GameObject> objects;
	[SerializeField] float spawnedObjectsLifespan = 10.0f;

	void Start ()
	{
		objects = new List<GameObject>();
		foreach (SFX_Object obj in GetComponentsInChildren<SFX_Object>(true))
		{
			objects.Add(obj.gameObject);
		}
	}


	public static void GetSFXObject(Vector3 _position)
	{
		bool isOnePickable = false;
		foreach(GameObject obj in objects)
		{
			if(obj.activeInHierarchy == false)
			{
				obj.transform.position = _position;
				obj.SetActive(true);
				isOnePickable = true;
				return;
			}
		}

		//Instancier un nouveau 
	}

}
