/*
* @PierreGauthier
* @PlayerInformations.cs
* @22/11/2017
* @Le script s'attache là où se trouve l'animator du player
*   - Le script contient les informations du joueur
*   - l'init est appelé dans Character initialisation
*   - a rajouter toutes les infos utiles au joueur ou a transmettre à l'animator
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInformations : MonoBehaviour
{
    int health;
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public bool TakeDamages(int _damages)
    {
        if (_damages > 0)
        {
            health -= _damages;
            return true;
        }
        else
            return false;
    }

    public void Init(int life)
    {
        health = life;
    }

}
