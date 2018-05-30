using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AI_Enemy_Basic))]
public class Givemask : MonoBehaviour
{
    [SerializeField] GameObject MaskToGive;

    AI_Enemy_Basic me;
    bool spawnMask = false;
    bool colEnabled = false;
    [SerializeField] float yOffest;
    [SerializeField] float ySpeed;

    Mask mask;
    ZoneDiscovery zd;
    CapsuleCollider c;
    float yCount = 0;
    Vector3 startPos;
	// Use this for initialization
	void Start ()
    {
        me = GetComponent<AI_Enemy_Basic>();

        zd = MaskToGive.GetComponent<ZoneDiscovery>();
        mask = MaskToGive.GetComponent<Mask>();
        c = MaskToGive.GetComponent<CapsuleCollider>();

        zd.enabled = mask.enabled = c.enabled = false;
        MaskToGive.SetActive(false);
        
    }
	
	// Update is called once per frame
	void Update ()
    {
		if(me.Health <= 0 && !spawnMask)
        {
            spawnMask = true;
            MaskToGive.SetActive(true);
            startPos = MaskToGive.transform.position;
            MaskToGive.transform.parent = null;
        }
        else if(spawnMask && yCount < yOffest)
        {
            float value = (ySpeed * Time.deltaTime);

            yCount += value;

            MaskToGive.transform.position = new Vector3(MaskToGive.transform.position.x, startPos.y + yCount, MaskToGive.transform.position.z);
        }
        else if(!colEnabled && yCount >= yOffest)
        {
            zd.enabled = mask.enabled = c.enabled = true;
            colEnabled = true;
        }
	}
}
