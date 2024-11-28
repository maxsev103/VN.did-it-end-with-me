using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CHARACTERS;

namespace DIALOGUE
{
    public class DialogueSystem : MonoBehaviour
    {
        [SerializeField] private DialogueSystemConfiguration_SO _config;
        public DialogueSystemConfiguration_SO config => _config;

        public DialogueContainer dialogueContainer = new DialogueContainer();
        public ConversationManager conversationManager { get; private set; }
        private TextArchitect architect;
        private AutoReader autoReader;
        [SerializeField] private CanvasGroup mainCanvas;

        public static DialogueSystem instance { get; private set; }

        public delegate void DialogueSystemEvent();
        public event DialogueSystemEvent onUserPrompt_Next;

        public bool isRunningConversation => conversationManager.isRunning;

        public DialogueContinuationPrompt prompt;
        private CanvasGroupController cgController;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                Initialize();
            }
            else
                DestroyImmediate(gameObject);
        }

        bool _initialized = false;
        private void Initialize()
        {
            if (_initialized) 
                return;

            architect = new TextArchitect(dialogueContainer.dialogueText);
            conversationManager = new ConversationManager(architect);

            cgController = new CanvasGroupController(this, mainCanvas);
            dialogueContainer.Initialize();

            if (TryGetComponent(out autoReader))
                autoReader.Initialize(conversationManager);
        }

        public void OnUserPromt_Next()
        {
            onUserPrompt_Next?.Invoke();

            if (autoReader != null && autoReader.isOn)
                autoReader.Disable();
        }

        public void OnSystemPrompt_Next()
        {
            onUserPrompt_Next?.Invoke();
        }

        public void ApplySpeakerDataToContainer(string speakerName)
        {
            Character character = CharacterManager.instance.GetCharacter(speakerName);
            CharacterConfig_Data config = character != null ? character.config : CharacterManager.instance.GetCharacterConfig(speakerName);

            dialogueContainer.SetDialogueColor(config.dialogueColor);
            dialogueContainer.SetDialogueFont(config.dialogueFont);
            dialogueContainer.nameContainer.SetNameColor(config.nameColor);
            dialogueContainer.nameContainer.SetNameFont(config.nameFont);
        }

        public void ApplySpeakerDataToContainer(CharacterConfig_Data config)
        {
            dialogueContainer.SetDialogueColor(config.dialogueColor);
            dialogueContainer.SetDialogueFont(config.dialogueFont);
            dialogueContainer.SetDialogueFontSize(config.dialoguefontSize * this.config.defaultFontScale);
            dialogueContainer.nameContainer.SetNameColor(config.nameColor);
            dialogueContainer.nameContainer.SetNameFont(config.nameFont);
            dialogueContainer.nameContainer.SetNameFontSize(config.namefontSize);

        }

        public void ShowSpeakerName(string speakerName = "")
        {
            if (speakerName.ToLower() != "narrator")
                dialogueContainer.nameContainer.Show(speakerName);
            else
            {
                HideSpeakerName();
                dialogueContainer.nameContainer.nameText.text = "";
            }
        }

        public void HideSpeakerName() => dialogueContainer.nameContainer.Hide();

        public Coroutine Say(string speaker, string dialogue)
        {
            List<string> conversation = new List<string>() { $"{speaker} \"{dialogue}\"" };
            return Say(conversation);
        }

        public Coroutine Say(List<string> lines)
        {
            Conversation conversation = new Conversation(lines);
            return conversationManager.StartConversation(conversation);
        }

        public Coroutine Say(Conversation conversation)
        {
            return conversationManager.StartConversation(conversation);
        }

        public bool isVisible => cgController.isVisible;
        public Coroutine Show(float speed = 1f, bool immediate = false) => cgController.Show(speed, immediate);
        public Coroutine Hide(float speed = 1f, bool immediate = false) => cgController.Hide(speed, immediate);
    }
}