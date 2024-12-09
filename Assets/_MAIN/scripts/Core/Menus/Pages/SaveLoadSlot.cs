using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using VISUALNOVEL;
using HISTORY;

public class SaveLoadSlot : MonoBehaviour
{
    public GameObject root;
    public RawImage previewImage;
    public TextMeshProUGUI statusTitle;
    public TextMeshProUGUI statusDateTime;
    public Button deleteButton;
    public Button saveSlotButton;

    [HideInInspector] public int fileNumber = 0;
    [HideInInspector] public string filePath = "";

    private UIConfirmationMenu uiChoiceMenu => UIConfirmationMenu.instance;

    private bool isAutoSaveSlot => fileNumber == 1;

    public void PopulateDetails(SaveandLoadMenu.MenuFunction function)
    {
        if (File.Exists(filePath))
        {
            VNGameSave file = VNGameSave.Load(filePath, activateOnLoad: false);
            PopulateDetailsFromFile(function, file);
        }
        else
        {
            PopulateDetailsFromFile(function, null);
        }
    }

    private void PopulateDetailsFromFile(SaveandLoadMenu.MenuFunction function, VNGameSave file)
    {
        if (file == null)
        {
            if (isAutoSaveSlot)
            {
                SetAutoSaveSlot(function, file);
                return;
            }

            if (function == SaveandLoadMenu.MenuFunction.Save)
                saveSlotButton.interactable = true;
            else
                saveSlotButton.interactable = false;

            statusTitle.text = $"{fileNumber - 1}. Empty Slot";
            statusDateTime.text = "";
            deleteButton.gameObject.SetActive(false);
            DisableCurrentOnClickAndSetNewOnClick(function, file);

            // set the preview image as the empty file image
            previewImage.texture = SaveandLoadMenu.instance.emptyFileImage;
        }
        else
        {
            if (isAutoSaveSlot)
            {
                SetAutoSaveSlot(function, file);
                return;
            }

            saveSlotButton.interactable = true;
            statusTitle.text = $"{fileNumber - 1}. {file.chapter}";
            statusDateTime.text = $"{file.timestamp}";
            deleteButton.gameObject.SetActive(true);
            DisableCurrentOnClickAndSetNewOnClick(function, file);

            // set the preview image as the screenshot
            byte[] imageData = File.ReadAllBytes(file.screenshotPath);
            Texture2D screenshotPreview = new Texture2D(1, 1);
            ImageConversion.LoadImage(screenshotPreview, imageData);
            previewImage.texture = screenshotPreview;
        }
    }   

    private void DisableCurrentOnClickAndSetNewOnClick(SaveandLoadMenu.MenuFunction function, VNGameSave file)
    {
        // Remove existing click events
        saveSlotButton.onClick.RemoveAllListeners();

        // Add new conditional click event
        if (function == SaveandLoadMenu.MenuFunction.Save)
        {
            saveSlotButton.onClick.AddListener(Save);
        }
        else if (function == SaveandLoadMenu.MenuFunction.Load)
        {
            saveSlotButton.onClick.AddListener(Load);
        }
    }

    private void SetAutoSaveSlot(SaveandLoadMenu.MenuFunction function, VNGameSave file)
    {
        statusTitle.text = file == null ? "Auto Save." : $"Auto. from Save {VNGameSave.activeFile.slotNumber} - {file.chapter}";
        statusDateTime.text = file == null ? "" : file.timestamp;
        deleteButton.gameObject.SetActive(false);
        saveSlotButton.onClick.RemoveAllListeners();

        // never add the onClick Save listener so that players cant overwrite the autosave
        if (function == SaveandLoadMenu.MenuFunction.Save)
        {
            saveSlotButton.interactable = false;
        }

        if (function == SaveandLoadMenu.MenuFunction.Load)
        {
            if (file == null)
                saveSlotButton.interactable = false;
            else
                saveSlotButton.interactable = true;

            // add the listener for loading
            saveSlotButton.onClick.AddListener(Load);
        }

        if (file == null)
            previewImage.texture = SaveandLoadMenu.instance.emptyFileImage;
        else
        {
            byte[] imageData = File.ReadAllBytes(file.screenshotPath);
            Texture2D screenshotPreview = new Texture2D(1, 1);
            ImageConversion.LoadImage(screenshotPreview, imageData);
            previewImage.texture = screenshotPreview;
        }
    }

    public void Delete()
    {
        uiChoiceMenu.Show("Do you want to delete this save? (This action cannot be undone)", 
            new UIConfirmationMenu.ConfirmationButton("Yes", OnConfirmDelete), 
            new UIConfirmationMenu.ConfirmationButton("No", null));
    }

    private void OnConfirmDelete()
    {
        var activeSave = VNGameSave.activeFile;
        File.Delete(activeSave.screenshotPath);
        File.Delete(filePath);
        PopulateDetails(SaveandLoadMenu.instance.menuFunction);
    }

    public void Save()
    {
        if (HistoryManager.instance.isViewingHistory)
        {
            uiChoiceMenu.Show("Can't save while viewing history.", new UIConfirmationMenu.ConfirmationButton("Back", null));
            return;
        }

        var activeSave = VNGameSave.activeFile;

        string expectedFilePath = $"{FilePaths.gameSaves}{fileNumber}{VNGameSave.FILE_TYPE}";

        bool fileExists = File.Exists(expectedFilePath);

        uiChoiceMenu.Show(fileExists ? "Do you want to overwrite this save file?" : "Do you want to save?", 
            new UIConfirmationMenu.ConfirmationButton("Yes", () => OnConfirmSave(activeSave)), 
            new UIConfirmationMenu.ConfirmationButton("No", null));
    }

    private void OnConfirmSave(VNGameSave activeSave)
    {
        activeSave.slotNumber = fileNumber;
        activeSave.Save();
        PopulateDetailsFromFile(SaveandLoadMenu.instance.menuFunction, activeSave);
    }

    public void Load()
    {
        uiChoiceMenu.Show("Do you want to load this save file?", 
            new UIConfirmationMenu.ConfirmationButton("Yes", OnConfirmLoad), 
            new UIConfirmationMenu.ConfirmationButton("No", null));
    }

    public void OnConfirmLoad()
    {
        VNGameSave file = VNGameSave.Load(filePath, activateOnLoad: false);
        SaveandLoadMenu.instance.Close(closeAllMenus: true);

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == MainMenu.MAIN_MENU_SCENE)
        {
            MainMenu.instance.LoadGame(file);
        }
        else
        {
            file.Activate();
        }
    }
}
