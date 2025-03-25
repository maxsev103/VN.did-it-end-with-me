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

        public float dialogueBoxAlpha;
        public float nameBoxAlpha;

        public bool isOnLogicalLine = false;
        public bool isWaitingForUserChoice = false;

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

            data.dialogueBoxAlpha = ds.dialogueContainer.dialogueBoxAlpha;
            data.nameBoxAlpha = ds.dialogueContainer.nameContainer.nameBoxAlpha;

            data.isOnLogicalLine = ds.conversationManager.isOnLogicalLine;

            return data;
        }

        public static void Apply(DialogueData data)
        {
            var ds = DialogueSystem.instance;
            var dialogueText = ds.dialogueContainer.dialogueText;
            var nameText = ds.dialogueContainer.nameContainer.nameText;

            ds.conversationManager.architect.SetText(data.currentDialogue);
            dialogueText.color = data.dialogueColor;
            dialogueText.fontSize = data.dialogueFontSize;

            ds.dialogueContainer.SetDialogueBoxAlpha(data.dialogueBoxAlpha);
            ds.dialogueContainer.nameContainer.SetNameBoxAlpha(data.nameBoxAlpha);

            ds.conversationManager.isOnLogicalLine = data.isOnLogicalLine;

            nameText.text = data.currentSpeaker;
            if (nameText.text != string.Empty)
                ds.dialogueContainer.nameContainer.Show();
            else
                ds.dialogueContainer.nameContainer.Hide();

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