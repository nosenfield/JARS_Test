using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Timers;
using UnityEditor;
using UnityEngine;

[Serializable]
public class JsonAnimation
{
    public string character;
    public string animation;
}

public class Main : MonoBehaviour
{
    [SerializeField] private string jsonAnimationDirectory;
    [SerializeField] private float fileCheckIntervalSeconds = 1f;
    private List<string> knownFiles;
    private System.Timers.Timer timer;
    private bool waitingForInterval;

    void Awake()
    {
        knownFiles = new List<string>();
        waitingForInterval = false;
    }

    void OnEnable()
    {
        // Disabling timer in favor of CoRoutine
        // timer = new System.Timers.Timer();
        // timer.Interval = fileCheckIntervalSeconds * 1000f;
        // timer.AutoReset = true;
        // timer.Elapsed += CheckAnimationDirectoryForFilesTimerWrapper;
        // timer.Start();
    }

    void Update()
    {
        if (!waitingForInterval)
        {
            StartCoroutine(CoroutineTimer());
        }
    }

    IEnumerator CoroutineTimer()
    {
        waitingForInterval = true;
        yield return new WaitForSeconds(fileCheckIntervalSeconds);
        CheckAnimationDirectoryForFiles();
        waitingForInterval = false;
    }

    void OnDisable()
    {
        timer?.Stop();
    }

    private void CheckAnimationDirectoryForFilesTimerWrapper(object source, ElapsedEventArgs e)
    {
        CheckAnimationDirectoryForFiles();
    }

    private void CheckAnimationDirectoryForFiles()
    {
        string directoryPath = Path.Combine(Application.dataPath, jsonAnimationDirectory);
        Debug.Log($"CheckAnimationDirectoryForFiles: {directoryPath}");

        if (Directory.Exists(directoryPath))
        {
            Debug.Log($"Enumerating files at: ${directoryPath}");
            List<string> currentFiles = new List<string>();
            var jsonFiles = Directory.EnumerateFiles(directoryPath, "*.json");

            foreach (string file in jsonFiles)
            {
                if (knownFiles.Contains(file))
                {
                    Debug.Log($"Skipping file: {file}, already counted");
                }
                else
                {
                    Debug.Log($"Reading contents of: {file}");

                    StreamReader reader = new StreamReader(file);
                    var json = reader.ReadToEnd();
                    JsonAnimation jsonAnimation = JsonUtility.FromJson<JsonAnimation>(json);
                    reader.Close();

                    Debug.Log($"character: {jsonAnimation.character}");
                    Debug.Log($"animation: {jsonAnimation.animation}");
                    TriggerAnimation(jsonAnimation.character);
                }

                currentFiles.Add(file);
            }

            // resetting the list of known files to current files handles file deletion/recreation
            knownFiles = currentFiles;
        }
        else
        {
            Debug.LogError($"{directoryPath} does not exist!");
        }
    }

    void TriggerAnimation(string characterId)
    {
        Debug.Log($"Trigger animation for character: {characterId}");

        GameObject character = GameObject.Find(characterId);
        if (character == null)
        {
            Debug.LogWarning($"GameObject not found for Id: {characterId}");
        }
        else
        {
            Debug.Log($"Trigger animation for: {character}");
            Animator animator = character.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogWarning($"No animator found on {character}");
            }
            else
            {
                animator.SetTrigger("Animate");
            }
        }
    }

    void OnDestroy()
    {
        timer?.Dispose();
        timer = null;
    }
}