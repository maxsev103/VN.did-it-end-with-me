using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace DIALOGUE
{
    [System.Serializable]
    public class DialogueContainer
    {
        public GameObject root;
        public Image dialogueBox;
        public NameContainer nameContainer;
        public TextMeshProUGUI dialogueText;

        private CanvasGroupController cgController;

        public void SetDialogueColor(Color color) => dialogueText.color = color;   
        public void SetDialogueFont(TMP_FontAsset font) => dialogueText.font = font;
        public void SetDialogueFontSize(float size) => dialogueText.fontSize = size;

        public void SetDialogueBoxAlpha(float alpha) => dialogueBox.color = new Color(dialogueBox.color.r, dialogueBox.color.g, dialogueBox.color.b, alpha);
        public void ResetAlpha() => dialogueBox.color = new Color(dialogueBox.color.r, dialogueBox.color.g, dialogueBox.color.b, 0.91f);

        private bool initialized = false;
        public void Initialize()
        {
            if (initialized) 
                return;

            cgController = new CanvasGroupController(DialogueSystem.instance, root.GetComponent<CanvasGroup>());
            initialized = true;
        }

        public bool isVisible => cgController.isVisible;
        public Coroutine Show(float speed = 1f, bool immediate = false) => cgController.Show(speed, immediate);
        public Coroutine Hide(float speed = 1f, bool immediate = false) => cgController.Hide(speed, immediate);
    }
}