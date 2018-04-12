/*
* @ArthurLacour
* @Checkpoint.cs
* @12/04/2018
* @ - Le Script s'attache au prefab parent du checkpoint.
*   
* Contient :
*   - Le nom du checkpoint. (De la zone ?)
* 
* A faire:
*/

using UnityEngine;

public class Checkpoint : MonoBehaviour {
    [SerializeField] private string checkpointName = "NoName";
    public string GetCheckpointName
    {
        get { return checkpointName; }
    }
}
