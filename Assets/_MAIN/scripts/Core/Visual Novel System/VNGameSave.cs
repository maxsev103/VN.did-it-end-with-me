using HISTORY;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VNGameSave
{
    public static VNGameSave activeFile = null;

    public const string FILE_TYPE = ".vns";
    public const string SCREENSHOT_FILE_TYPE = ".jpg";
    public const bool ENCRYPT_FILES = false;

    public string filePath => $"{FilePaths.gameSaves}{slotNumber}{FILE_TYPE}";
    public string screenshotPath => $"{FilePaths.gameSaves}{slotNumber}{SCREENSHOT_FILE_TYPE}";

    public int slotNumber = 1;
    public string playerName;

    public string[] activeConversations;
    public HistoryState activeState;
    public HistoryState[] historyLog;


}
