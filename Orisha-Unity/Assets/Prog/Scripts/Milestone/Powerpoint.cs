using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Powerpoint : MonoBehaviour
{
    float timer = 100f;
    [SerializeField] float slideDuration = 4f;
    [SerializeField] Sprite[] slides;
    [SerializeField] Image refCanvas;
    int id = 0;
	// Update is called once per frame
	void Update ()
    {
        timer += Time.deltaTime;

        if(timer >= slideDuration)
        {
            refCanvas.sprite = slides[id%slides.Length];
            ++id;
            timer = 0f;
        }
	}
}
