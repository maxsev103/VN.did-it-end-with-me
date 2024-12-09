using DIALOGUE;
using HISTORY;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class VN_Configuration
{
    public static VN_Configuration activeConfig;

    public static string filePath => $"{FilePaths.root}vnconfig.cfg";

    public const bool ENCRYPT = true;

    // general settings
    public bool display_fullscreen = true;
    public string displayResolution = "1920x1080";
    public bool continueSkippingAfterChoice = false;
    public float dialogueTextSpeed = 1f;
    public float dialogueAutoReadSpeed = 1f;

    // audio settings
    public float musicVolume = 1f;
    public float sfxVolume = 1f;
    public float voiceVolume = 1f;
    public bool musicMute = false;
    public bool sfxMute = false;
    public bool voiceMute = false;

    // other settings
    public float historyLogScale = 1f;

    public void Load()
    {
        var ui = ConfigMenu.instance.ui;

        // GENERAL SETTINGS
        // set window size
        ConfigMenu.instance.ToggleFullscreen(display_fullscreen);

        // set screen resolution
        
        int resIndex = 0;
        if (ui.resolutions != null)
        {
            
            for (int i = 0; i < ui.resolutions.options.Count; i++)
            {
                string resolution = ui.resolutions.options[i].text;
                if (resolution == displayResolution)
                {
                    resIndex = i;
                    break;
                }
            }
        }

        if (ui.resolutions != null)
            ui.resolutions.value = resIndex;

        // set continue skipping after option
        ui.SetButtonSelection(ui.skippingContinue, ui.skippingStop, continueSkippingAfterChoice);

        // set the value of the text and autoreader speed
        ui.architectSpeed.value = dialogueTextSpeed;
        ui.autoReaderSpeed.value = dialogueAutoReadSpeed;

        // set the log scaling
        if (ui.logScaling != null)
            ui.logScaling.value = historyLogScale;

        // set audio mixer volumes
        ui.musicVolume.value = musicVolume;
        ui.sfxVolume.value = sfxVolume;
        ui.voiceVolume.value = voiceVolume;
    }

    public void Save()
    {
        FileManager.Save(filePath, JsonUtility.ToJson(this), encrypt: ENCRYPT);
    }
}
