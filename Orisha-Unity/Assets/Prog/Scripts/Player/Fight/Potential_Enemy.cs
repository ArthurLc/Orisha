using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potential_Enemy : MonoBehaviour
{
    List<AI_Enemy_Basic> enemy_List;

    public delegate void OnFightEnter(bool enter);
    public static event OnFightEnter enteringFight;

    private static bool isOnFight = false;
    public static bool IsOnFight
    {
        get { return isOnFight; }
        set
        {
            isOnFight = value;
            enteringFight(isOnFight);
        }
    }

    private void Start()
    {
        enemy_List = new List<AI_Enemy_Basic>();
    }

    public void Add_Potential_Ennemy(AI_Enemy_Basic to_Add)
    {
        if(!enemy_List.Contains(to_Add))
        {
            enemy_List.Add(to_Add);

            if (!IsOnFight)
                IsOnFight = true;
        }
    }

    public void Pop_Potential_Ennemy(AI_Enemy_Basic to_Pop)
    {
        if (enemy_List.Contains(to_Pop))
        {
            enemy_List.Remove(to_Pop);

            if (enemy_List.Count == 0 && IsOnFight)
            {
                IsOnFight = false;
                //Debug.Log("Je suis plus en combat");
            }
        }
    }
}
