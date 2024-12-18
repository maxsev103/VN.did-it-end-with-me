using DIALOGUE;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VISUALNOVEL;

namespace COMMANDS
{
    public class CMD_DatabaseExtension_General : CMDDatabaseExtension
    {
        private static string[] PARAM_SPEED = new string[] { "-spd", "-speed" };
        private static string[] PARAM_IMMEDIATE = new string[] { "-i", "-immediate" };
        private static string[] PARAM_FILEPATH = new string[] { "-f", "-file", "-filepath" };
        private static string[] PARAM_ENQUEUE = new string[] { "-enq", "-enqueue" };

        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("wait", new Func<string, IEnumerator>(Wait));

            // dialogue system controls
            database.AddCommand("showsystem", new Func<string[], IEnumerator>(ShowDialogueSystem));
            database.AddCommand("hidesystem", new Func<string[], IEnumerator>(HideDialogueSystem));

            // dialogue box controls
            database.AddCommand("showdb", new Func<string[], IEnumerator>(ShowDialogueBox));
            database.AddCommand("hidedb", new Func<string[], IEnumerator>(HideDialogueBox));
            database.AddCommand("dbalpha", new Action<string>(SetDialogueBoxAlpha));
            database.AddCommand("resetdbalpha", new Action<string>(ResetDialogueBoxAlpha));

            // file loading for branching paths
            database.AddCommand("load", new Action<string[]>(LoadNewDialogueFile));

            // system navigation
            database.AddCommand("returntomainmenu", new Func<string, IEnumerator>(ReturnToMainMenu));

            // saving to autosave
            database.AddCommand("save", new Action<string>(AutoSave));
        }

        private static void LoadNewDialogueFile(string[] data)
        {
            string fileName = string.Empty;
            bool enqueue = false;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_FILEPATH, out fileName);
            parameters.TryGetValue(PARAM_ENQUEUE, out enqueue, defaultValue: false);

            string filePath = FilePaths.GetPathToResources(FilePaths.resources_dialogueFiles, fileName);
            TextAsset file = Resources.Load<TextAsset>(filePath);

            if (file == null)
            {
                Debug.LogError($"Could not load '{fileName}' from the dialogue files. Please ensure it exists within the '{FilePaths.resources_dialogueFiles}' resources folder");
                return;
            }

            List<string> lines = FileManager.ReadTextAsset(file, includeBlankLines: true);
            Conversation newConversation = new Conversation(lines, file: fileName);

            if (enqueue)
                DialogueSystem.instance.conversationManager.Enqueue(newConversation);
            else
                DialogueSystem.instance.conversationManager.StartConversation(newConversation);
        }

        private static IEnumerator Wait(string data)
        {
            if (float.TryParse(data, out float time)) 
            {
                yield return new WaitForSeconds(time);    
            }
        }

        private static IEnumerator ShowDialogueBox(string[] data)
        {
            float speed;
            bool immediate;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);

            yield return DialogueSystem.instance.dialogueContainer.Show();
        }

        private static IEnumerator HideDialogueBox(string[] data)
        {
            float speed;
            bool immediate;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);

            yield return DialogueSystem.instance.dialogueContainer.Hide();
        }

        private static IEnumerator ShowDialogueSystem(string[] data) 
        {
            float speed;
            bool immediate;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);

            yield return DialogueSystem.instance.Show(speed, immediate);
        }

        private static IEnumerator HideDialogueSystem(string[] data)
        {
            float speed;
            bool immediate;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);

            yield return DialogueSystem.instance.Hide(speed, immediate);
        }

        private static IEnumerator ReturnToMainMenu(string data)
        {
            CanvasGroup main;
            CanvasGroupController mainCG;
            
            main = GraphicPanelManager.instance.GetPanel("cg").rootPanel.transform.parent.GetComponentInParent<CanvasGroup>();
            mainCG = new CanvasGroupController(VNManager.instance, main);

            mainCG.Hide(speed: 0.5f);
            AudioManager.instance.StopTrack(0);

            while (mainCG.isVisible)
                yield return null;

            VN_Configuration.activeConfig.Save();

            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "VisualNovel (Android)")
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu (Android)");
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }

        private static void AutoSave(string data)
        {
            VNGameSave.activeFile.AutoSave();
        }

        private static void SetDialogueBoxAlpha(string data)
        {
            float alpha;

            if (float.TryParse(data, out alpha))
            {
                DialogueSystem.instance.dialogueContainer.SetDialogueBoxAlpha(alpha);
                DialogueSystem.instance.dialogueContainer.nameContainer.SetNameBoxAlpha(alpha);
                return;
            }

            Debug.LogError("Invalid value passed into command 'dbalpha'");
        }

        private static void ResetDialogueBoxAlpha(string data)
        {
            if (!(data == string.Empty))
            {
                Debug.LogError("Invalid parameters for command 'resetdbalpha'.");
                return;
            }

            DialogueSystem.instance.dialogueContainer.ResetAlpha();
            DialogueSystem.instance.dialogueContainer.nameContainer.ResetAlpha();
        }

    }
}