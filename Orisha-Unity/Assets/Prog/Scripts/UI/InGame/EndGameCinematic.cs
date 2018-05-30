using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameCinematic : MonoBehaviour {

    [SerializeField] private string mainMenuName = "MainMenu";
    [SerializeField] private bool isMainMenuStartLoading = false;

    private void Start() {
        isMainMenuStartLoading = false;
    }

    // Update is called once per frame
    void Update () {
		if(isMainMenuStartLoading) {
            SceneManager.LoadScene(mainMenuName);
        }
	}
}
