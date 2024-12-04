using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using VISUALNOVEL;

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

            statusTitle.text = $"{fileNumber - 1}. Empty Slot";
            statusDateTime.text = "";
            deleteButton.gameObject.SetActive(false);
            DisableCurrentOnClickAndSetNewOnClick(function, file);
            previewImage.texture = SaveandLoadMenu.instance.emptyFileImage;
        }
        else
        {
            if (isAutoSaveSlot)
            {
                SetAutoSaveSlot(function, file);
                return;
            }

            statusTitle.text = $"{fileNumber - 1}. {file.chapter}";
            statusDateTime.text = $"{file.timestamp}";
            deleteButton.gameObject.SetActive(true);
            DisableCurrentOnClickAndSetNewOnClick(function, file);

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
        statusTitle.text = file == null ? "Auto Save." : $"Auto. {file.chapter}";
        statusDateTime.text = file == null ? "" : file.timestamp;
        deleteButton.gameObject.SetActive(false);
        saveSlotButton.onClick.RemoveAllListeners();

        // never add the onClick Save listener so that players cant overwrite the autosave
        if (function == SaveandLoadMenu.MenuFunction.Save)
        {
            ColorBlock colors = saveSlotButton.colors;
            colors.highlightedColor = Color.white;
            saveSlotButton.colors = colors;
        }

        if (function == SaveandLoadMenu.MenuFunction.Load)
        {
            ColorBlock colors = saveSlotButton.colors;
            colors.highlightedColor = new Color(234, 234, 234, 216);
            saveSlotButton.colors = colors;
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
        var activeSave = VNGameSave.activeFile;
        File.Delete(activeSave.screenshotPath);
        File.Delete(filePath);
        PopulateDetails(SaveandLoadMenu.instance.menuFunction);
    }

    public void Save()
    {
        var activeSave = VNGameSave.activeFile;
        activeSave.slotNumber = fileNumber;

        activeSave.Save();
        PopulateDetailsFromFile(SaveandLoadMenu.instance.menuFunction, activeSave);
    }

    public void Load()
    {
        VNGameSave file = VNGameSave.Load(filePath, activateOnLoad: true);
        SaveandLoadMenu.instance.Close(closeAllMenus: true);
    }
}
