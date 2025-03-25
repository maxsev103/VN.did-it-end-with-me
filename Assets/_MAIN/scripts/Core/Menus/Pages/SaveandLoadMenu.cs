using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VISUALNOVEL;

public class SaveandLoadMenu : MenuPage
{
    public static SaveandLoadMenu instance { get; private set; }

    public const int MAX_FILES = 42;

    private int currentPage = 1;
    private bool loadedFilesForFirstTime = false;

    public enum MenuFunction { Save, Load }
    public MenuFunction menuFunction = MenuFunction.Save;

    public SaveLoadSlot[] saveSlots;
    public int slotsPerPage => saveSlots.Length;

    public Texture emptyFileImage;
    public Sprite saveSprite;
    public Sprite loadSprite;
    public Image titleImage;

    private void Awake()
    {
        instance = this;
    }

    public override void Open()
    {
        base.Open();

        if (menuFunction == MenuFunction.Save)
        {
            titleImage.sprite = saveSprite;
        }
        else
        {
            titleImage.sprite = loadSprite;
        }

        if (!loadedFilesForFirstTime)
        {
            PopulateSaveSlotsForPage(currentPage);
        }
    }

    public void PopulateSaveSlotsForPage(int pageNumber)
    {
        currentPage = pageNumber;
        int startingFile = ((currentPage - 1) * slotsPerPage) + 1;
        int endingFile = startingFile + slotsPerPage - 1;

        for (int i = 0; i < slotsPerPage; i++)
        {
            int fileNum = startingFile + i;
            SaveLoadSlot slot = saveSlots[i];

            if (fileNum <= MAX_FILES)
            {
                slot.root.SetActive(true);
                string filePath = $"{FilePaths.gameSaves}{fileNum}{VNGameSave.FILE_TYPE}";
                slot.fileNumber = fileNum;
                slot.filePath = filePath;
                slot.pageNumber = currentPage;
                slot.PopulateDetails(menuFunction);
            }
            else
            {
                slot.root.SetActive(false);
            }
        }
    }
}
