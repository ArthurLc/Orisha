using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingPartDoor : MonoBehaviour {

	[Range(0.0f, 10.0f)] public float valueDisplacement;
	[Range(0.0f, 1.0f)] public float speedDisplacement;
	private float timerDisplacement;

	private void Start()
	{
		timerDisplacement = Random.Range(Mathf.PI * (-2.0f),Mathf.PI * 2.0f);
	}

	public void Update()
	{
		timerDisplacement += Time.deltaTime * speedDisplacement;
		if(timerDisplacement > Mathf.PI * 2.0f)
			timerDisplacement = Mathf.PI * (-2.0f);

		transform.position = (new Vector3(Mathf.Cos(timerDisplacement), Mathf.Sin(timerDisplacement), 0.0f) * valueDisplacement) + transform.parent.position;
	}
}
