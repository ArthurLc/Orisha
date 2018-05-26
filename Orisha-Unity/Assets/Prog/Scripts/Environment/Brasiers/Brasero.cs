using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brasero : MonoBehaviour
{
    [Header("Particles")]
    [SerializeField] ParticleSystem mainPs;
    [SerializeField] ParticleSystem subPs;

    [Header("Sfx")]
    [SerializeField] AudioClip fireLoop;
    [SerializeField] AudioClip fireStart;

    SFX_Object fireLoopSource;

    private void Start()
    {
        if(mainPs.gameObject.activeSelf || subPs.gameObject.activeSelf)
            fireLoopSource = SoundManager.instance.SFX_LoopAtPosition(fireLoop, transform.position, 0, 30);
    }

    public void StartBrasero()
    {
        if (!mainPs.gameObject.activeSelf)
        {
            mainPs.gameObject.SetActive(true);
            if(fireLoop)
                fireLoopSource = SoundManager.instance.SFX_LoopAtPosition(fireLoop, transform.position, 0, 30);
            if(fireStart)
                SoundManager.instance.SFX_PlayOneShot(fireStart);
        }
    }

    public void StopBrasero()
    {
        if(mainPs)
            mainPs.gameObject.SetActive(false);
        if(subPs)
            subPs.gameObject.SetActive(false);

        if (fireLoopSource)
            SFX_Pool.Instance.ReturnToPool(fireLoopSource);
    }


    public void StartSubBrasero()
    {
        if (!subPs.gameObject.activeSelf)
        {
            subPs.gameObject.SetActive(true);
            if (fireLoop)
                SoundManager.instance.SFX_LoopAtPosition(fireLoop, transform.position);
            if (fireStart)
                SoundManager.instance.SFX_PlayOneShot(fireStart);
        }
    }
}
