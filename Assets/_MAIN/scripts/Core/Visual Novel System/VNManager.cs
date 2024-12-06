using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using TESTING;
using UnityEngine;

namespace VISUALNOVEL
{
    public class VNManager : MonoBehaviour
    {
        public static VNManager instance { get; private set; }
        [SerializeField] private VisualNovelSO config;
        public Camera mainCamera;

        private void Awake()
        {
            instance = this;
            Debug.Log($"Starting file is called: {config.startingFile.name}");
            VNDatabaseLinkSetup linkSetup = GetComponent<VNDatabaseLinkSetup>();
            linkSetup.SetupExternalLinks();

            if (VNGameSave.activeFile == null)
                VNGameSave.activeFile = new VNGameSave();
        }

        private void Start()
        {
            
            LoadGame();
        } 

        private void LoadGame()
        {
            if (VNGameSave.activeFile.newGame)
            {
                List<string> lines = FileManager.ReadTextAsset(config.startingFile);
                Conversation start = new Conversation(lines, file: config.startingFile.name);
                DialogueSystem.instance.Say(start);
            }
            else
            {
                VNGameSave.activeFile.Activate();
            }
        }
    }
}