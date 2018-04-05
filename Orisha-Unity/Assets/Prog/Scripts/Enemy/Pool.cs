/*
*@ Creator: Julien Lopez
*@ Worked on it: Julien Lopez, Romain Seurot, 
*@ Enemy Pool
*@ Date: 18/10/2017
*@ Description: This script contain a list of pool (enemy Type)               
*@ Last Modification: 25/10/2017
*@ By: Romain Seurot
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Definition structure d'une pool
[System.Serializable]
public class sPool
{
    public string szNamePool;   //Nom pour choisir la pool au moment de l'appel
    public GameObject[] gObjectsInPool; //Liste des go dans la pool
    public GameObject[] gObjectsInGame; //Liste des go utilise en jeu
    public Vector3 vPositionPool; //Position de retour dans la pool
}

public class Pool : MonoBehaviour {

    public List<sPool> Pools;   //Liste des pools à édité dans l'éditeur

    private void Awake()
    {
        foreach(sPool _tempPool in Pools)
        {
            for (int i = 0; i < _tempPool.gObjectsInPool.Length; i++)
            {
                _tempPool.gObjectsInPool[i].transform.position = _tempPool.vPositionPool;
            }
        }
    }

    void Update () {
        
        
    }


    /// <summary>
    /// Permet d'appeller un go dans une pool avec son nom (_szName) , à une position définit (_vDestination), et une direction (_vOrientation) 
    /// </summary>
    /// <param name="_szName"> Nom de la pool à piocher </param>
    /// <param name="_vDestination"> vecteur de position de la poule </param>
    /// <param name="_vOrientation"> vecteur directeur de la poule </param>
    /// <returns></returns>
    public GameObject TakeInPool(string _szName, Vector3 _vDestination, Vector3 _vOrientation)
    {
        foreach (sPool pool in Pools)
        {
            if((pool.szNamePool == _szName)&&(pool.gObjectsInPool != null)) //Test le nom de la pool et si un go est dispo
            {
                // Récupère et échange de la pool dans la liste (gOjectsInPool => gObjectsInGame) d'un go
                GameObject goTemp;
                int iIndex = GetCase(pool.gObjectsInPool, true);
                goTemp = pool.gObjectsInPool[iIndex];
                pool.gObjectsInGame[GetCase(pool.gObjectsInGame, false)] = goTemp;
                pool.gObjectsInPool[iIndex] = null;

                if (goTemp != null)
                {
                    // Set sa nouvelle position et direction
                    goTemp.transform.position = _vDestination;
                    goTemp.transform.forward = _vOrientation;
                    // Le retourne pour une utilisation future
                    return goTemp;
                }

            }
        }

        Debug.Log("Fail to take in pool");
        return null;
    }


    /// <summary>
    /// Update des gos in game, test s'ils sont encore actif si non les renvoit dans leur pools
    /// </summary>
    /// <param name="_szName"> Nom de la pool à remplir </param>
    /// <param name="_go"> gameObject à ranger </param>
    public void BackToPool(string _szName, GameObject _go)
    {
        foreach (sPool pool in Pools)
        {
            if ((pool.szNamePool == _szName)&&(_go != null)) //Test le nom de la pool
            {

                // Récupère et échange de la liste dans la pool (gOjectsInGame => gObjectsInPool) d'un go
                for (int i = 0; i < pool.gObjectsInGame.Length; i++)
                {
                    if(pool.gObjectsInGame[i] == _go)
                    {
                        pool.gObjectsInPool[GetCase(pool.gObjectsInPool, false)] = _go;
                        pool.gObjectsInGame[i] = null;
                    }
                }
                
                // Le repositionne à sa pool et le réactive
                _go.transform.position = pool.vPositionPool;
                _go.SetActive(true);

            }
            else
            {
                Debug.Log("Erreur : Pool ou Poule inexistante !");
            }
        }
    }

    /// <summary>
    /// Permet de trouver dans une liste de GameObject une emplacement vide ou un plein
    /// </summary>
    /// <param name="_gObjects"> Tableau dans lequel on recherche une case </param>
    /// <param name="_bExist"> true = case avec objet / false = case vide </param>
    /// <returns></returns>
    private int GetCase(GameObject[] _gObjects , bool _bExist)
    {
        for (int i = 0; i < _gObjects.Length; i++)
        {
            if (_bExist)
            {
                if (_gObjects[i] != null)
                {
                    return i;
                }
            }
            else
            {
                if (_gObjects[i] == null)
                {
                    return i;
                }
            }
        }
        return _gObjects.Length - 1;
    }

   
}
