using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brasero : MonoBehaviour
{
    [SerializeField] ParticleSystem ps;
    [SerializeField] AudioClip fireLoop;
    [SerializeField] AudioClip fireStart;

    public void StartBrasero()
    {
        if (!ps.gameObject.activeSelf)
        {
            ps.gameObject.SetActive(true);
            if(fireLoop)
                SoundManager.instance.SFX_LoopAtPosition(fireLoop, transform.position);
            if(fireStart)
                SoundManager.instance.SFX_PlayOneShot(fireStart);
        }
    }
}
