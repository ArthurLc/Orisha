using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Trello;

public class BugReporter : MonoBehaviour
{    
    public Button displayButton;
    public GameObject pnl_Window;
    private bool isDisplayed;


    void Start ()
    {
        if(GameObject.FindObjectOfType<BugReporter>() != null && GameObject.FindObjectOfType<BugReporter>() != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        isDisplayed = false;
        pnl_Window.SetActive(isDisplayed);
    }


    public void OnDisplayButtonClick()
    {
        isDisplayed = !isDisplayed;
        pnl_Window.SetActive(isDisplayed);
    }
}
