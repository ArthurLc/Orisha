using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using vd_Player;

public class AerialBugDetection : MonoBehaviour 
{
	Vector3 basePos;
	float timer = 0.0f;
	[SerializeField]float debugTime = 2.0f;
	PlayerController pc;

	// Use this for initialization
	void Start () 
	{
		timer = debugTime;
		basePos = transform.position;
		pc = GetComponentInParent<PlayerController> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!pc.IsGrounded) 
		{
			if (transform.position.y <= basePos.y - 0.01f) 
			{
				timer = debugTime;
				basePos = transform.position;
			} 
			else 
			{
				timer -= Time.deltaTime;
				//Debug.Log ("Timer in action");
				if (timer <= 0) 
				{
					CheckpointsManager.RepopPlayerToCloserCheckpoint ();
					//Debug.Log ("Get In Place");
					timer = debugTime;
				}
			}
		}
		else 
		{
			basePos = transform.position;
			timer = debugTime;
		}
	}
}
