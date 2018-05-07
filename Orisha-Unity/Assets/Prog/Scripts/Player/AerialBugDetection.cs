using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using vd_Player;

public class AerialBugDetection : MonoBehaviour 
{
	Vector3 basePos;
	float debugTime = 5.0f;
	PlayerController pc;

	// Use this for initialization
	void Start () 
	{
		basePos = transform.position;
		pc = GetComponentInParent<PlayerController> ();
	}

	public void CheckIfInBBug()
	{
		StartCoroutine (Check());
	}

	IEnumerator Check()
	{
		basePos = transform.position;
		if (!pc.IsGrounded) 
			yield return new WaitForSeconds (debugTime);
		else 
			yield break;

		if (!pc.IsGrounded) 
		{
			if (transform.position.y <= basePos.y - 0.025f) 
				yield break; 
			else 
				CheckpointsManager.RepopPlayerToCloserCheckpoint ();
		}
		else 
			yield break;
	}
}
