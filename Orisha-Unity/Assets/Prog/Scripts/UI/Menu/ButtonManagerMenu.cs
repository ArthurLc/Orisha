/* ButtonManagerMenu.cs
* script contenant ce qui est utile à la pression des boutons
* Objectifs : 
    - Fonction public d'utilisation des bouttons
    - Manipulation des objets du prefab "Canvas" du MainMenu uniquement
    - Affichage d'images cosmethiques en plus du chargement du page, etc.


* Crée par Arthur LACOUR le 01/12/2018
* Dernière modification par Arthur LACOUR le 01/12/2018

*/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManagerMenu : MonoBehaviour {

    [System.Serializable]
    struct ButtonPossibilities
    {
        public Button button; //Bouton dans la scène
        public List<GameObject> relativeObjects; //Liste des GameObject relatifs au bouton à activer
    }

    [SerializeField] bool isFirstElementEnable = false; //Est-ce que le premier élément de la liste est de base actif ?
    [SerializeField] List<ButtonPossibilities> buttons = new List<ButtonPossibilities>(); //Liste des boutons du MainMenu
    int lastButtonID = -1;

    private void Awake()
    {
        if (isFirstElementEnable == true)
            lastButtonID = 0;
    }

    public void Open_SubMenu(Button _button)
    {
        int newButtonID = -1;
        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i].button == _button)
                newButtonID = i;
        }

        if (lastButtonID != newButtonID)
        {
            if (lastButtonID != -1)
                ActiveSubMenu(lastButtonID, false);

            lastButtonID = newButtonID;
            ActiveSubMenu(lastButtonID, true);
        }
    }
    public void OpenOrClose_SubMenu(Button _button)
    {
        int newButtonID = -1;
        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i].button == _button)
                newButtonID = i;
        }

        if (lastButtonID != -1 && lastButtonID != newButtonID)
            ActiveSubMenu(lastButtonID, false);

        lastButtonID = newButtonID;
        ActiveSubMenu(lastButtonID, !buttons[lastButtonID].relativeObjects[0].activeInHierarchy);
    }
    void ActiveSubMenu(int _lastButtonID, bool _active)
    {
        for (int i = 0; i < buttons[_lastButtonID].relativeObjects.Count; i++) {
            buttons[_lastButtonID].relativeObjects[i].SetActive(_active);
        }
    }
}
