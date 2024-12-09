using System.Collections;
using UnityEngine;
using VISUALNOVEL;

public class MainMenu : MonoBehaviour
{
    public const string MAIN_MENU_SCENE = "MainMenu";

    public static MainMenu instance { get; private set; }

    public AudioClip menuMusic;
    public CanvasGroup mainPanel;
    private CanvasGroupController mainCG;

    private UIConfirmationMenu uiChoiceMenu => UIConfirmationMenu.instance;

    public GameObject loadGame;
    public GameObject loadGameSpacer;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        mainCG = new CanvasGroupController(this, mainPanel);

        AudioManager.instance.StopAllTracks();
        AudioManager.instance.StopAllSoundEffects();
        AudioManager.instance.PlayTrack(menuMusic, channel: 0, startingVolume: 0.7f);

        HideLoadOptionOnNoSaveFiles();
    }

    private void Update()
    {
        HideLoadOptionOnNoSaveFiles();
    }

    public void ClickStartNewGame()
    {
        uiChoiceMenu.Show("Do you want to start a new game?", new UIConfirmationMenu.ConfirmationButton("Yes", StartNewGame), new UIConfirmationMenu.ConfirmationButton("No", null));
    }

    public void ClickStartNewGameMobile()
    {
        uiChoiceMenu.Show("Do you want to start a new game?", new UIConfirmationMenu.ConfirmationButton("Yes", StartNewGameMobile), new UIConfirmationMenu.ConfirmationButton("No", null));
    }

    private void StartNewGame()
    {
        VNGameSave.activeFile = new VNGameSave();
        StartCoroutine(StartingGame());
    }

    public void StartNewGameMobile()
    {
        VNGameSave.activeFile = new VNGameSave();
        StartCoroutine(StartingGameMobile());
    }

    private IEnumerator StartingGameMobile()
    {
        mainCG.Hide(speed: 0.5f);
        AudioManager.instance.StopTrack(0);

        while (mainCG.isVisible)
            yield return null;

        VN_Configuration.activeConfig.Save();
        UnityEngine.SceneManagement.SceneManager.LoadScene("VisualNovel (Android)");
    }

    public void LoadGame(VNGameSave file)
    {
        VNGameSave.activeFile = file;
        StartCoroutine(StartingGame());
    }

    private IEnumerator StartingGame()
    {
        mainCG.Hide(speed: 0.5f);
        AudioManager.instance.StopTrack(0);

        while (mainCG.isVisible)
            yield return null;

        VN_Configuration.activeConfig.Save();
        UnityEngine.SceneManagement.SceneManager.LoadScene("VisualNovel");
    }

    private void HideLoadOptionOnNoSaveFiles()
    {
        if (!HasSaveFiles())
        {
            loadGame.gameObject.SetActive(false);
            loadGameSpacer.gameObject.SetActive(false);
        }
        else
        {
            if (loadGame.gameObject.activeInHierarchy && loadGameSpacer.gameObject.activeInHierarchy)
            {
                return;
            }

            loadGame.gameObject.SetActive(true);
            loadGameSpacer.gameObject.SetActive(true);
        }
    }

    private bool HasSaveFiles()
    { 
        try
        {
            if (!System.IO.Directory.Exists(FilePaths.gameSaves))
            {
                FileManager.TryCreateDirectoryFromPath(FilePaths.gameSaves);
                return false;
            }

            string[] saveFiles = System.IO.Directory.GetFiles(FilePaths.gameSaves, "*.vns");

            return saveFiles.Length > 0;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error checking save files: {e.Message}");
            return false;
        }
    }
}
