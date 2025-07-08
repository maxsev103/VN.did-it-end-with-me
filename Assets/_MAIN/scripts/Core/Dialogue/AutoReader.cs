using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace DIALOGUE
{
    public class AutoReader : MonoBehaviour
    {
        private const int DEFAULT_CHARACTERS_READ_PER_SECOND = 18;
        private const float READ_TIME_PADDING = 0.5f;
        private const float MIN_READ_TIME = 2f;
        private const float MAX_READ_TIME = 99f;
        private const string STATUS_TEXT_AUTO = "Auto";
        private const string STATUS_TEXT_SKIP = "Skip";

        private ConversationManager conversationManager;
        private TextArchitect architect => conversationManager.architect;

        private Coroutine co_running = null;
        public bool isOn => co_running != null;

        public bool skip { get; set; } = false;
        public float speed { get; set; } = 1f;

        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private Animator anim;
        [SerializeField] public Button autoButton;
        [SerializeField] public Button skipButton;

        [HideInInspector] public bool allowToggle = true;

        public void Initialize(ConversationManager conversationManager)
        {
            this.conversationManager = conversationManager;
            statusText.text = string.Empty;
        }

        public void Enable()
        {
            if (isOn)
                return;

            autoButton.interactable = true;
            skipButton.interactable = true;
            anim.gameObject.SetActive(true);
            co_running = StartCoroutine(AutoRead());
        }
        
        public void Disable()
        {
            if (!isOn)
                return;

            StopCoroutine(co_running);
            skip = false;
            co_running = null;
            anim.gameObject.SetActive(false);
            statusText.text = string.Empty;
        }

        private IEnumerator AutoRead()
        {
            if (!conversationManager.isRunning)
            {
                Disable();
                yield break;
            }

            if (!architect.isBuilding && architect.currentText != string.Empty)
                DialogueSystem.instance.OnSystemPrompt_Next();

            while (conversationManager.isRunning)
            {
                bool shouldBreak = false;
                // read and wait
                if (!skip)
                {
                    while (!architect.isBuilding && !conversationManager.isWaitingOnAutoTimer)
                    {
                        // Check if skip has been enabled
                        if (skip)
                        {
                            shouldBreak = true;
                            break;
                        }
                        yield return null;
                    }

                    if (shouldBreak) continue;

                    float timeStarted = Time.time;

                    while (architect.isBuilding || conversationManager.isWaitingOnAutoTimer)
                    {
                        // Check if skip has been enabled
                        if (skip)
                        {
                            shouldBreak = true;
                            break;
                        }
                        yield return null;
                    }

                    if (shouldBreak) continue;

                    float timeToRead = Mathf.Clamp(((float) architect.tmpro.textInfo.characterCount / DEFAULT_CHARACTERS_READ_PER_SECOND), MIN_READ_TIME, MAX_READ_TIME);
                    timeToRead = Mathf.Clamp((timeToRead - (Time.time - timeStarted)), MIN_READ_TIME, MAX_READ_TIME);
                    timeToRead = (timeToRead / speed) + READ_TIME_PADDING;

                    yield return new WaitForSeconds(timeToRead);
                }
                // skip
                else
                {
                    architect.ForceComplete();

                    yield return null;
                }

                DialogueSystem.instance.OnSystemPrompt_Next();
            }

            Disable();
        }

        public void ToggleAuto()
        {
            if (!allowToggle)
                return;

            if (skip)
                Enable();
            else
            {
                if (!isOn)
                    Enable();
                else 
                {
                    autoButton.OnDeselect(null);
                    Disable();
                }
            }

            skip = false;
            if (isOn)
                statusText.text = STATUS_TEXT_AUTO;
        }

        public void ToggleSkip()
        {
            if (!allowToggle)
                return;

            if (!skip)
                Enable();
            else
            {
                if (!isOn)
                    Enable();
                else
                {
                    skipButton.OnDeselect(null);
                    Disable();
                }
            }

            skip = true;
            if (isOn)
                statusText.text = STATUS_TEXT_SKIP;
        }
    }
}