using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConsoleController : MonoBehaviour
{
    private KeyboardControls keyboardControls;
    private bool consoleOn;
    public GameObject consolePrefab;
    private GameObject console;
    private Stack<string> existingLog = new Stack<string>();

    private void OnEnable()
    {
        keyboardControls?.Enable();
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        keyboardControls.Disable();
        Application.logMessageReceived -= HandleLog;
    }

    private void Awake()
    {
        keyboardControls = new KeyboardControls();

        keyboardControls.Mouse.Tilde.performed += _ => ShowConsole(!consoleOn);
    }

    public void ShowConsole(bool state)
    {
        consoleOn = state;
        
        if (consoleOn)
        {
            console = Instantiate(consolePrefab, InteractableCollection.Instance.Canvas.transform);
            TMP_Text consoleText = console.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<TMP_Text>();

            consoleText.text = "-> Most recent \n";

            foreach (string log in existingLog)
                consoleText.text += log + "\n";
        } 
        else
        {
            Destroy(console);
            console = null;
        }
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        string newString = "\n [" + type + "] : " + logString;

        if (type == LogType.Exception)
        {
            newString = "\n" + stackTrace;
        }

        existingLog.Push(newString);
    }
}
