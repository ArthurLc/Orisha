using UnityEngine;

/*
* @ArthurLacour
* @Breakable.cs
* @03/05/2018
* @Le script s'attache à un GameObject qui possède le collider de destruction.
*   - Permet de briser un pot en linkan la partie break de l'objet.
*/

public class Breakable : MonoBehaviour {
    [SerializeField] string nameBreakerLayer;
    [SerializeField] GameObject breakObject;
    [SerializeField] AudioClip breakSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(nameBreakerLayer))
        {
            breakObject.SetActive(true);
            SoundManager.instance.SFX_PlayOneShot(breakSound);
            Destroy(this.gameObject);
        }
    }
}
