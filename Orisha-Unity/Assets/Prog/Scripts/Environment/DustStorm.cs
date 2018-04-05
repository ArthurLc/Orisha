using UnityEngine;

    public class DustStorm : MonoBehaviour {

        ParticleSystem psDustStorm;

         [Range(1.0f, 50.0f)] public float offset;

        // Use this for initialization
        void Start() {
            psDustStorm = GetComponent<ParticleSystem>();
        }

        // Update is called once per frame
        void Update()
        {
            psDustStorm.transform.position = Camera.main.transform.position + Camera.main.transform.forward * offset;
            psDustStorm.transform.rotation = Camera.main.transform.rotation;

        }
    }
