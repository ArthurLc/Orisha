using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AmbianceZone : MonoBehaviour {
    
    enum MusicModificationMode
    {
        Music,
        Fighting
    }

    [Header("Settings")]
    [SerializeField] MusicModificationMode ambianceMusicChange = MusicModificationMode.Music;
    [SerializeField] AudioClip ambianceMusic;
    private AudioClip previousMusic;

    private vd_Player.CharacterInitialization ci;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (ci == null)
                ci = other.GetComponentInParent<vd_Player.CharacterInitialization>();

            switch (ambianceMusicChange)
            {
                case MusicModificationMode.Music:
                    if (ambianceMusic != null)
                        previousMusic = ci.Audio_MainCam.ChangeMusic(ambianceMusic);
                    else
                        Debug.LogError("Music non référencé !");
                    break;
                case MusicModificationMode.Fighting:
                    if (ambianceMusic != null)
                        previousMusic = ci.Audio_MainCam.ChangeFighting(ambianceMusic);
                    else
                        Debug.LogError("Music non référencé !");
                    break;
                default:
                    break;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (ci == null)
                ci = other.GetComponentInParent<vd_Player.CharacterInitialization>();
            
            switch (ambianceMusicChange)
            {
                case MusicModificationMode.Music:
                    if (ambianceMusic != null)
                        ci.Audio_MainCam.ChangeMusic(previousMusic);
                    else
                        Debug.LogError("Music non référencé !");
                    break;
                case MusicModificationMode.Fighting:
                    if (ambianceMusic != null)
                        ci.Audio_MainCam.ChangeFighting(previousMusic);
                    else
                        Debug.LogError("Music non référencé !");
                    break;
                default:
                    break;
            }
        }
    }
}
