using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DIALOGUE
{
    public class DialogueParser
    {
        private const string commandRegexPattern = @"[\w\[\]]*[^\s]\(";

        public static DialogueLine Parse(string rawLine)
        {
            (string speaker, string dialogue, string commands) = RipContent(rawLine);
            
            return new DialogueLine(speaker, dialogue, commands);
        }

        /// <summary>
        /// Rips the different segments from a raw line from a text file. Will differentiate between speaker, dialogue,
        /// and commands, and will return the ripped content as a tuple of strings
        /// </summary>
        /// <param name="rawLine"></param>
        /// <returns></returns>
        private static (string, string, string) RipContent(string rawLine)
        {
            string speaker = "", dialogue = "", commands="";

            int dialogueStart = -1; // sets the indices for when dialogue starts or ends
            int dialogueEnd = -1;
            bool isEscaped = false;

            for (int i = 0; i < rawLine.Length; i++)
            {
                char current = rawLine[i];
                // this checks if the next character is an escaped character
                if (current == '\\')
                {
                    isEscaped = !isEscaped;
                } 
                // otherwise checks if we've hit non-escaped quotation marks a.k.a. dialogue
                else if (current == '"' && !isEscaped)
                {
                    // if this is the start of the dialogue line
                    if (dialogueStart == -1)
                        dialogueStart = i;
                    // otherwise, check if it is the end of the dialogue, and break out of the loop
                    // if true
                    else if (dialogueEnd == -1)
                    {
                        dialogueEnd = i;
                        break;
                    }
                }
                // default, reset the isEscaped boolean
                else
                    isEscaped = false;
            }

            // identify command pattern
            Regex commandRegex = new Regex(commandRegexPattern);
            // look for all the matches using the command pattern
            MatchCollection matches = commandRegex.Matches(rawLine);

            int commandStart = -1;
            foreach (Match match in matches)
            {
                // this checks if this is a line of only commands and no dialogue was found with it
                if (match.Index < dialogueStart ||  match.Index > dialogueEnd)
                {
                    commandStart = match.Index;
                    break;
                }
            }

            // returns only the command line if this is a line with only commands on it
            if (commandStart != -1 && (dialogueStart == -1 && dialogueEnd == -1))
                return ("", "", rawLine.Trim());

            // check if the parsed string in quotes is dialogue or part of a command by checking if dialogue has been detected
            // and if no command was detected (commandStart == -1 || commandStart > dialogueEnd)
            if (dialogueStart != -1 && dialogueEnd != -1 && (commandStart == -1 || commandStart > dialogueEnd))
            {
                // no dialogue has yet been captured so get this dialogue
                speaker = rawLine.Substring(0, dialogueStart).Trim();
                dialogue = rawLine.Substring(dialogueStart + 1, dialogueEnd - dialogueStart - 1).Replace("\\\"", "\"");

                // check if a command has been parsed and assign it to commands if so
                if (commandStart != -1)
                    commands = rawLine.Substring(commandStart).Trim();
            }
            else if (commandStart != -1 && dialogueStart > commandStart)
                commands = rawLine;
            else
                dialogue = rawLine;

            return (speaker, dialogue, commands);
        }
    }
}