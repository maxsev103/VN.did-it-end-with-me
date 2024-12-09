using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace DIALOGUE
{
    [System.Serializable]
    /// <summary>
    /// The box that holds the speaker's name in a dialogue box. Part of the dialogue container
    /// </summary>
    public class NameContainer
    {
        [SerializeField] private GameObject root;
        public Image nameBox;
        [field:SerializeField] public TextMeshProUGUI nameText { get; private set; }

        public void SetNameColor(Color color) => nameText.color = color;
        public void SetNameFont(TMP_FontAsset font) => nameText.font = font;
        public void SetNameFontSize(float size) => nameText.fontSize = size;
        public void SetNameBoxAlpha(float alpha) => nameBox.color = new Color(nameBox.color.r, nameBox.color.g, nameBox.color.b, alpha);
        public void ResetAlpha() => nameBox.color = new Color(nameBox.color.r, nameBox.color.g, nameBox.color.b, 0.91f);

        public void Show(string nameToShow = "")
        {
            root.SetActive(true);
            if (nameToShow != string.Empty)
                nameText.text = nameToShow;
        }

        public void Hide()
        {
            root.SetActive(false);
        }
    }
}