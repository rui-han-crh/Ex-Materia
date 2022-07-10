using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Yarn.Unity;

public class DialogueVariableStorage : InMemoryVariableStorage, ISaveable
{
    Dictionary<string, float> floatDict = new Dictionary<string, float>();
    Dictionary<string, string> stringDict = new Dictionary<string, string>();
    Dictionary<string, bool> boolDict = new Dictionary<string, bool>();

    public void LoadData()
    {
        if (!SaveFile.file.HasData(typeof(DialogueVariableStorage)))
        {
            Debug.Log("No prior DialogueVariableStorage was found in save");
            return;
        }

        Dictionary<string, object> dict = SaveFile.file.Load(typeof(DialogueVariableStorage));

        Debug.Log($"Loading in {dict.Count} variables");

        foreach (KeyValuePair<string, object> kvp in dict)
        {
            if (kvp.Value is string)
            {
                stringDict.Add(kvp.Key, (string)kvp.Value);
            } 
            else if (kvp.Value is bool)
            {
                boolDict.Add(kvp.Key, (bool)kvp.Value);
            }
            else if (kvp.Value is float || kvp.Value is double)
            {
                floatDict.Add(kvp.Key, Convert.ToSingle(kvp.Value));
            } 
            else
            {
                Debug.LogWarning($"A variable called {kvp.Key} had a type of {kvp.Value.GetType()}, but was not accounted for. No loading was done");
            }
        }

        //if (SaveFile.file.HasData(typeof(VariableStorage), "floatDict"))
        //{
        //    floatDict = SaveFile.file.Load<Dictionary<string, float>>(typeof(VariableStorage), "floatDict");
        //}

        //if (SaveFile.file.HasData(typeof(VariableStorage), "stringDict"))
        //{
        //    stringDict = SaveFile.file.Load<Dictionary<string, string>>(typeof(VariableStorage), "stringDict");
        //}

        //if (SaveFile.file.HasData(typeof(VariableStorage), "boolDict"))
        //{
        //    boolDict = SaveFile.file.Load<Dictionary<string, bool>>(typeof(VariableStorage), "boolDict");
        //}

        SetAllVariables(floatDict, stringDict, boolDict);
        Debug.Log(string.Join(", ", boolDict.Select(kvp => kvp.Key + ": " + kvp.Value.ToString())));
    }

    public void SaveData()
    {
        (floatDict, stringDict, boolDict) = GetAllVariables();

        foreach (KeyValuePair<string, float> kvp in floatDict)
        {
            SaveFile.file.Save(typeof(DialogueVariableStorage), kvp.Key, kvp.Value);
        }

        foreach (KeyValuePair<string, string> kvp in stringDict)
        {
            SaveFile.file.Save(typeof(DialogueVariableStorage), kvp.Key, kvp.Value);
        }

        foreach (KeyValuePair<string, bool> kvp in boolDict)
        {
            SaveFile.file.Save(typeof(DialogueVariableStorage), kvp.Key, kvp.Value);
        }

        Debug.Log($"Saved {floatDict.Count} float, {stringDict.Count} string and {boolDict.Count} bool variables");
    }
}
