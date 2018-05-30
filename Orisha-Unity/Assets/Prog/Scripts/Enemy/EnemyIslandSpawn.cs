using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Collider))]
public class EnemyIslandSpawn : MonoBehaviour {

	Collider trigger;
	[SerializeField] private AI_Enemy_Basic[] enemiesToSpawn;
	[SerializeField] private SandShaderPositionner[] enemiesShaders;
	[SerializeField] private float spawnSpeed = 2.0f;
	private bool areSpawning = false;
	private bool areDespawning = false;

	private float timer;
	private Vector3[] initPos;
	private Vector3[] finalPos;

	private Vector3[] initPosDespawn;
	private Vector3[] finalPosDespawn;

	void Start ()
	{
		enemiesShaders = new SandShaderPositionner[enemiesToSpawn.Length];

		for (int i = 0; i < enemiesShaders.Length; i++)
		{
			enemiesShaders[i] = enemiesToSpawn[i].GetComponentInChildren<SandShaderPositionner>();
			enemiesShaders[i].OnBegin();
		}

		trigger = GetComponent<Collider>();
		trigger.isTrigger = true;

		initPos = new Vector3[enemiesToSpawn.Length];
		finalPos = new Vector3[enemiesToSpawn.Length];

		initPosDespawn = new Vector3[enemiesToSpawn.Length];
		finalPosDespawn = new Vector3[enemiesToSpawn.Length];

		for (int i = 0; i < enemiesToSpawn.Length; i++)
		{
			enemiesToSpawn[i].gameObject.SetActive(false);

			initPos[i] = enemiesToSpawn[i].transform.position + Vector3.down * 3.0f;         
			finalPos[i] = enemiesToSpawn[i].transform.position;

			enemiesToSpawn[i].transform.position = initPos[i];
		}
	}


	void Update ()
	{
		if(areSpawning)
		{
			timer += Time.deltaTime * spawnSpeed;
			for(int i = 0; i < enemiesToSpawn.Length; i++)
			{
				enemiesToSpawn[i].transform.position = Vector3.Lerp(initPos[i], finalPos[i], timer);
			}

			if(timer > 1.0f)
			{
				EndSpawn();
			}
		}
		if(areDespawning)
		{
			timer -= Time.deltaTime * spawnSpeed;
			for(int i = 0; i < enemiesToSpawn.Length; i++)
			{
				enemiesToSpawn[i].transform.position = Vector3.Lerp(initPosDespawn[i], finalPosDespawn[i], timer);
			}

			if(timer < 0.0f)
			{
				EndDespawn();
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		StartCoroutine (EnterZone (other));
	}

	private void OnTriggerExit(Collider other)
	{
		StartCoroutine (ExitZone (other));
	}

	private IEnumerator EnterZone(Collider other)
	{
		do {
			yield return 0;
		} while(areDespawning);

			if (areSpawning == false && other.tag == "Player")
			{
				areSpawning = true;
				timer = -0.1f;
				for (int i = 0; i < enemiesToSpawn.Length; i++)
				{
					enemiesToSpawn [i].UnfreezeAnim (1.0f);
					enemiesToSpawn[i].gameObject.SetActive(true);
					enemiesToSpawn[i].enabled = false;
					enemiesToSpawn[i].GetComponent<NavMeshAgent>().enabled = false;
				}
			}
		yield break;
	}


	private IEnumerator ExitZone(Collider other)
	{
		do {
			yield return 0;
		} while(areSpawning);

		if (areDespawning == false && other.tag == "Player")
		{
			areDespawning = true;
			timer = 1.1f;
			for (int i = 0; i < enemiesToSpawn.Length; i++)
			{
				initPosDespawn[i] = enemiesToSpawn[i].transform.position + Vector3.down * 3.0f;         
				finalPosDespawn[i] = enemiesToSpawn[i].transform.position;

				enemiesToSpawn [i].FreezeAnim ();
				enemiesToSpawn[i].enabled = false;
				enemiesToSpawn[i].GetComponent<NavMeshAgent>().enabled = false;
			}
		}
		yield break;
	}


	private void EndSpawn()
	{
		for(int i = 0; i < enemiesToSpawn.Length; i++)
		{
			enemiesToSpawn[i].enabled = true;
			enemiesToSpawn[i].GetComponent<NavMeshAgent>().enabled = true;
		}
		areSpawning = false;
	}

	private void EndDespawn()
	{
		for(int i = 0; i < enemiesToSpawn.Length; i++)
		{
			enemiesToSpawn[i].gameObject.SetActive(false);
			enemiesToSpawn[i].enabled = true;
			enemiesToSpawn[i].GetComponent<NavMeshAgent>().enabled = true;
			enemiesToSpawn[i].transform.position = initPosDespawn[i];
		}
		areDespawning = false;
	}
}
