using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeBar_Debug : MonoBehaviour {

	[SerializeField] GameObject child;

	// Use this for initialization
	void Start () {
		child.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.KeypadDivide)) {
			child.SetActive (!child.activeInHierarchy);
		}
	}
}
