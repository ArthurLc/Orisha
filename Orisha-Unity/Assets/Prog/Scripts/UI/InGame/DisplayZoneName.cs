/*
* @AmbreLacour
* @DisplayZoneName.cs
* @05/02/2018
* @ - Le Script s'attache a un text sur un canvas
*   - Est appelé par les scripts de ZoneDiscovery lorsqu'une zone est découverte pour en afficher le nom
*/

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text), typeof(AudioSource))]
public class DisplayZoneName : MonoBehaviour
{
    string zoneName;
    float fadeInDuration;
    float displayDuration;

    float timer;
    int index;

    Text txt;

    // Sounds
    private AudioSource source;

    enum DisplayState
    {
        inactive,
        fadingIn,
        showing,
        fadingOut
    }
    DisplayState state;


    void Start ()
    {       
        txt = GetComponent<Text>();
        source = GetComponent<AudioSource>();
    }


    void Update ()
    {
		switch(state)
        {
            case DisplayState.fadingIn:
                FadeIn();
                break;

            case DisplayState.showing:
                timer -= Time.deltaTime;
                if (timer < 0)
                    state = DisplayState.fadingOut;
                break;

            case DisplayState.fadingOut:
                FadeOut();
                break;

            case DisplayState.inactive:
                break;
        }
	}

    public void BeginDisplay(string name, float apparitionSpeed, float duration)
    {
        zoneName = name;
        fadeInDuration = apparitionSpeed;
        displayDuration = duration;

        timer = 0.0f;
        state = DisplayState.fadingIn;

        source.Play();
    }


    void FadeIn()
    {
        if(index < zoneName.Length + 1)// fading in
        {
            timer += Time.deltaTime;
            if (timer > (fadeInDuration / zoneName.Length)) // Affichage lettre/lettre
            {
                txt.text = "<color=#453800ff>";
                for (int i = 0; i < index; i++) // text showed
                    txt.text += zoneName[i];

                if(index < zoneName.Length)
                txt.text += "</color><color=#4538007b>" + zoneName[index] + "</color><color=#45380000>";

                for (int i = index+1; i < zoneName.Length; i++) // transparent txt;
                    txt.text += zoneName[i]; ;
                txt.text += "</color>";

                index++;
                timer = 0;
            }
        }
        else // end of fade in
        {
            index = 0;
            timer = displayDuration;
            state = DisplayState.showing;
        }
    }

    void FadeOut()
    {
        if (index < zoneName.Length + 1)// fading out
        {
            timer += Time.deltaTime;
            if (timer > (fadeInDuration / zoneName.Length)) // Affichage lettre/lettre
            {
                txt.text = "<color=#45380000>";
                for (int i = 0; i < index; i++) // text showed
                    txt.text += zoneName[i];

                if (index < zoneName.Length)
                    txt.text += "</color><color=#4538007b>" + zoneName[index] + "</color><color=#453800ff>";

                for (int i = index + 1; i < zoneName.Length; i++) // transparent txt;
                    txt.text += zoneName[i]; ;
                txt.text += "</color>";

                index++;
                timer = 0;
            }
        }
        else // end of fading out
        {
            state = DisplayState.inactive;
        }
    }
}
