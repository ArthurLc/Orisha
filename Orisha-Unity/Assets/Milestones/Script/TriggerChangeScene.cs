using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Milestone
{

    public class TriggerChangeScene : MonoBehaviour
    {
        public List<GameObject> toDestroy;


        public string sceneToLoad;

        private void Start()
        {
            toDestroy.Add(GameObject.Find("InputManager"));
            toDestroy.Add(GameObject.Find("Debug"));
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                foreach(GameObject go in toDestroy)
                {
                    Destroy(go);
                }

                SceneManager.LoadScene(sceneToLoad);
            }

        }
    }

}
