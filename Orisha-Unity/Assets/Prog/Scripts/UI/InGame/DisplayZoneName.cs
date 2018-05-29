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
    float timerBG;
    int index;

    Text txt;

    [SerializeField] private Image background;
    [SerializeField][Range(0.0f,255.0f)] private float transparenceMax;
    private float transparenceSpeed;
    private Color color_BG;

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

        if (background != null)
            color_BG = background.color;
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
        transparenceSpeed = transparenceMax / fadeInDuration;

        timer = 0.0f;
        timerBG = 0.0f;
        index = 0;
        state = DisplayState.fadingIn;

        source.Play();
    }


    void FadeIn()
    {
        if(index < zoneName.Length + 1)// fading in
        {
            timer += Time.deltaTime;
            timerBG += Time.deltaTime;
            if (background != null && ((timerBG * transparenceSpeed) < transparenceMax))
            {
                background.color = new Color(color_BG.r, color_BG.g, color_BG.b, (timerBG * transparenceSpeed) / 100.0f);
                if (timerBG > transparenceMax)
                    background.color = new Color(color_BG.r, color_BG.g, color_BG.b, transparenceMax);
            }
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
            timerBG = fadeInDuration;
            state = DisplayState.showing;
            background.color = new Color(color_BG.r, color_BG.g, color_BG.b, transparenceMax);
        }
    }

    void FadeOut()
    {
        if (index < zoneName.Length + 1)// fading out
        {
            timer += Time.deltaTime;
            timerBG -= Time.deltaTime;
            if (background != null)
            {
                background.color = new Color(color_BG.r, color_BG.g, color_BG.b, (timerBG * transparenceSpeed) / 100.0f);
                if (timerBG < 0.0f)
                    background.color = new Color(color_BG.r, color_BG.g, color_BG.b, 0.0f);
            }
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
            index = 0;
            state = DisplayState.inactive;
            background.color = new Color(color_BG.r, color_BG.g, color_BG.b, 0.0f);
        }
    }
}
