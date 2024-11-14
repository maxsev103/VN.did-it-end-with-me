using System.Collections;
using UnityEngine;
using TMPro;
using System.Linq;
public class TextArchitect
{
    private TextMeshProUGUI tmpro_ui; // for ui space text
    private TextMeshPro tmpro_world; // world space text
    public TMP_Text tmpro => tmpro_ui != null ? tmpro_ui : tmpro_world; // get whichever space text is assigned to text architect

    public string currentText => tmpro.text; // get the text from tmpro variable
    public string targetText { get; private set; } = ""; // initialize to empty
    public string preText { get; private set; } = ""; // initialize to empty
    private int preTextLength = 0; // initialize to 0

    public string fullTargetText => preText + targetText; // full target text i.e. what we want to fully display

    public enum BuildMethod { instant, typewriter }; // tracks the different text animation styles
    public BuildMethod buildMethod = BuildMethod.typewriter; // set default build method to typewriter style

    /// <summary>
    /// gets the color of whatever text was assigned. this is important to manipulate alpha values for the text animation
    /// since this simply reveals text character by character as opposed to writing to the screen character by character
    /// </summary>
    public Color textColor { get { return tmpro.color; } set { tmpro.color = value; } }

    /// <summary>
    /// variables for text animation speed, with standard speed being the one changed at runtime
    /// </summary>
    public float standardSpeed { get { return baseSpeed * speedMultiplier; } set { speedMultiplier = value; } }
    private const float baseSpeed = 1;
    private float speedMultiplier = 1;

    /// <summary>
    /// variables to decide how many characters per frame are loaded in
    /// </summary>
    public int charactersPerCycle { get { return standardSpeed <= 2f ? characterMultiplier : speedMultiplier <= 2.5f ? characterMultiplier * 2 : characterMultiplier * 3; } }
    private int characterMultiplier = 1;

    // boolean to check for if the player wants to hurry the text animation
    public bool hurryUp = false;

    /// <summary>
    /// constructors for the TextArchitext class
    /// </summary>
    /// <param name="tmpro_ui"></param>
    public TextArchitect(TextMeshProUGUI tmpro_ui)
    {
        this.tmpro_ui = tmpro_ui;
    }

    public TextArchitect(TextMeshPro tmpro_world)
    {
        this.tmpro_world = tmpro_world;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public Coroutine Build(string text)
    {
        preText = "";
        targetText = text;

        Stop();

        buildProcess = tmpro.StartCoroutine(Building());
        return buildProcess;
    }

    /// <summary>
    /// append text to what is already in text architect
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public Coroutine Append(string text)
    {
        preText = tmpro.text;
        targetText = text;

        Stop();

        buildProcess = tmpro.StartCoroutine(Building());
        return buildProcess;
    }

    // track if coroutine building is running
    private Coroutine buildProcess = null;
    public bool isBuilding => buildProcess != null;

    /// <summary>
    /// method to stop a running build process
    /// </summary>
    public void Stop()
    {
        if (!isBuilding)
            return;

        tmpro.StopCoroutine(buildProcess);
        buildProcess = null;
    }

    IEnumerator Building()
    {
        Prepare();

        switch (buildMethod)
        {
            case BuildMethod.typewriter:
                yield return Build_Typewriter();
                break;
            case BuildMethod.instant:
                break;
        }
    }

    private void OnComplete()
    {
        buildProcess = null;
        hurryUp = false;
    }

    public void ForceComplete()
    {
        switch (buildMethod)
        {
            case BuildMethod.typewriter:
                tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
                break;
        }

        Stop();
        OnComplete();
    }

    private void Prepare()
    {
        switch (buildMethod)
        {
            case BuildMethod.instant:
                Prepare_Instant();
                break;
            case BuildMethod.typewriter:
                Prepare_Typewriter();
                break;
        }
    }

    /// <summary>
    /// this method forces tmpro to update the text color to what it was without changes (effectively canceling any
    /// text building methods like typewriter or fade) and makes the full text fully visible
    /// </summary>
    private void Prepare_Instant()
    {
        tmpro.color = tmpro.color;
        tmpro.text = fullTargetText;
        tmpro.ForceMeshUpdate();
        tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
    }

    private void Prepare_Typewriter()
    {
        tmpro.color = tmpro.color;
        tmpro.maxVisibleCharacters = 0;
        tmpro.text = preText;

        if (preText != "")
        {
            tmpro.ForceMeshUpdate();
            tmpro.maxVisibleCharacters = tmpro.textInfo.characterCount;
        }

        tmpro.text += targetText;
        tmpro.ForceMeshUpdate();
    }

    private IEnumerator Build_Typewriter()
    {
        while (tmpro.maxVisibleCharacters < tmpro.textInfo.characterCount)
        {
            tmpro.maxVisibleCharacters += hurryUp ? charactersPerCycle * 5 : charactersPerCycle;

            yield return new WaitForSeconds(0.015f / standardSpeed);
        }
    }

    
}
