
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
    private bool emissiveActivated = false;

	private MeshRenderer MrEmissve;

    private MaskInventory maskInventory;
    private Transform playerPos;
	private Transform StockMask;
	private Transform partDoor;
	private Transform[] doorPart = new Transform[3];
    private List<Transform> listDoorMask = new List<Transform>();

    [Header("Cinematic")]
    [SerializeField] UnityEngine.Playables.PlayableDirector playableDirector;
    [SerializeField] private bool isCinematicFreezeEnnemies = true; //Booléan nécessaire pour la cinématique
    private bool isCinematicDoorNoMaskPlayed;

    // Use this for initialization
    void Start () {
        for (int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).name == "StockMask")
            {
                StockMask = transform.GetChild(i);
            }
			if(transform.GetChild(i).name == "partDoor")
			{
				partDoor = transform.GetChild (i);
				doorPart[0] = partDoor.GetChild(0);
				doorPart[1] = partDoor.GetChild(1);
				doorPart[2] = partDoor.GetChild(2);
			}
        }
		MrEmissve = GetComponent<MeshRenderer> ();

		MrEmissve.sharedMaterials[1].SetColor ("_EmissionColor", Color.clear);
		MrEmissve.sharedMaterials[2].SetColor ("_EmissionColor", Color.clear);

        isCinematicDoorNoMaskPlayed = false;
	}
	
	// Update is called once per frame
	void Update () {

        if (maskInventory == null)
            return;

		if (isTryOpenning && maskInventory.ListMasks.Count >= nbrMaskToOpen) {
			lerpValueMask += Time.deltaTime * 0.5f;
			durationFreeze -= Time.deltaTime;

			if (listDoorMask.Count == 0)
				return;

			//Boucle sur les spot de mask (lerp pos/scale/rot)
			for (int i = 0; i < 3; i++) {
				listDoorMask [i].position = Vector3.Lerp (playerPos.position, transform.GetChild (i).position, lerpValueMask);
				listDoorMask [i].localScale = Vector3.Lerp (new Vector3 (0.01f, 0.01f, 0.01f), new Vector3 (1.0f, 1.0f, 1.0f), lerpValueMask);
				listDoorMask [i].Rotate (Vector3.up, 20.0f);
			}

			//Fin du lerp reset rotation , list player, et déclanche la porte
			if (lerpValueMask >= 1.0f) {
				for (int i = 0; i < 3; i++) {
					listDoorMask [i].rotation = transform.GetChild (i).rotation;
					listDoorMask [i].parent = doorPart [i];
				}
				maskInventory.ResetListMask ();

				lerpValueMask = 0.0f;
				emissiveActivated = true;
			}
		} else {
			isTryOpenning = false;
		}

		if (emissiveActivated) {
			lerpValueMask += Time.deltaTime;
			if (lerpValueMask <= 1.0f) {
				Color emissiveColor = new Color (255.0f, 135.0f, 0.0f, 255.0f);
				float H,S,V;
				Color.RGBToHSV (emissiveColor, out H, out S, out V);

				MrEmissve.sharedMaterials[1].SetColor ("_EmissionColor", Color.HSVToRGB (H, S, lerpValueMask));
				MrEmissve.sharedMaterials[2].SetColor ("_EmissionColor", Color.HSVToRGB (H, S, lerpValueMask));
			} else {
				doorActivaded = true;
				emissiveActivated = false;

				playerPos.GetComponentInParent<CameraShaker>().ShakeActualCam (6.5f, 0.1f, 0.001f);
			}
		}

        //move the door
        if(doorActivaded)
        {
			Debug.Log ("OPENING !!");
            //transform.position += (Vector3.up * Time.deltaTime);
			doorPart[0].position += (partDoor.up * Time.deltaTime);
			doorPart[1].position += (partDoor.right * Time.deltaTime);
			doorPart[2].position += (-partDoor.right * Time.deltaTime);

			if (doorPart [0].localPosition.y >= 9.0f) {
				isTryOpenning = false;
				Debug.Log("out : " + maskInventory.ListMasks.Count);
				if (doorActivaded)
				{
					maskInventory = null;
					GetComponent<SphereCollider>().enabled = false;
					doorActivaded = false;
				}
			}
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        isTryOpenning = true;
        if(other.tag == "Player")
        {
            playerPos = other.transform;
            maskInventory = other.GetComponentInParent<MaskInventory>();
            if (maskInventory.ListMasks.Count >= nbrMaskToOpen)
            {
                for (int i = 0; i < StockMask.childCount; i++)
                {
                    StockMask.GetChild(i).position = other.transform.position;
                    listDoorMask.Add(StockMask.GetChild(i));
                }

                TimeManager.Instance.Block_Player_WithTimer(durationFreeze);
                MaskInventory.Instance.EquipDefaultMask();
            }
            else if(isCinematicDoorNoMaskPlayed == false) //Sinon, on envois le tuto
            {
                playableDirector.Play();

                TimeManager.Instance.Block_Player_WithTimer((float)playableDirector.duration);
                if (isCinematicFreezeEnnemies)
                    TimeManager.Instance.Block_Ennemies_WithTimer((float)playableDirector.duration, true);

                isCinematicDoorNoMaskPlayed = true;
            }
        }
    }

}
