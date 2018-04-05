// GameLoopManager.cs
// Script absent de la scène, assure la continuité entre menu et niveau (stocke seulement certaines données)
// Objectifs : 
// - Enregistrement des modes d'input de chaque joueur
// - Chargement du niveau selon le nom de la scène
// - Infos des options (langue, axes inversés ou non etc.)
// Crée par Ambre LACOUR le 08/10/2017
// Dernière modification par Ambre LACOUR le 23/11/2017 (getteur sur PlayInSoloMode) 

using UnityEngine;
using UnityEngine.SceneManagement;
using vd_Options;

using vd_Inputs;

public enum SelectedCharacter
{
    charA,
    charB,
    none
};

public static class GameLoopManager
{

    static string sceneToLoad = string.Empty;
    public static string GetLevelToLoad()
    {
        return sceneToLoad;
    }

    /// <summary>
    ///     Chargement du niveau
    /// </summary>
    public static void LaunchLevel(string _levelName, bool isLoadingScreenNeeded)
    {
        if (!isLoadingScreenNeeded)
        {
            Debug.Log("Loading scene : " + _levelName);
            SceneManager.LoadScene(_levelName);
        }
        else
        {
            sceneToLoad = _levelName;
            SceneManager.LoadScene("LoadingScene");
        }
    }
    
    public static void EnableMouse()
    {
        if (InputManager.GetInputMode == InputMode.keyboard)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
    public static void DisableMouse()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Langues
    private static string[] langages = { "EN", "FR" };
    public static string[] Langages { get { return langages; } }

    static string currentLangage = "EN";
    public static string CurrentLangage
    {
        get { return currentLangage; }
        set { currentLangage = value; if(IsLangageOk(value)) LanguageSO.LoadDatabase(currentLangage); }
    }
    public static int CurrentLangageIndex
    {
        get { for (int i = 0; i < langages.Length; i++) { if (langages[i] == currentLangage) return i; } return -1; }
    }

    private static bool IsLangageOk(string _langage)
    {
        for(int i = 0; i < langages.Length; i++)
        {
            if (langages[i] == _langage)
                return true;
        }
        return false;
    }


    // Axes de camera inversé ou non
    public static bool camera_isXaxisInversed = false;
    public static bool camera_isYaxisInversed = false;

    // Sensibilité de camera
    public static float camera_sensitivity = 1.0f;

}
