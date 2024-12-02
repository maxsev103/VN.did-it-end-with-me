using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static DIALOGUE.LogicalLines.LogicalLineUtils.Encapsulation;
using static DIALOGUE.LogicalLines.LogicalLineUtils.Conditions;

namespace DIALOGUE.LogicalLines
{
    public class LL_Condition : ILogicalLine
    {
        public string keyword => "if";
        public const string ELSE = "else";
        private readonly string[] CONTAINERS = new string[] { "(", ")" };
 
        public IEnumerator Execute(DialogueLine line)
        {
            string rawCondition = ExtractCondition(line.rawData.Trim());
            bool conditionResult = EvaluateCondition(rawCondition);

            Conversation currentConversation = DialogueSystem.instance.conversationManager.conversation;
            int currentProgress = DialogueSystem.instance.conversationManager.conversationProgress;

            EncapsulatedData ifData = RipEncapsulatedData(currentConversation, currentProgress, ripHeaderAndEncapsulators: false, parentStartingIndex: currentConversation.fileStartIndex);
            EncapsulatedData elseData = new EncapsulatedData();

            if (ifData.endingIndex + 1 < currentConversation.Count)
            {
                string nextLine = currentConversation.GetLines()[ifData.endingIndex + 1].Trim();
                if (nextLine == ELSE)
                {
                    elseData = RipEncapsulatedData(currentConversation, ifData.endingIndex + 1, ripHeaderAndEncapsulators: false, parentStartingIndex: currentConversation.fileStartIndex);
                }
            }

            currentConversation.SetProgress(elseData.isNull ? ifData.endingIndex : elseData.endingIndex);

            EncapsulatedData selectedData = conditionResult ? ifData : elseData;

            if (!selectedData.isNull && selectedData.lines.Count > 0)
            {
                // remove the header and encapsulator lines from the conversation indices
                selectedData.startingIndex += 2; // remove header and starting encapsulator
                selectedData.endingIndex -= 1; // remove the ending encapsulator

                Conversation newConversation = new Conversation(selectedData.lines, file: currentConversation.file, fileStartIndex: selectedData.startingIndex, fileEndIndex: selectedData.endingIndex);
                DialogueSystem.instance.conversationManager.EnqueuePriority(newConversation);
            }

            yield return null;
        }

        public bool Matches(DialogueLine line)
        {
            return line.rawData.Trim().StartsWith(keyword);
        }

        private string ExtractCondition(string line)
        {
            int startIndex = line.IndexOf(CONTAINERS[0]) + 1;
            int endIndex = line.IndexOf(CONTAINERS[1]);

            return line.Substring(startIndex, endIndex - startIndex).Trim();
        }
    }
}