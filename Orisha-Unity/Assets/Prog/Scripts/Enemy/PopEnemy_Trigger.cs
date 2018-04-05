using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class PopEnemy_Information
{    
    public string EnemyName;    
    public Vector3 enemyOrientation;    
    public int numberOfEnemy;
}

public class PopEnemy_Trigger : MonoBehaviour
{
    [SerializeField]
    private Pool EnemyPool;
    [SerializeField]
    private Vector3 whereTheEnemiesPop;

    [SerializeField]
    private float distanceBetweenEnemies;
    [SerializeField]
    PopEnemy_Information EnemyLeader;
    
    [SerializeField]
    List<PopEnemy_Information> Enemies;

    private int numberOfPlayerInTrigger = 0;
    [SerializeField]
    private bool isAbleToPopEnemy; // if the player has already enclenche the trigger, it wait the player go out of it to be able again

    private float timerCoolDown = 0.0f;
    [SerializeField]
    private float MaxTimerCoolDown = 10.0f;

    private void Start()
    {
        if(EnemyPool == null)
        {
            GameObject tempPoolGo = GameObject.Find("Enemy_Pool");
            if (tempPoolGo != null)
            {
                EnemyPool = tempPoolGo.GetComponent<Pool>();
            }
            else
            {
                Debug.LogError("Impossible to find EnnemyPool.");
            }
        }
    }

    private void Update()
    {
        if (numberOfPlayerInTrigger == 0 && !isAbleToPopEnemy)
        {
            timerCoolDown += Time.deltaTime;
            if (timerCoolDown >= MaxTimerCoolDown)
            {
                isAbleToPopEnemy = true;
            }
        }
        else
        {
            timerCoolDown = 0.0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            PopEnemyGroup();
            numberOfPlayerInTrigger++;
            isAbleToPopEnemy = false;
        }
        else if (other.transform.parent.tag == "Player")
        {
            PopEnemyGroup();
            numberOfPlayerInTrigger++;
            isAbleToPopEnemy = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            numberOfPlayerInTrigger--;
        }
        else if (other.transform.parent.tag == "Player")
        {
            numberOfPlayerInTrigger--;
        }

    }
    private void PopEnemyGroup()
    {
        // Leader
        EnemyPool.TakeInPool(EnemyLeader.EnemyName, whereTheEnemiesPop, EnemyLeader.enemyOrientation);

        int totalNumberOfGroup = 0;
        foreach (PopEnemy_Information eInfo in Enemies)
        {
            totalNumberOfGroup++;
        }        

        for (int j = 0; j < totalNumberOfGroup; j++)
        {
            for (int i = 1; i < Enemies[j].numberOfEnemy; i++)
            {
                Vector3 tempPos = new Vector3(whereTheEnemiesPop.x + (distanceBetweenEnemies * (j+1)), whereTheEnemiesPop.y, whereTheEnemiesPop.z);
                GameObject basicEnemy = EnemyPool.TakeInPool(Enemies[j].EnemyName, tempPos, Enemies[j].enemyOrientation);
                if (basicEnemy != null)
                {
                    basicEnemy.transform.RotateAround(whereTheEnemiesPop, Vector3.up, ((360.0f / ((float)Enemies[j].numberOfEnemy - 1)) * i) + (7 * j));
                }
            }
        }
    }

}
