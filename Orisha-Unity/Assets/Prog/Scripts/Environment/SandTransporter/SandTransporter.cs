using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandTransporter : MonoBehaviour
{
	[SerializeField] float GlobalDuration = 10.0f;
	private bool isMoving = false;
	private bool isAtStart = true;

	private vd_Player.CharacterInitialization Ci;
	private Rigidbody playerRB;

	public AnimationCurve curveMoveY;
	public Transform StartPoint;
	public Transform EndPoint;

	private float interpo = 0.0f;

	private bool needToLeave = false;
	private BoxCollider bc;

	private void Start()
	{
		bc = GetComponent<BoxCollider>();
		for (int i = 0; i < curveMoveY.length; i++) {
			curveMoveY.SmoothTangents (i, 1.1f);
		}
	}

	private void Update(){
		if (!isAtStart && !isMoving && !needToLeave)
        {
            if (Vector3.Distance(Ci.PlayerTr.position, transform.position) <= 0.25f)
            {
				StartPoint.parent = null;
				EndPoint.parent = null;
                Ci.transform.parent = transform;
                isMoving = true;
                Ci.PlayerController.isPlayingCinematic = false;
                playerRB.isKinematic = true;
                Ci.Anim.SetFloat("Speed", 0.0f);
                Ci.PlayerTr.position = transform.position;
                Ci.PlayerTr.localRotation = Quaternion.Euler(0, Ci.PlayerTr.localRotation.eulerAngles.y, 0);
                isAtStart = true;
            }
            else
            {
                playerRB.isKinematic = (Ci.PlayerTr.position.y <= transform.position.y);
                Ci.PlayerController.isPlayingCinematic = true;
                Ci.PlayerTr.LookAt(transform);
                Ci.Anim.SetFloat("Speed", 0.5f);
            }
        }

		if (playerRB && needToLeave) 
		{
			Vector3 pos2D, playerPos2D;
			pos2D = transform.position; 
			playerPos2D = Ci.PlayerTr.position;
			pos2D.y = playerPos2D.y = 0.0f;
			if (Vector3.Distance (playerPos2D, pos2D) >= bc.size.x + 0.5f) 
			{
				needToLeave = false;
				//Debug.Log("tu peut reprendre la plateforme !");	
			}
		}
			

		MoveTransporter ();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (Ci == null)
			Ci = other.GetComponentInParent<vd_Player.CharacterInitialization>();
        if (playerRB == null)
            playerRB = Ci.Rb;

		if (isAtStart && !needToLeave)
		{
			isAtStart = false;
            Ci.FreezeInputs();
            playerRB.isKinematic = true;
            Ci.PlayerTr.LookAt(transform);
            Ci.PlayerController.isPlayingCinematic = true;
            Ci.Anim.SetFloat("Speed", 0.5f);
        }
	}

	private void MoveTransporter()
	{
        if (isMoving)
        {
			interpo += Time.deltaTime;

            Ci.PlayerTr.position = transform.position;
            Ci.PlayerTr.localRotation = Quaternion.Euler(0, Ci.PlayerTr.localRotation.eulerAngles.y, 0);
            Ci.Anim.SetFloat("Speed", 0.0f);

			if (interpo < GlobalDuration)
            {
				transform.position = Vector3.Lerp(StartPoint.position, EndPoint.position, (interpo / GlobalDuration));
				transform.position = new Vector3 (transform.position.x, transform.position.y * curveMoveY.Evaluate (interpo / GlobalDuration)
												 , transform.position.z);
				//Debug.Log(transform.position + " | " + interpo);

			}
            else
            {

				transform.position = EndPoint.position;
                
				//Debug.Log("End : " + transform.position + " | " + interpo);

				//Arrivé
				interpo = 0.0f;
				isMoving = false;
				Transform tmpTr = StartPoint;
				StartPoint = EndPoint;
				EndPoint = tmpTr;

				StartPoint.parent = transform;
				EndPoint.parent = transform;

				Keyframe[] keys = curveMoveY.keys;
				int keyCount = keys.Length;
				WrapMode postWrapmode = curveMoveY.postWrapMode;
				curveMoveY.postWrapMode = curveMoveY.preWrapMode;
				curveMoveY.preWrapMode = postWrapmode;
				for(int i = 0; i < keyCount; i++ )
				{
					Keyframe K = keys[i];
					K.time = 1.0f - K.time;
					float tmp = -K.inTangent;
					K.inTangent = -K.outTangent;
					K.outTangent = tmp;
					keys[i] = K;
				}

				curveMoveY.keys = keys;

				//Debug.Log ("Tu ne peut pas prendre la platerforme");

				needToLeave = true;
            }

            
        }
        else if (Ci != null && playerRB != null)
        {
            if (Vector3.Distance(Ci.PlayerTr.position, transform.position) <= 0.1f)
            {
                Ci.transform.parent = null;
                Ci.UnfreezeInputs();
                playerRB.isKinematic = false;
            }
        }
	}
}
