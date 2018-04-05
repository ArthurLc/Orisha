using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaleZone : MonoBehaviour {

    [SerializeField] private float percentThunderbolt;
    [SerializeField] private float frequency;

    private float timerFreq = 0.0f;
    private Light lightThunder;
    private AudioSource source;

    public bool isLightOn = false;

	// Use this for initialization
	void Start () {
        lightThunder = GetComponentInChildren<Light>();
        source = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {

        timerFreq += Time.deltaTime;
        
        if (timerFreq > frequency && isLightOn)
        {
            float testRand = Random.value * 100.0f;
            if(testRand < percentThunderbolt)
            {
                lightThunder.enabled = true;
                if(!source.isPlaying)
                    source.PlayOneShot(source.clip);
            }
            else
            {
                lightThunder.enabled = false;
                timerFreq = 0.0f;
            }
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            isLightOn = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            isLightOn = false;
        }
    }
}
