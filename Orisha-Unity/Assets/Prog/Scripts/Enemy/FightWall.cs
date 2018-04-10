using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
* @JulienLopez
* @FightWall.cs
* @10/04/2018
* @Le script s'attache sur l'objet qui possède le particle system representant le wall
*   - penser a bien choisir les endroit où poser la zone
* 
*/
public class FightWall : MonoBehaviour {

	private BoxCollider boxCol;
	//private SphereCollider sphereTrigger;
	private Transform Player;
	private ParticleSystem particleSys;

	[SerializeField, Range(10.0f,100.0f)] private float sizeWall = 20.0f;

	public bool wallIsActivaded = false;
	private bool AnyoneAlive = true;
	private float timer = 0.0f;

	// Use this for initialization
	void Start () {
		boxCol = GetComponent<BoxCollider> ();
		//sphereTrigger = GetComponent<SphereCollider> ();
		Player = FindObjectOfType<vd_Player.CharacterInitialization>().PlayerTr;
		particleSys = GetComponent<ParticleSystem>();

		//Set the size of the donuts
		ParticleSystem.ShapeModule shape = particleSys.shape;
		shape.donutRadius = sizeWall;

		//sphereTrigger.radius = sizeWall - (sizeWall / sizeWall);
	}
	
	// Update is called once per frame
	void Update () {
		if(Potential_Enemy.IsOnFight)//Detection d'un enemy et activation de la zone
			wallIsActivaded = true;

		//Gestion de la zone
		if (wallIsActivaded) {
			if (!particleSys.isPlaying)
				particleSys.Play();
			if (!boxCol.enabled) {
				boxCol.enabled = true;
			}

			Vector3 dirBox = Player.position - transform.position;
			dirBox.y = 0.0f;
			boxCol.center = dirBox.normalized * sizeWall;
		} else {
			if (particleSys.isPlaying)
				particleSys.Stop();
			if (boxCol.enabled)
				boxCol.enabled = false;
		}

		//Test de désactivation de la zone
		timer += Time.deltaTime;
		if (timer >= 1.0f) {
			timer = 0.0f;
			Collider[] cols = Physics.OverlapSphere (transform.position, sizeWall);
			if (cols != null) {
				AnyoneAlive = false;
				foreach (Collider col in cols) {
					if (col.tag == "Enemy" && col.GetComponentInParent<AI_Enemy_Basic>().myState != AI_Enemy_Basic.State.Die) {
						AnyoneAlive = true;
					}
				}
			} else {
				wallIsActivaded = false;
				AnyoneAlive = false;
			}
		}

		//S'il n'y a plus d'ennemies desactive la zone
		if (!AnyoneAlive)
			wallIsActivaded = false;
	}

}
