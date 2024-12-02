using CHARACTERS;
using COMMANDS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DIALOGUE.LogicalLines;

namespace DIALOGUE
{
    public class ConversationManager
    {
        private DialogueSystem dialogueSystem => DialogueSystem.instance;
        private Coroutine process = null;
        public bool isRunning => process != null;
        public bool isOnLogicalLine { get; set; } = false;

        public TextArchitect architect = null;
        private bool userPrompt = false;

        private LogicalLineManager logicalLineManager;

        public Conversation conversation => (conversationQueue.IsEmpty() ? null : conversationQueue.top);
        public int conversationProgress => (conversationQueue.IsEmpty() ? -1 : conversationQueue.top.GetProgress());
        private ConversationQueue conversationQueue;

        public bool allowUserPrompts = true;

        public ConversationManager(TextArchitect architect)
        {
            this.architect = architect;
            dialogueSystem.onUserPrompt_Next += OnUserPrompt_Next;

            logicalLineManager = new LogicalLineManager();

            conversationQueue = new ConversationQueue();
        }

        public Conversation[] GetConversationQueue() => conversationQueue.GetReadOnlyQueue();

        public void Enqueue(Conversation conversation) => conversationQueue.Enqueue(conversation);
        public void EnqueuePriority(Conversation conversation) => conversationQueue.EnqueuePriority(conversation);

        private void OnUserPrompt_Next()
        {
            if (allowUserPrompts)
                userPrompt = true;
        }
        
        public Coroutine StartConversation(Conversation conversation)
        {
            StopConversation();
            conversationQueue.Clear();

            Enqueue(conversation);

            process = dialogueSystem.StartCoroutine(RunningConversation());
            
            return process;
        }

        public void StopConversation()
        {
            if (!isRunning)
                return;

            dialogueSystem.StopCoroutine(process);
            process = null;
        }

        IEnumerator RunningConversation()
        {
            while (!conversationQueue.IsEmpty())
            {
                Conversation currentConversation = conversation;

                if (currentConversation.HasReachedEnd())
                {
                    conversationQueue.Dequeue();
                    continue;
                }

                string rawLine = conversation.GetCurrentLine();

                // dont show any blank lines or try to run logic on them
                if (string.IsNullOrWhiteSpace(rawLine))
                {
                    TryAdvanceConversation(currentConversation);
                    continue;
                }

                DialogueLine line = DialogueParser.Parse(rawLine);

                if (logicalLineManager.TryGetLogic(line, out Coroutine logic))
                {
                    isOnLogicalLine = true;
                    yield return logic;
                }
                else
                {        // show dialogue
                    if (line.hasDialogue)
                        yield return Line_RunDialogue(line);

                    // run commands if any
                    if (line.hasCommands)
                        yield return Line_RunCommands(line);

                    // wait for user input
                    if (line.hasDialogue)
                    {
                        // wait for user input
                        yield return WaitForUserInput();

                        CommandManager.instance.StopAllProcesses();

                        dialogueSystem.OnSystemPrompt_Clear();
                    }
                }

                TryAdvanceConversation(currentConversation);
                isOnLogicalLine = false;
            }

            process = null;
        }

        private void TryAdvanceConversation(Conversation conversation)
        {
            conversation.IncrementProgress();

            if (conversation != conversationQueue.top)
                return;

            if (conversation.HasReachedEnd())
                conversationQueue.Dequeue();
        }

        IEnumerator Line_RunDialogue(DialogueLine line)
        {
            // handle the speaker logic from the txt files such as name and position casting
            if (line.hasSpeaker)
                HandleSpeakerLogic(line.speakerData);

            // show the dialogue box if it is currently hidden
            if (!dialogueSystem.dialogueContainer.isVisible)
                dialogueSystem.dialogueContainer.Show();

            // build the dialogue
            yield return BuildLineSegments(line.dialogueData);
        }

