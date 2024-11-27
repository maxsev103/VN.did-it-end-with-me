using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DIALOGUE
{
    public class DialogueLine
    {
        public string rawData { get; private set; } = string.Empty;
        public DL_SpeakerData speakerData;
        public DL_DialogueData dialogueData;
        public DL_CommandData commandData;

        public bool hasDialogue => dialogueData != null;
        public bool hasCommands => commandData != null;
        public bool hasSpeaker => speakerData != null;

        public DialogueLine(string rawLine, string speaker, string dialogue, string commands)
        {
            rawData = rawLine;
            this.speakerData = (string.IsNullOrWhiteSpace(speaker) ? null : new DL_SpeakerData(speaker));
            this.dialogueData = (string.IsNullOrWhiteSpace(dialogue) ? null : new DL_DialogueData(dialogue));
            this.commandData = (string.IsNullOrWhiteSpace(commands) ? null : new DL_CommandData(commands));
        }
    }
}