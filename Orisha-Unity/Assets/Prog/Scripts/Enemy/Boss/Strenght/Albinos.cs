using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AI_Enemy_Basic))]
public class Albinos : MonoBehaviour
{
    [SerializeField] GameObject wavesPrefab;
    List<GameObject> waves;
    int waveNumber;
    AI_Enemy_Basic me;

    [SerializeField] float invoqueFrequency = 20.0f;
    Vector3 startPos;
    GameObject currentWavesGo;

    float lastLifePercentage;
    float nextPercentSpawn;
    float percentLossPerSpawn;
    int waveId;

    bool wantToInvoc = false;
    [SerializeField] Transform backUpPos;

    AI_Enemy_Basic ally;
	// Use this for initialization
	void Start ()
    {
        me = GetComponent<AI_Enemy_Basic>();
        startPos = transform.position;

        currentWavesGo = Instantiate(wavesPrefab, startPos, Quaternion.Euler(0,0,0));

        waves = new List<GameObject>();

        InitWaves();

        CheckpointsManager.OnPlayerRespawn += Reset;
    }

    void InitWaves()
    {
        lastLifePercentage = 100;

        waveNumber = wavesPrefab.transform.childCount;
        percentLossPerSpawn = (lastLifePercentage / (waveNumber + 1));
        nextPercentSpawn = lastLifePercentage - percentLossPerSpawn;
        waveId = 0;

        Debug.Log(lastLifePercentage + " / " + percentLossPerSpawn + " / " + nextPercentSpawn + " / " + waveNumber);

        waves.Clear();
        for (int i = 0; i < waveNumber; ++i)
        {
            GameObject go = currentWavesGo.transform.GetChild(i).gameObject;
            waves.Add(go);
            go.SetActive(false);
        }
    }

    void GoToBackUpPos()
    {
        Debug.Log("Vas là bas !");
        me.HardFreezeStates();
        me.GoTo(backUpPos);
        me.IsInvincible = true;
        wantToInvoc = true;
    }

    private void Update()
    {
        //si la vie a baissé
        if(lastLifePercentage > CurrentHealthToPercent())
        {
            //si la vie vient de passer sous le seuil d'invoc
            if(waveId < waveNumber && lastLifePercentage > nextPercentSpawn && CurrentHealthToPercent() < nextPercentSpawn)
            {
                Debug.Log(waveId + " / " + waveNumber);
                //donne boss pile la vie pour son invoc
                me.Health = (nextPercentSpawn / 100) * me.BaseHealth;
                //demande à l'ennemi d'aller ) la position de BackUp
                GoToBackUpPos();

                nextPercentSpawn -= percentLossPerSpawn;
                waveId++;
            }
            Debug.Log("La vie a baissé");
        }


        if(wantToInvoc && Vector3.Distance(me.transform.position,me.MyAgent.destination) < 1f)
        {
            me.CallEnemies();
            wantToInvoc = false;
            me.transform.LookAt(me.CurrentTarget);
        }

        if(ally && ally.Health <= 0)
        {
            me.HardUnfreezeStates();
            me.IsInvincible = false;
            ally = null;
        }

        lastLifePercentage = CurrentHealthToPercent();
    }

    float CurrentHealthToPercent()
    {
        return (me.Health / me.BaseHealth) * 100;
    }

    public void SpawnEnemies()
    {
        waves[0].SetActive(true);
        ally = waves[0].transform.GetChild(1).GetChild(0).GetComponent<AI_Enemy_Basic>();
        waves.Remove(waves[0]);
    }

    private void Reset()
    {
        if(me.Health > 0)
        {
            me.Reset();
            DestroyImmediate(currentWavesGo);
            currentWavesGo = Instantiate(wavesPrefab, startPos, Quaternion.Euler(0, 0, 0));
            InitWaves();
        }
    }
}
