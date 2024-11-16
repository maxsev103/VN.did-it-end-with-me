using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DIALOGUE
{
    public class DialogueLine
    {
        public string speaker;
        public DL_DialogueData dialogue;
        public string commands;

        public bool hasDialogue => dialogue.hasDialogue;
        public bool hasCommands => commands != string.Empty;
        public bool hasSpeaker => speaker != string.Empty;

        public DialogueLine(string speaker, string dialogue, string commands)
        {
            this.speaker = speaker;
            this.dialogue = new DL_DialogueData(dialogue);
            this.commands = commands;
        }
    }
}