using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/*
* @ArthurLacour
* @SettingsManager.cs
* @14/02/2018
* @ - Le Script est un manager qui va gérer la sauvegarde/le chargement des Settings du joueur.
*/
public class SettingsManager : MonoBehaviour {

    //Listes des settings. (éditables depuis le prefab)
    [SerializeField] List<string> stringSettings = new List<string>();
    [SerializeField] List<string> intSettings = new List<string>();
    [SerializeField] List<string> floatSettings = new List<string>();
    //Dictionnaire de lecture des settings.
    Dictionary<string, string> stringDictionary = new Dictionary<string, string>();
    Dictionary<string, int> intDictionary = new Dictionary<string, int>();
    Dictionary<string, float> floatDictionary = new Dictionary<string, float>();

    private void Start () {
        FreeEveryDictonary();
        LoadAllSettings();
    }
    private void OnDestroy() {
        SaveAllSettings();
    }
    
    public void LoadAllSettings()
    {
        AsignEveryDictionary();

        foreach (KeyValuePair<string, string> entry in stringDictionary)
        {
            PlayerPrefs.GetString(entry.Key, entry.Value);
        }
        foreach (KeyValuePair<string, int> entry in intDictionary)
        {
            PlayerPrefs.GetInt(entry.Key, entry.Value);
        }
        foreach (KeyValuePair<string, float> entry in floatDictionary)
        {
            PlayerPrefs.GetFloat(entry.Key, entry.Value);
        }
    }
    public void SaveAllSettings()
    {
        foreach (KeyValuePair<string, string> entry in stringDictionary)
        {
            PlayerPrefs.SetString(entry.Key, entry.Value);
        }
        foreach (KeyValuePair<string, int> entry in intDictionary)
        {
            PlayerPrefs.SetInt(entry.Key, entry.Value);
        }
        foreach (KeyValuePair<string, float> entry in floatDictionary)
        {
            PlayerPrefs.SetFloat(entry.Key, entry.Value);
        }

        PlayerPrefs.Save();
    }

    void FreeEveryDictonary()
    {
        stringDictionary.Clear();
        intDictionary.Clear();
        floatDictionary.Clear();
    }
    void AsignEveryDictionary()
    {
        for(int i = 0; i < stringSettings.Count; i++) {
            stringDictionary.Add(stringSettings[i], PlayerPrefs.GetString(stringSettings[i]));
        }
        for (int i = 0; i < intSettings.Count; i++) {
            intDictionary.Add(intSettings[i], PlayerPrefs.GetInt(intSettings[i]));
        }
        for (int i = 0; i < floatSettings.Count; i++) {
            floatDictionary.Add(floatSettings[i], PlayerPrefs.GetFloat(floatSettings[i]));
        }
    }
}
