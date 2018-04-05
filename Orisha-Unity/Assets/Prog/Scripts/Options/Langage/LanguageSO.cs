/* LangageSO.cs
 * Script qui permet de créer un scriptable object qui va contenir les textes d'une langue
 * 
 * - Par défaut la database anglaise est chargée
 * - Pour charger la langue, appeler "LoadDatabase()"
 * - Pour qu'un Text s'adapte à la langue choisie, lui joindre le script "LangageUpdater" et ajouter sa clef et sonn texte dans les databases de langues
 * - NB : L'info de la langue sélectionnée est stockée dans le gameloop manager
 * 
 * Ajouté par Ambre LACOUR le 16/10/2017
 * Dernière modification par  le 
 */



using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;


namespace vd_Options
{
    [CreateAssetMenu(fileName = "LanguageDatabase.asset", menuName = "Language Database")]
    public class LanguageSO : ScriptableObject
    {

        [System.Serializable]
        private struct KeyValueStore
        {
            public string key;
            public string value;
        }

        [SerializeField] private string languageName;

        [SerializeField] private List<KeyValueStore> entries;

        private static UnityEvent updateEvent = new UnityEvent();
        public static UnityEvent UpdateEvent { get { return updateEvent; } }

        private static LanguageSO activeDatabase = null;
        private static LanguageSO Database
        {
            get
            {
                if (activeDatabase == null)
                    activeDatabase = Resources.Load<LanguageSO>("EN");
                return activeDatabase;
            }
        }

        public static void LoadDatabase(string language)
        {
            activeDatabase = Resources.Load<LanguageSO>(language);
            UpdateEvent.Invoke();
        }

        public static string GetText(string key)
        {
            LanguageSO db = Database;
            foreach (KeyValueStore store in db.entries)
            {
                if (store.key == key)
                    return store.value;
            }
            return string.Format("[{0}]", key);
        }
    }

}
