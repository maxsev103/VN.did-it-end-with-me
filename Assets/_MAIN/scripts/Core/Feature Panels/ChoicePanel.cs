using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DIALOGUE;

public class ChoicePanel : MonoBehaviour
{
    private const float BUTTON_HEIGHT_PER_LINE = 25f;
    private const float BASE_BUTTON_HEIGHT = 100f;

    public static ChoicePanel instance { get; private set; }

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject choiceButtonPrefab;
    [SerializeField] private VerticalLayoutGroup buttonLayoutGroup;

    private CanvasGroupController cg = null;
    private DialogueSystem dialogueSystem => DialogueSystem.instance;
    private List<ChoiceButton> buttons = new List<ChoiceButton>();
    public ChoicePanelDecision lastDecision { get; private set; } = null;
    public bool isWaitingForUserChoice { get; private set; } = false;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        cg = new CanvasGroupController(this, canvasGroup);

        cg.alpha = 0;
        cg.SetInteractableState(false);
    }

    public void Show(string question, string[] choices)
    {
        lastDecision = new ChoicePanelDecision(choices);
        isWaitingForUserChoice = true;

        cg.Show();
        cg.SetInteractableState(active: true);

        dialogueSystem.dialogueContainer.nameContainer.Hide();
        dialogueSystem.dialogueContainer.dialogueText.text = question;
        StartCoroutine(GenerateChoices(choices));
    }

    private IEnumerator GenerateChoices(string[] choices)
    {
        for (int i = 0; i < choices.Length; i++)
        {
            ChoiceButton choiceButton;
            if (i < buttons.Count)
            {
                choiceButton = buttons[i];
            }
            else
            {
                GameObject newButtonObject = Instantiate(choiceButtonPrefab, buttonLayoutGroup.transform);
                newButtonObject.SetActive(true);

                Button newButton = newButtonObject.GetComponent<Button>();
                TextMeshProUGUI newChoiceText = newButton.GetComponentInChildren<TextMeshProUGUI>();
                LayoutElement newLayout = newButton.GetComponent<LayoutElement>();

                choiceButton = new ChoiceButton { button = newButton, layout = newLayout, choiceText = newChoiceText };

                buttons.Add(choiceButton);
            }

            choiceButton.button.onClick.RemoveAllListeners();
            int buttonIndex = i;
            choiceButton.button.onClick.AddListener(() => AcceptAnswer(buttonIndex));
            choiceButton.choiceText.text = choices[i];
        }

        yield return new WaitForEndOfFrame();

        // resize the buttons if needed
        foreach (var button in buttons)
        {
            button.choiceText.ForceMeshUpdate();
            int lines = button.choiceText.textInfo.lineCount;
            button.layout.preferredHeight = lines <= 1 ? BASE_BUTTON_HEIGHT : BASE_BUTTON_HEIGHT + (BUTTON_HEIGHT_PER_LINE * lines);
        }

        yield return new WaitForEndOfFrame();

        for (int i = 0; i < buttons.Count; i++)
        {
            bool show = i < choices.Length;
            buttons[i].button.gameObject.SetActive(show);
        }

    }

    public void Hide()
    {
        cg.Hide();
    }
    
    private void AcceptAnswer(int index)
    {
        if (index < 0 || index > lastDecision.choices.Length - 1)
            return;

        lastDecision.answerIndex = index;
        isWaitingForUserChoice = false;
        Hide();
    }

    public class ChoicePanelDecision
    {
        public string question = string.Empty;
        public int answerIndex = -1;
        public string[] choices = new string[0];

        public ChoicePanelDecision(string[] choices)
        {
            this.choices = choices;
            answerIndex = -1;
        }
    }

    private struct ChoiceButton
    {
        public Button button;
        public LayoutElement layout;
        public TextMeshProUGUI choiceText;
    }
}
