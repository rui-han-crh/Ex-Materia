using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadButtonInitialiser : MonoBehaviour
{
    [SerializeField]
    private TMP_Text saveRecord;

    private void Awake()
    {
        string fullPath = Application.persistentDataPath + SaveManager.directoryPath + SaveManager.fileName;

        if (File.Exists(fullPath))
        {
            GetComponent<Button>().onClick.AddListener(SaveFunction);
            saveRecord.text = "Last Saved: " + File.GetLastWriteTime(fullPath).ToString("G", CultureInfo.GetCultureInfo("en-GB"));
        } 
        else
        {
            GetComponent<Button>().interactable = false;
        }
    }

    private void SaveFunction()
    {
        SaveManager.DeserialiseFromFile();
        SaveFile.Load();
    }
}
