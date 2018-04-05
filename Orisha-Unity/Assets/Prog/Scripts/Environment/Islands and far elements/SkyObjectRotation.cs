using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyObjectRotation : MonoBehaviour
{
    public float radius;
    public float speed;
    public Vector3 speedOnItSelf;
    public Vector3 Origin;

    private void Start()
    {
        speedOnItSelf = new Vector3(Random.Range(-100.0f, 100.0f), Random.Range(-100.0f, 100.0f), Random.Range(-100.0f, 100.0f));
        Origin = new Vector3(Origin.x, transform.position.y, Origin.z);
        if (Random.value < 0.5f)
            speed = -speed;
    }
    void FixedUpdate ()
    {
        transform.RotateAround(Origin, Vector3.up, speed * 0.01f);

    }
}