        private void HandleSpeakerLogic(DL_SpeakerData speakerData)
        {
            bool characterMustBeCreated = (speakerData.makeCharacterEnter || speakerData.isCastingPosition || speakerData.isCastingExpressions);

            Character character = CharacterManager.instance.GetCharacter(speakerData.name, createIfNotExisting: characterMustBeCreated);

            // if we are trying to make a character enter the scene, check if the character has been created or not
            // if not, then create them. otherwise, just reveal them again
            if (speakerData.makeCharacterEnter && (!character.isVisible && !character.isRevealing)) {

                if (speakerData.makeCharacterEnterLeft)
                    character.SetPosition(new Vector2(-0.5f, 0));
                else if (speakerData.makeCharacterEnterRight)
                    character.SetPosition(new Vector2(1.5f, 0));

                character.Show();
            }

            // add character name to UI
            dialogueSystem.ShowSpeakerName(TagManager.Inject(speakerData.displayName));

            // customize dialogue look and feel according to the character config
            DialogueSystem.instance.ApplySpeakerDataToContainer(speakerData.name);

            if (speakerData.isCastingPosition)
            {
                character.MoveToPosition(speakerData.castPosition);
            }

            if (speakerData.isCastingExpressions)
            {
                foreach (var ce in speakerData.CastExpressions)
                    character.OnReceiveCastingExpression(ce.layer, ce.expression);
            }
        }

        IEnumerator Line_RunCommands(DialogueLine line)
        {
            List<DL_CommandData.Command> commands = line.commandData.commands;

            foreach (DL_CommandData.Command command in commands)
            {
                if (command.waitForCompletion || command.name == "wait") 
                {
                    CoroutineWrapper cw = CommandManager.instance.Execute(command.name, command.arguments);
                    while (!cw.isDone)
                    {
                        if (userPrompt) 
                        {
                            CommandManager.instance.StopCurrentProcess();
                            userPrompt = false;
                        }

                        yield return null;
                    }
                }
                else
                    CommandManager.instance.Execute(command.name, command.arguments);
            }

            yield return null;
        }

        IEnumerator BuildLineSegments(DL_DialogueData line)
        {
            for (int i = 0; i < line.segments.Count; i++)
            {
                DL_DialogueData.DialogueSegment segment = line.segments[i];

                yield return WaitForDialogueSegmentSignalTrigger(segment);

                yield return BuildDialogue(segment.dialogue, segment.appendText);
            }
        }

        public bool isWaitingOnAutoTimer { get; private set; } = false;
        IEnumerator WaitForDialogueSegmentSignalTrigger(DL_DialogueData.DialogueSegment segment)
        {
            switch (segment.startSignal)
            {
                case DL_DialogueData.DialogueSegment.StartSignal.C:
                    yield return WaitForUserInput();
                    dialogueSystem.OnSystemPrompt_Clear();
                    break;
                case DL_DialogueData.DialogueSegment.StartSignal.A:
                    yield return WaitForUserInput();
                    break;
                case DL_DialogueData.DialogueSegment.StartSignal.WC:
                    isWaitingOnAutoTimer = true;
                    yield return new WaitForSeconds(segment.signalDelay);
                    isWaitingOnAutoTimer = false;
                    dialogueSystem.OnSystemPrompt_Clear();
                    break;
                case DL_DialogueData.DialogueSegment.StartSignal.WA:
                    isWaitingOnAutoTimer = true;
                    yield return new WaitForSeconds(segment.signalDelay);
                    isWaitingOnAutoTimer = false;
                    break;
                default:
                    break;
            }
        }

        IEnumerator BuildDialogue(string dialogue, bool append = false)
        {
            dialogue = TagManager.Inject(dialogue);

            if (!append)
                architect.Build(dialogue);
            else
                architect.Append(dialogue);

            while (architect.isBuilding)
            {
                if (userPrompt)
                {
                    if (!architect.hurryUp)
                        architect.hurryUp = true;
                    else
                        architect.ForceComplete();

                    userPrompt = false;
                }

                yield return null;
            }
        }
        
        IEnumerator WaitForUserInput()
        {
            yield return new WaitForSeconds(0.05f);

            dialogueSystem.prompt.Show();

            while (!userPrompt)
                yield return null;

            dialogueSystem.prompt.Hide();

            userPrompt = false;
        }
    }
}