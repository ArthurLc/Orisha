


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkDustPool : MonoBehaviour
{

    static List<GameObject> objects;
    [SerializeField] float spawnedObjectsLifespan = 10.0f;

    void Start ()
    {
        objects = new List<GameObject>();
        foreach (WalkDustObject obj in GetComponentsInChildren<WalkDustObject>(true))
        {
            obj.lifespan = spawnedObjectsLifespan;
            objects.Add(obj.gameObject);
        }
	}


    public static void SpawnObject(Vector3 _position, Quaternion _rotation)
    {
        foreach(GameObject obj in objects)
        {
            if(obj.activeInHierarchy == false)
            {
                obj.transform.position = _position;
                obj.transform.rotation = _rotation;
                obj.SetActive(true);
                return;
            }
        }
    }
}
