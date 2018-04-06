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

	private Transform parentListPoint;
	private List<Transform> listTransform = new List<Transform>();

	private int nbrPoint = 1;
	private float interpo = 0.0f;
	private float travelDuration;

	private void Start()
	{
		parentListPoint = transform.GetChild(1);
		for (int i = 0; i < parentListPoint.childCount; i++) {
			listTransform.Add (parentListPoint.GetChild (i));
		}
	}

	private void Update(){
		MoveTransporter (GlobalDuration);
	}

	private void OnTriggerStay(Collider other)
	{
		if (Ci == null)
			Ci = other.GetComponentInParent<vd_Player.CharacterInitialization>();
		if (playerRB == null)
			playerRB = other.GetComponentInChildren<Rigidbody>();


		if (isAtStart)
		{
			parentListPoint.parent = null;
			Ci.transform.parent = transform;
			Ci.FreezeInputs();
			playerRB.isKinematic = true;
			isMoving = true;
			isAtStart = false;
		}
	}

	private void OnCollisionExit(Collision collisionInfo)
	{
		isAtStart = true;
	}

	private void MoveTransporter(float duration)
	{
		
		if (isMoving) {
			travelDuration = duration / (float)listTransform.Count;

			interpo += Time.deltaTime;

			if (interpo < travelDuration) {
				transform.position = Vector3.Lerp (listTransform [nbrPoint - 1].position, listTransform [nbrPoint].position, interpo / travelDuration);
				transform.rotation = Quaternion.Lerp (listTransform [nbrPoint - 1].rotation, listTransform [nbrPoint].rotation, interpo / travelDuration);

			} else {

				transform.position = listTransform[nbrPoint].position;
				transform.rotation = listTransform[nbrPoint].rotation;

				++nbrPoint;

				Debug.Log (nbrPoint + " | " + interpo);
				interpo = 0.0f;
			}

			if (nbrPoint >= listTransform.Count) {
				interpo = 0.0f;
				nbrPoint = 1;
				isMoving = false;
	
				listTransform.Reverse ();
			}
		} else {
			if (Ci != null && playerRB != null) {
				Ci.transform.parent = null;
				parentListPoint.parent = transform;
				Ci.UnfreezeInputs ();
				playerRB.isKinematic = false;
			}
		}
	}
}
