/*
* @ArthurLacour
* @CheckpointsManager.cs
* @12/04/2018
* @ - Le Script est un static qui contient les checkpoints register par le joueur.
*   
* Contient :
*   - La liste de tous les checkpoints enregistré par le joueur.
*   - Les fonctions à appeler pour repop le Player.
* 
* A faire:
*/

using System.Collections.Generic;
using UnityEngine;
using vd_Player;

public static class CheckpointsManager {
    public static List<Checkpoint> checkpointList = new List<Checkpoint>(); //Checkpoints enregistrés par le joueur.

    private static CharacterInitialization player;
    public static CharacterInitialization PlayerRef
    {
        set { player = value; }
    }

    public static void RepopPlayerToLastCheckpoint()
    {
        if(checkpointList.Count == 0)
            player.RepopPlayerAtTransform(player.StartingTr);
        else
            player.RepopPlayerAtTransform(checkpointList[checkpointList.Count - 1].transform);
    }
    public static void RepopPlayerToCloserCheckpoint()
    {
        Transform closerCheckpoint = null;
        for(int i = 0; i < checkpointList.Count; i++) {
            if (closerCheckpoint == null)
                closerCheckpoint = checkpointList[i].transform;
            else
            {
                closerCheckpoint = Vector3.Distance(player.PlayerTr.position, closerCheckpoint.position) <
                    Vector3.Distance(player.PlayerTr.position, checkpointList[i].transform.position) ?
                        closerCheckpoint : checkpointList[i].transform;
            }
        }

        player.RepopPlayerAtTransform(closerCheckpoint != null ? closerCheckpoint : player.StartingTr);
    }
}
