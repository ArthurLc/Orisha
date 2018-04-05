using UnityEngine;
using UnityEngine.UI;

namespace Milestone
{
    public class Trigger_ChapterTitleUpdate : MonoBehaviour
    {
        public string title = "Orisha";
        UI_ShowChapterTitle txt;

        void Start()
        {
            txt = FindObjectOfType<UI_ShowChapterTitle>();
        }

        private void OnTriggerEnter(Collider other)
        {
            txt.ChangeTitle(title);

        }

        private void OnCollisionEnter(Collision collision)
        {
            txt.ChangeTitle(title);
        }
    }
}
