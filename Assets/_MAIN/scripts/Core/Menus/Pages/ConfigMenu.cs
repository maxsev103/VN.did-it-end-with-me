using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ConfigMenu : MenuPage
{
    public static ConfigMenu instance { get; private set; }

    [SerializeField] private GameObject[] panels;
    private GameObject activePanel;

    public UI_ITEMS ui;

    private VN_Configuration config => VN_Configuration.activeConfig;
    public Toggle[] toggles;

    private void Awake()
    {
        instance = this;   
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == 0);
        }

        activePanel = panels[0];
        toggles[0].interactable = true;
        toggles[0].isOn = true;

        SetAvailableResolutions();

        LoadConfig();
    }

    public void OpenPanel(string panelName)
    {
        GameObject panel = panels.First(p => p.name.ToLower() == panelName.ToLower());

        if (panel == null)
        {
            Debug.LogWarning($"Did not find panel called '{panelName}' in config menu.");
            return;
        }
        
        if (activePanel != null && activePanel != panel)
            activePanel.SetActive(false);

        panel.SetActive(true);
        activePanel = panel;
    }

    private void LoadConfig()
    {
        if (File.Exists(VN_Configuration.filePath))
        {
            VN_Configuration.activeConfig = FileManager.Load<VN_Configuration>(VN_Configuration.filePath, encrypt: VN_Configuration.ENCRYPT);
        }
        else
        {
            VN_Configuration.activeConfig = new VN_Configuration();
        }

        VN_Configuration.activeConfig.Load();
    }

    private void OnApplicationQuit()
    {
        VN_Configuration.activeConfig.Save();
        VN_Configuration.activeConfig = null;
    }

    private void SetAvailableResolutions()
    {
        Resolution[] resolutions = Screen.resolutions;
        List<string> options = new List<string>();

        for (int i = resolutions.Length - 1; i >= 0; i--)
        {
            options.Add($"{resolutions[i].width}x{resolutions[i].height}");
        }

        ui.resolutions.ClearOptions();
        ui.resolutions.AddOptions(options);
    }


    [System.Serializable]
    public class UI_ITEMS
    {
        private static Color textSelectedColor = new Color(138f/255f, 79f/255f, 28f/255f, 1);
        private static Color textUnselectedColor = Color.white;

        [Header("General")]
        public TMP_Dropdown resolutions;
        public Button fullscreen, windowed;
        public Button skippingContinue, skippingStop;
        public Slider architectSpeed, autoReaderSpeed;

        [Header("Audio")]
        public Slider musicVolume;
        public Slider sfxVolume;
        public Slider voiceVolume;

        [Header("Other")]
        public Slider logScaling;

        public void SetButtonSelection(Button A, Button B, bool selectedA)
        {
            Debug.Log($"SetButtonSelection called - A: {A.name}, B: {B.name}, selectedA: {selectedA}");

            if (A == null || B == null)
            {
                Debug.LogError("Buttons are null!");
                return;
            }

            A.interactable = true;
            B.interactable = true;

            var textA = A.GetComponentInChildren<TextMeshProUGUI>();
            var textB = B.GetComponentInChildren<TextMeshProUGUI>();

            if (textA == null || textB == null)
            {
                Debug.LogError("Could not find TextMeshPro components!");
                return;
            }

            textA.color = selectedA ? textSelectedColor : textUnselectedColor;
            textB.color = !selectedA ? textSelectedColor : textUnselectedColor;
            
            try
            {
                if (selectedA)
                {
                    A.Select();
                }
                else
                {
                    B.Select();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Button selection error: {e.Message}");
            }
        }
    }

    // UI CALLABLE FUNCTIONS
    public void ToggleFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
        ui.SetButtonSelection(ui.fullscreen, ui.windowed, fullscreen);
    }

    public void SetDisplayResolution()
    {
        string resolution = ui.resolutions.captionText.text;
        string[] values = resolution.Split('x');

        if (int.TryParse(values[0], out int width) && int.TryParse(values[1], out int height))
        {
            Screen.SetResolution(width, height, Screen.fullScreen);
            config.displayResolution = resolution;
        }
        else
            Debug.LogError($"Parsing error for screen resolution! [{resolution}] could not be parsed into WIDTHxHEIGHT");
    }

    public void SetContinueSkippingAfterChoice(bool continueSkipping)
    {
        config.continueSkippingAfterChoice = continueSkipping;
        ui.SetButtonSelection(ui.skippingContinue, ui.skippingStop, continueSkipping);
    }

    public void SetTextArchitectSpeed()
    {
        config.dialogueTextSpeed = ui.architectSpeed.value;
        DialogueSystem.instance.conversationManager.architect.standardSpeed = config.dialogueTextSpeed;
    }

    public void SetAutoReadSpeed()
    {
        config.dialogueAutoReadSpeed = ui.autoReaderSpeed.value;

        AutoReader autoReader = DialogueSystem.instance.autoReader;
        if (autoReader != null)
            autoReader.speed = config.dialogueAutoReadSpeed;
    }

    public void SetMusicVolume()
    {
        config.musicVolume = ui.musicVolume.value;
        AudioMixerGroup mixer = AudioManager.instance.musicMixer;
        AnimationCurve fallOffCurve = AudioManager.instance.audioFallOffCurve;

        mixer.audioMixer.SetFloat(AudioManager.MUSIC_VOLUME_PARAMETER_NAME, fallOffCurve.Evaluate(config.musicVolume));
    }

    public void SetSFXVolume()
    {
        config.sfxVolume = ui.sfxVolume.value;
        AudioMixerGroup mixer = AudioManager.instance.musicMixer;
        AnimationCurve fallOffCurve = AudioManager.instance.audioFallOffCurve;

        mixer.audioMixer.SetFloat(AudioManager.SFX_VOLUME_PARAMETER_NAME, fallOffCurve.Evaluate(config.sfxVolume));
    }

    public void SetVoiceVolume()
    {
        config.voiceVolume = ui.voiceVolume.value;
        AudioMixerGroup mixer = AudioManager.instance.musicMixer;
        AnimationCurve fallOffCurve = AudioManager.instance.audioFallOffCurve;

        mixer.audioMixer.SetFloat(AudioManager.VOICE_VOLUME_PARAMETER_NAME, fallOffCurve.Evaluate(config.voiceVolume));
    }
}
