using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DIALOGUE;
using TMPro;

namespace HISTORY
{
    [System.Serializable]
    public class DialogueData 
    { 
        public string currentDialogue = "";
        public string currentSpeaker = "";

        public string dialogueFont;
        public Color dialogueColor;
        public float dialogueFontSize;
        
        public string speakerFont;
        public Color speakerNameColor;
        public float speakerFontSize;

        public static DialogueData Capture()
        {
            DialogueData data = new DialogueData();

            var ds = DialogueSystem.instance;
            var dialogueText = ds.dialogueContainer.dialogueText;
            var nameText = ds.dialogueContainer.nameContainer.nameText;

            data.currentDialogue = dialogueText.text;
            data.dialogueFont = FilePaths.resources_fonts + dialogueText.font.name;
            data.dialogueColor = dialogueText.color;
            data.dialogueFontSize = dialogueText.fontSize;

            data.currentSpeaker = nameText.text;
            data.speakerFont = FilePaths.resources_fonts + nameText.font.name;
            data.speakerNameColor = nameText.color;
            data.speakerFontSize = nameText.fontSize;

            return data;
        }

        public static void Apply(DialogueData data)
        {
            var ds = DialogueSystem.instance;
            var dialogueText = ds.dialogueContainer.dialogueText;
            var nameText = ds.dialogueContainer.nameContainer.nameText;

            dialogueText.text = data.currentDialogue;
            dialogueText.color = data.dialogueColor;
            dialogueText.fontSize = data.dialogueFontSize;

            nameText.text = data.currentSpeaker;
            nameText.color = data.speakerNameColor;
            nameText.fontSize = data.speakerFontSize;

            if (data.dialogueFont != dialogueText.font.name)
            {
                TMP_FontAsset fontAsset = HistoryCache.LoadFont(data.dialogueFont);
                if (fontAsset != null)
                {
                    dialogueText.font = fontAsset;
                }
            }

            if (data.speakerFont != nameText.font.name)
            {
                TMP_FontAsset fontAsset = HistoryCache.LoadFont(data.speakerFont);
                if (fontAsset != null)
                {
                    nameText.font = fontAsset;
                }
            }
        }
    }
}