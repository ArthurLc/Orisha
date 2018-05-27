using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BraseroTriggerActivation : MonoBehaviour
{
    [SerializeField]Brasero[] braseros;
    private void OnTriggerEnter(Collider other)
    {
        if (braseros.Length > 0 && other.tag == "Player")
        {
            for (int i = 0; i < braseros.Length; i++)
                braseros[i].StartBrasero();

            Destroy(this);
        }
    }
}
