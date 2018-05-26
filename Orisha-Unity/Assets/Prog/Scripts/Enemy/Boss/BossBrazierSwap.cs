using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBrazierSwap : MonoBehaviour
{
    [SerializeField] AI_Enemy_Basic boss;
    [SerializeField] Brasero[] braseros;
	// Use this for initialization
	void Start ()
    {
        if (!boss)
            Debug.LogWarning("There is no Boss attached to BossBrazierSwap");

        if (braseros.Length == 0)
            Debug.LogWarning("There is no braseros attached to BossBrazierSwap");
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (boss && boss.Health <= 0)
        {
            StartCoroutine(SwapParticles(0.5f));
            enabled = false;
        }
	}

    IEnumerator SwapParticles(float _frequency)
    {
        for(int i = 0; i < braseros.Length; ++i)
        {
            braseros[i].StopBrasero();
            braseros[i].StartSubBrasero();

            yield return new WaitForSeconds(_frequency);
        }
    }
}
