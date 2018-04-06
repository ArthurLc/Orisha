using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using vd_Inputs;

public class FirstScreen : MonoBehaviour
{
    public KeyCode launchKey;
    bool isFading;
    float timer;
    Color color;

    MaskableGraphic[] imagesAndTexts;

    private void Start()
    {
        imagesAndTexts = GetComponentsInChildren<MaskableGraphic>();
        timer = 1.0f;
    }

    // Update is called once per frame
    void Update ()
    {
		if ((Input.GetKeyDown(launchKey) || Input.GetAxis(InputManager.Focus) > 0.5f))
        {
            isFading = true;
        }

        if(isFading)
        {
            timer -= Time.deltaTime;
            if(timer < 0.0f)
            {
                timer = 0.0f;
                foreach (MaskableGraphic graph in imagesAndTexts)
                {
                    color = graph.color;
                    color.a = timer;
                    graph.color = color;
                }
                isFading = false;
                return;
            }
            foreach(MaskableGraphic graph in imagesAndTexts)
            {
                color = graph.color;
                color.a = timer;
                graph.color = color;
            }
        }
	}
}
