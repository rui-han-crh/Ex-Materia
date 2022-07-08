using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

public class SaveManager : MonoBehaviour
{
    private static SaveManager instance;

    public static SaveManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SaveManager>();
                Debug.Assert(instance != null, "There was no SaveManager in this scene, consider adding one?");
            }
            return instance;
        }
    }

    public static string directoryPath = "/Saves/";
    public static string fileName = "SaveFile.json";

    public void Awake()
    {
        Debug.Assert(instance == null, "There already exists a SaveManager in the scene, delete this Component!");
    }

    public void Start()
    {
        DeserialiseFromFile();
    }

    public static void SerialiseToFile()
    {
        IEnumerable<ISaveable> saveableObjects = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();

        foreach (ISaveable saveableObject in saveableObjects)
        {
            saveableObject.SaveData();
            Debug.Log($"Saved {(MonoBehaviour)saveableObject}");
        }

        string dir = Application.persistentDataPath + directoryPath;

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        Debug.Log(string.Join(", ", SaveFile.file.data.Select(kpv => kpv.Key + ": " + kpv.Value)));

        string jsonString = JsonConvert.SerializeObject(SaveFile.file);

        File.WriteAllText(dir + fileName, jsonString);
        Debug.Log("The save was successful");
    }

    public static void DeserialiseFromFile()
    {
        string fullPath = Application.persistentDataPath + directoryPath + fileName;

        if (File.Exists(fullPath))
        {
            string jsonString = File.ReadAllText(fullPath);
            SaveFile.file = JsonConvert.DeserializeObject<SaveFile>(jsonString);
        } 
        else
        {
            Debug.LogWarning($"There was no file at {fullPath}. Nothing was loaded in.");
        }

        IEnumerable<ISaveable> saveableObjects = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();

        foreach (ISaveable saveableObject in saveableObjects)
        {
            saveableObject.LoadData();
        }
    }
}
