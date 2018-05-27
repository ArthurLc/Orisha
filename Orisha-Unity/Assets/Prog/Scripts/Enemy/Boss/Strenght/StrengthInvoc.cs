﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AI_Enemy_Basic))]
public class StrengthInvoc : MonoBehaviour
{
    [SerializeField] GameObject wavesPrefab;
    List<GameObject> waves;
    AI_Enemy_Basic me;

    [SerializeField] float invoqueFrequency = 20.0f;
    Vector3 startPos;
    GameObject currentWavesGo;
	// Use this for initialization
	void Start ()
    {
        me = GetComponent<AI_Enemy_Basic>();
        startPos = transform.position;

        currentWavesGo = Instantiate(wavesPrefab, startPos, Quaternion.Euler(0,0,0));

        waves = new List<GameObject>();

        InitWaves();

        CheckpointsManager.OnPlayerRespawn += Reset;
        StartCoroutine(Invoque(invoqueFrequency));
    }

    void InitWaves()
    {
        waves.Clear();
        for (int i = 0; i < wavesPrefab.transform.childCount; ++i)
        {
            GameObject go = currentWavesGo.transform.GetChild(i).gameObject;
            waves.Add(go);
            go.SetActive(false);
        }
    }

    public void SpawnEnemies()
    {
        waves[0].SetActive(true);
        waves.Remove(waves[0]);
    }

    IEnumerator Invoque(float _frequency)
    {
        while(me.Health > 0 && waves.Count > 0 && me.myState != AI_Enemy_Basic.State.Die)
        {
            if (me.CurrentTarget)
            {
                me.CallEnemies();
            }
            yield return new WaitForSeconds(_frequency);
        };
    }

    private void Reset()
    {
        if(me.Health > 0)
        {
            StopCoroutine(Invoque(invoqueFrequency));
            me.Reset();

            DestroyImmediate(currentWavesGo);
            currentWavesGo = Instantiate(wavesPrefab, startPos, Quaternion.Euler(0, 0, 0));
            InitWaves();

            StartCoroutine(Invoque(invoqueFrequency));
        }
    }
}
