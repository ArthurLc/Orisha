
/*
* @JulienLopez
* @OpenDoor.cs
* @29/03/2018
* @ - Le Script s'attache à une Porte
*   
* A l'entrée dans le trigger :
*   - Récupère la liste des masks sur le joueur
*   - Lance les mask sur la porte
*   - Bloque les déplacement du joueur
* 
* En fin de timer:
*   - La liste des masks du joueur est reset
*   - La porte est ouverte
*   - Trigger désactivé
*   
* A FAIRE :
*   - Modifier le déplacement de la porte pour coller au style de porte voulu
*   - Déplacer les point d'ancrage des masks dans la porte
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour {

    public int nbrMaskToOpen;
    public float durationFreeze;

    private float lerpValueMask = 0.0f;
    private bool doorActivaded = false;
    private bool isTryOpenning = false;


    private MaskInventory maskInventory;
    private Transform playerPos;
    private Transform StockMask;
    private List<Transform> listDoorMask = new List<Transform>();

	// Use this for initialization
	void Start () {
        for (int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).name == "StockMask")
            {
                StockMask = transform.GetChild(i);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {

        if (maskInventory == null)
            return;


        Debug.Log("up : " + maskInventory.ListMasks.Count);

        if (isTryOpenning && maskInventory.ListMasks.Count >= nbrMaskToOpen)
        {
            lerpValueMask += Time.deltaTime * 0.5f;
            durationFreeze -= Time.deltaTime;

            if (listDoorMask.Count == 0)
                return;

            //Boucle sur les spot de mask (lerp pos/scale/rot)
            for (int i = 0; i < transform.childCount - 1; i++)
            {
                listDoorMask[i].position = Vector3.Lerp(playerPos.position, transform.GetChild(i).position, lerpValueMask);
                listDoorMask[i].localScale = Vector3.Lerp(new Vector3(0.01f, 0.01f, 0.01f), new Vector3(1.0f, 0.4f, 0.4f), lerpValueMask);
                listDoorMask[i].Rotate(Vector3.up, 20.0f);
            }

            //Fin du lerp reset rotation , list player, et déclanche la porte
            if (lerpValueMask >= 1.0f)
            {
                for (int i = 0; i < transform.childCount - 1; i++)
                {
                    listDoorMask[i].rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
                }
                maskInventory.ResetListMask();
                doorActivaded = true;
            }
        }

        //move the door
        if(doorActivaded)
        {
            transform.position += (Vector3.up * Time.deltaTime);
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        isTryOpenning = true;
        if(other.tag == "Player")
        {
            playerPos = other.transform;
            maskInventory = other.GetComponentInParent<MaskInventory>();
            Debug.Log("in : " + maskInventory.ListMasks.Count);
            if (maskInventory.ListMasks.Count >= nbrMaskToOpen)
            {
                for (int i = 0; i < StockMask.childCount; i++)
                {
                    StockMask.GetChild(i).position = other.transform.position;
                    listDoorMask.Add(StockMask.GetChild(i));
                }

                TimeManager.Instance.Block_Player_WithTimer(durationFreeze);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isTryOpenning = false;
        Debug.Log("out : " + maskInventory.ListMasks.Count);
        if (doorActivaded)
        {
            maskInventory = null;
            GetComponents<BoxCollider>()[1].enabled = false;
            doorActivaded = false;
        }
    }
}
