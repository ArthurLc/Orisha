using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brasero : MonoBehaviour
{
    [SerializeField] ParticleSystem ps;
    [SerializeField] AudioClip clip;
	
    public void StartBrasero()
    {
        if (!ps.gameObject.activeSelf)
        {
            ps.gameObject.SetActive(true);
            SoundManager.instance.SFX_LoopAtPosition(clip, transform.position);
        }
    }
}
