using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using vd_Player;

[RequireComponent(typeof(AI_Enemy_Basic))]
public class StrengthInvoc : MonoBehaviour
{
    [SerializeField] GameObject wavesPrefab;
    List<GameObject> waves;
    AI_Enemy_Basic me;

    [SerializeField] float invoqueFrequency = 20.0f;
    Vector3 startPos;
    GameObject currentWavesGo;

    [SerializeField] float roarRange = 10;
    [SerializeField] int roarDamages = 10;

    int waveNumber;
    float lastLifePercentage;
    float nextPercentSpawn;
    float percentLossPerSpawn;
    int waveId;
    bool wantToInvoc = false;

    [SerializeField] LayerMask maskPlayer;
    [SerializeField] ParticleSystem shield;

    // Use this for initialization
    void Start ()
    {
        me = GetComponent<AI_Enemy_Basic>();
        startPos = transform.position;

        if(wavesPrefab)
            currentWavesGo = Instantiate(wavesPrefab, startPos, Quaternion.Euler(0,0,0));

        waves = new List<GameObject>();

        if(wavesPrefab)
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

        //Debug.Log(lastLifePercentage + " / " + percentLossPerSpawn + " / " + nextPercentSpawn + " / " + waveNumber);

        waves.Clear();
        for (int i = 0; i < waveNumber; ++i)
        {
            GameObject go = currentWavesGo.transform.GetChild(i).gameObject;
            waves.Add(go);
            go.SetActive(false);
        }
    }

    private void Update()
    {
        //si la vie a baissé
        if (lastLifePercentage > CurrentHealthToPercent())
        {
            //si la vie vient de passer sous le seuil d'invoc
            if (waveId < waveNumber && lastLifePercentage > nextPercentSpawn && CurrentHealthToPercent() <= nextPercentSpawn)
            {
                //donne boss pile la vie pour son invoc
                me.Health = (nextPercentSpawn / 100) * me.BaseHealth;
                nextPercentSpawn -= percentLossPerSpawn;
                //Debug.Log(waveId + " / " + waveNumber + " / " + nextPercentSpawn);
                waveId++;               

                me.HardFreezeStates();
                me.CallEnemies();
                me.transform.LookAt(me.CurrentTarget);
            }
        }

        lastLifePercentage = CurrentHealthToPercent();
    }
    
    float CurrentHealthToPercent()
    {
        return (me.Health / me.BaseHealth) * 100;
    }

    public void SpawnEnemies()
    {
        if(waves != null)
        {
            waves[0].SetActive(true);
            waves.Remove(waves[0]);

            Collider[] cols = Physics.OverlapSphere(me.transform.position, roarRange, maskPlayer);

            if (cols.Length > 0)
            {
                for (int i = 0; i < cols.Length; ++i)
                {
                    PlayerController pc = cols[i].transform.parent.GetComponent<PlayerController>();
                    if (pc)
                    {
                        pc.Ci.RoarDamages(roarDamages, transform.position);
                    }
                }                
            }

        }     
        me.HardUnfreezeStates();
    }

    private void Reset()
    {
        if(me.Health > 0)
        {
            me.Reset();
            me.MyAgent.SetDestination(startPos);
            DestroyImmediate(currentWavesGo);
            currentWavesGo = Instantiate(wavesPrefab, startPos, Quaternion.Euler(0, 0, 0));
            InitWaves();
        }
    }
}
