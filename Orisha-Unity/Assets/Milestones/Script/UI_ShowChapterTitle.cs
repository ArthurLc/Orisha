using UnityEngine;
using UnityEngine.UI;

namespace Milestone
{
    public class UI_ShowChapterTitle : MonoBehaviour
    {
        bool isActive = false;
        Text uiText;
        Image uiImage;

        // Use this for initialization
        void Start()
        {
            uiText = GetComponentInChildren<Text>();
            uiImage = GetComponentInChildren<Image>();
        }

        // Update is called once per frame
        void Update()
        {
            uiText.enabled = isActive;
            uiImage.enabled = isActive;
        }

        public void ChangeTitle(string _newTitle)
        {
            uiText.text = _newTitle;
            isActive = true;
        }
    }
}
