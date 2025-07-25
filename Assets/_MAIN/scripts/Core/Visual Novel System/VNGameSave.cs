using DIALOGUE;
using HISTORY;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VISUALNOVEL
{
    [System.Serializable]
    public class VNGameSave
    {
        public static VNGameSave activeFile = null;

        public const string FILE_TYPE = ".vns";
        public const string SCREENSHOT_FILE_TYPE = ".jpg";
        public const bool ENCRYPT_FILES = true;
        public const float SCREENSHOT_DOWNSCALE_AMOUNT = 0.25f;

        public string filePath => $"{FilePaths.gameSaves}{slotNumber}{FILE_TYPE}";
        public string screenshotPath => $"{FilePaths.gameSaves}{slotNumber}{SCREENSHOT_FILE_TYPE}";

        public int slotNumber = 1;
        public string playerName;
        public int SPValue;

        public bool newGame = true;
        public string[] activeConversations;
        public HistoryState activeState;
        public HistoryState[] historyLog;
        public VN_VariableData[] variables;

        public string chapter;
        public string timestamp;

        public static VNGameSave Load(string filePath, bool activateOnLoad = false)
        {
            VNGameSave save = FileManager.Load<VNGameSave>(filePath, ENCRYPT_FILES);

            activeFile = save;

            if (activateOnLoad)
                save.Activate();

            return save;
        }

        public void Save()
        {
            newGame = false;

            activeState = HistoryState.Capture();
            historyLog = HistoryManager.instance.history.ToArray();
            activeConversations = GetConversationData();
            variables = GetVariableData();

            timestamp = DateTime.Now.ToString("MMMM dd, yyyy HH:mm:ss");

            // this expects all the chapter files to be written in this format "Chapter N - <Title>" and splits it by the '-' sign

            string chapterFile = DialogueSystem.instance.conversationManager.conversation.file;
            int splitIndex = chapterFile.IndexOf('-');

            if (splitIndex != -1 && splitIndex < chapterFile.Length - 1)
            {
                chapter = chapterFile.Substring(splitIndex + 1).Trim();
            }
            else
            {
                Debug.LogError($"Could not properly split string '{chapterFile}'");
            }

            string saveJSON = JsonUtility.ToJson(this);
            FileManager.Save(filePath, saveJSON, ENCRYPT_FILES);
            ScreenshotMaster.CaptureScreenshot(VNManager.instance.mainCamera, Screen.width, Screen.height, SCREENSHOT_DOWNSCALE_AMOUNT, screenshotPath);
        }

        public void AutoSave()
        {
            slotNumber = 1;
            Save();
        }

        public void Activate()
        {
            if (activeState != null)
                activeState.Load();

            HistoryManager.instance.history = historyLog.ToList();
            HistoryManager.instance.logManager.Clear();
            HistoryManager.instance.logManager.Rebuild();

            SetVariableData();

            SetConversationData();

            DialogueSystem.instance.prompt.Hide();

            if (!activeState.dialogue.isOnLogicalLine && ChoicePanel.instance.cg.isVisible)
            {
                ChoicePanel.instance.Hide();
            }
        }

        public string[] GetConversationData()
        {
            List<string> returnData = new List<string>();
            var conversations = DialogueSystem.instance.conversationManager.GetConversationQueue();
            
            for (int i = 0; i < conversations.Length; i++)
            {
                var conversation = conversations[i];
                string data = "";

                if (conversation.file != string.Empty)
                {
                    var compressedData = new VN_ConversationDataCompressed();
                    compressedData.fileName = conversation.file;
                    compressedData.progress = conversation.GetProgress();
                    compressedData.startIndex = conversation.fileStartIndex;
                    compressedData.endIndex = conversation.fileEndIndex;
                    data = JsonUtility.ToJson(compressedData);
                }
                else
                {
                    var fullData = new VN_ConversationData();
                    fullData.conversation = conversation.GetLines();
                    fullData.progress = conversation.GetProgress();
                    data = JsonUtility.ToJson(fullData);
                }

                returnData.Add(data);
            }

            return returnData.ToArray();
        }

        private void SetConversationData()
        {
            for (int i = 0; i < activeConversations.Length; i++)
            {
                try
                {
                    string data = activeConversations[i];
                    Conversation conversation = null;

                    var fullData = JsonUtility.FromJson<VN_ConversationData>(data);
                    if (fullData != null && fullData.conversation != null && fullData.conversation.Count > 0)
                    {
                        conversation = new Conversation(fullData.conversation, fullData.progress);
                    }
                    else
                    {
                        var compressedData = JsonUtility.FromJson<VN_ConversationDataCompressed>(data);
                        if (compressedData != null && compressedData.fileName != string.Empty)
                        {
                            TextAsset file = Resources.Load<TextAsset>(FilePaths.GetPathToResources(FilePaths.resources_dialogueFiles, compressedData.fileName));

                            int count = compressedData.endIndex - compressedData.startIndex;

                            List<string> lines = FileManager.ReadTextAsset(file).Skip(compressedData.startIndex).Take(count + 1).ToList();

                            conversation = new Conversation(lines, compressedData.progress, compressedData.fileName, compressedData.startIndex, compressedData.endIndex);
                        }
                        else
                        {
                            Debug.LogError($"Unknown conversation format! Unable to reload conversation from VNGameSave using data '{data}'");
                        }
                    }

                    if (conversation != null && conversation.GetLines().Count > 0)
                    {
                        if (i == 0)
                            DialogueSystem.instance.conversationManager.StartConversation(conversation);
                        else
                            DialogueSystem.instance.conversationManager.Enqueue(conversation);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Encountered error while extracting saved conversation data. {e}");
                }
            }
        }

        private VN_VariableData[] GetVariableData()
        {
            List<VN_VariableData> returnData = new List<VN_VariableData>();

            foreach (var database in VariableStore.databases.Values)
            {
                foreach (var variable in database.variables)
                {
                    VN_VariableData variableData = new VN_VariableData();
                    variableData.name = $"{database.name}.{variable.Key}";
                    string val = $"{variable.Value.Get()}";
                    variableData.value = val;
                    variableData.type = val == string.Empty ? "System.String" : variable.Value.Get().GetType().ToString() == "System.Double" ? "System.Single" : variable.Value.Get().GetType().ToString();
                    returnData.Add(variableData);
                }
            }

            return returnData.ToArray();
        }

        private void SetVariableData()
        {
            foreach (var variable in variables)
            {
                string val = variable.value;

                switch (variable.type)
                {
                    case "System.Boolean":
                        if (bool.TryParse(val, out bool b_val))
                        {
                            VariableStore.TrySetValue(variable.name, b_val);
                            continue;
                        }
                        break;
                    case "System.Int32":
                        if (int.TryParse(val, out int i_val))
                        {
                            VariableStore.TrySetValue(variable.name, i_val);
                            continue;
                        }
                        break;
                    case "System.Single":
                        if (float.TryParse(val, out float f_val))
                        {
                            VariableStore.TrySetValue(variable.name, f_val);
                            continue;
                        }
                        break;
                    case "System.String":
                        VariableStore.TrySetValue(variable.name, val);
                        continue;
                }

                Debug.LogError($"Could not interpret variable type. {variable.name} = {variable.value}");    
            }
        }
    }
}