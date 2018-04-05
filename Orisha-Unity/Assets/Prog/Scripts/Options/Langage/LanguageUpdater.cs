/* LangageUpdater.cs
 * Script qui s'attache à un Text de la scène pour traduire son contenu en fonction de la langue sélectionnée
 * 
 * Pour qu'un Text s'adapte à la langue choisie :
 *  - lui joindre le script "LangageUpdater" et préciser sa key,
 *  - ajouter sa key et le contenu auquel elle correspond dans les databases de langue
 *   
 * Ajouté par Ambre LACOUR le 16/10/2017
 * Dernière modification par  le 
 */

using UnityEngine;
using UnityEngine.UI;


namespace vd_Options
{

    public class LanguageUpdater : MonoBehaviour
    {

        [SerializeField] private string key;

        void Awake()
        {
            LanguageSO.UpdateEvent.AddListener(UpdateL10n);
        }

        void OnDestroy()
        {
            LanguageSO.UpdateEvent.RemoveListener(UpdateL10n);
        }

        void UpdateL10n()
        {
            if (GetComponent<Text>() != null)
                GetComponent<Text>().text = LanguageSO.GetText(key);
            else if (GetComponent<Button>() != null)
                GetComponentInChildren<Text>().text = LanguageSO.GetText(key);
        }

    }

}
