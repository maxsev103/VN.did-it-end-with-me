using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VISUALNOVEL;

public class AutoSaveManager : MonoBehaviour
{
    public static AutoSaveManager instance;
    public float saveInterval = 300f; // 5 minutes
    private bool isPaused = false;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        StartCoroutine(AutosaveRoutine());
    }

    IEnumerator AutosaveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(saveInterval);

            if (!isPaused && VNGameSave.activeFile != null)
            {
                VNGameSave.activeFile.AutoSave();
                Debug.Log("Autosave completed");
            }
        }
    }

    public void PauseAutosave() => isPaused = true;
    public void ResumeAutosave() => isPaused = false;
}
