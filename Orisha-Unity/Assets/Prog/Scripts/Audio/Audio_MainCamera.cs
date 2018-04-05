/*
* @AmbreLacour
* @Audio_MainCamera.cs
* @07/02/2017
* @Script attaché à la camera du joueur, a accès aux audio sources en enfant de celle-ci (UI, ambiance et musique)
*   - Permet de manager l'ambiance (musique et sons d'ambiance)
*   - Permet de jouer des sons d'UI (alarme de vie basse, son de mort etc.)
*   
* @Edit 26/03/2018
*   - Ajout de la Fighting_Music, permet de fade de la musique d'ambiance à la musique de combat.
* 
*/

using UnityEngine;

public class Audio_MainCamera : MonoBehaviour
{
    [SerializeField] AudioSource source_UI;
    [SerializeField] AudioSource source_Music;
    [SerializeField] AudioSource source_Ambient;
    [SerializeField] AudioSource source_Fighting;

    bool isFading = false;
    float save_MusicTime = 0.0f;

    float fadeVolume = 0.0f;
    // Use this for initialization
    void Start ()
    {
        Potential_Enemy.enteringFight += StartFade;
    }

	// Update is called once per frame
	void Update ()
    {
		if(isFading)
        {
            Fade();
        }
	}

    void StartFade(bool enteringFight)
    {
        if((!source_Fighting.isPlaying && enteringFight) || (!source_Music.isPlaying && !enteringFight))
        {
            isFading = true;
            if (enteringFight)
            {

                save_MusicTime = source_Music.time;
                source_Fighting.volume = 0.0f;
                source_Fighting.Play();
                source_Fighting.volume = 0.0f;
            }
            else
            {
                source_Music.Play();
                source_Music.volume = 0.0f;
                source_Music.time = save_MusicTime;
            }
            fadeVolume = 0.0f;
        }     
    }

    void Fade()
    {

        fadeVolume += Time.deltaTime * 0.3f;
        fadeVolume = Mathf.Clamp(fadeVolume, 0.0f, 1.0f);

        if (Potential_Enemy.IsOnFight)
        {
            source_Music.volume = 1 - fadeVolume;
            source_Fighting.volume = fadeVolume;
        }
        else
        {
            source_Music.volume = fadeVolume;
            source_Fighting.volume = 1 - fadeVolume;
        }

        if(fadeVolume == 1)
        {
            isFading = false;
            fadeVolume = 0.0f;
            if (Potential_Enemy.IsOnFight)
                source_Music.Stop();
            else
                source_Fighting.Stop();
        }
    }

    private void OnDestroy()
    {
        Potential_Enemy.enteringFight -= StartFade;
    }
}
