using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DIALOGUE;

namespace HISTORY
{
    public class HistoryLogManager : MonoBehaviour
    {
        public static HistoryLogManager instance { get; private set; }

        private const float LOG_STARTING_HEIGHT = 2f;
        private const float LOG_HEIGHT_PER_LINE = 1f;
        private const float DEFAULT_HEIGHT = 1f;
        private const float TEXT_DEFAULT_SCALE = 1f;

        private const string NAMETEXT_NAME = "Speaker";
        private const string DIALOGUETEXT_NAME = "Dialogue";

        private const float DEFAULT_LOG_FONT_SIZE = 35f;

        public float logScaling = 1f;

        [SerializeField] private Animator anim;
        [SerializeField] private GameObject logPrefab;

        [SerializeField] private AutoReader autoReader;
        HistoryManager manager => HistoryManager.instance;
        private List<HistoryLog> logs = new List<HistoryLog>();

        public bool isOpen { get; private set; } = false;

        [SerializeField] private Slider logScaleSlider;

        private float textScaling => logScaling * 3f;

        private void Awake()
        {
            instance = this;
        }

        public void Open()
        {
            if (isOpen) 
                return;

            anim.Play("Open");
            isOpen = true;

            if (autoReader.isOn)
                autoReader.Disable();
        }

        public void Close()
        {
            if (!isOpen)
                return;

            anim.Play("Close");
            isOpen = false;
        }

        public void AddLog(HistoryState state)
        {
            if (logs.Count >= HistoryManager.HISTORY_CACHE_LIMIT)
            {
                DestroyImmediate(logs[0].container);
                logs.RemoveAt(0);
            }

            CreateLog(state);
        }

        private void CreateLog(HistoryState state)
        {
            HistoryLog log = new HistoryLog();

            log.container = Instantiate(logPrefab, logPrefab.transform.parent);
            log.container.SetActive(true);

            log.nameText = log.container.transform.Find(NAMETEXT_NAME).GetComponent<TextMeshProUGUI>();
            log.dialogueText = log.container.transform.Find(DIALOGUETEXT_NAME).GetComponent<TextMeshProUGUI>();

            // add the speaker name data
            if (state.dialogue.currentSpeaker == string.Empty)
            {
                log.nameText.text = string.Empty;
            }
            else
            {
                log.nameText.text = state.dialogue.currentSpeaker;
                log.nameText.font = HistoryCache.LoadFont(state.dialogue.speakerFont);
                log.nameText.color = state.dialogue.speakerNameColor;
                log.nameFontSize = TEXT_DEFAULT_SCALE * DEFAULT_LOG_FONT_SIZE;
                log.nameText.fontSize = log.nameFontSize + textScaling;
            }

            // add the dialogue data
            log.dialogueText.text = state.dialogue.currentDialogue;
            log.dialogueText.font = HistoryCache.LoadFont(state.dialogue.dialogueFont);
            log.dialogueText.color = state.dialogue.dialogueColor;
            log.dialogueFontSize = TEXT_DEFAULT_SCALE * DEFAULT_LOG_FONT_SIZE;
            log.dialogueText.fontSize = log.dialogueFontSize + textScaling;

            FitLogToText(log);

            logs.Add(log);
        }

        private void FitLogToText(HistoryLog log)
        {
            RectTransform rect = log.dialogueText.GetComponent<RectTransform>();
            ContentSizeFitter textCSF = log.dialogueText.GetComponent<ContentSizeFitter>();

            textCSF.SetLayoutVertical();

            LayoutElement logLayout = log.container.GetComponent<LayoutElement>();
            float height = rect.rect.height;

            float perc = height / DEFAULT_HEIGHT;
            float extraScale = (LOG_HEIGHT_PER_LINE * perc) - LOG_HEIGHT_PER_LINE;
            float scale = LOG_STARTING_HEIGHT + extraScale;

            logLayout.preferredHeight = scale + textScaling;

            logLayout.preferredHeight += 2f * logScaling;
        }

        public void SetLogScaling()
        {
            logScaling = logScaleSlider.value;

            foreach (HistoryLog log in logs)
            {
                log.nameText.fontSize = log.nameFontSize + textScaling;
                log.dialogueText.fontSize = log.dialogueFontSize + textScaling;

                FitLogToText(log);
            }
        }

        public void Clear()
        {
            for (int i = 0; i < logs.Count; i++)
                DestroyImmediate(logs[i].container);

            logs.Clear();
        }

        internal void Rebuild()
        {
            foreach (var state in manager.history)
            {
                CreateLog(state);
            }
        }
    }
}