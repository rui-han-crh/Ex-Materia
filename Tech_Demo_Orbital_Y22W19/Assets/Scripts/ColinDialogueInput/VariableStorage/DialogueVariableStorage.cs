using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Yarn.Unity;

public class DialogueVariableStorage : InMemoryVariableStorage
{
    Dictionary<string, float> floatDict = new Dictionary<string, float>();
    Dictionary<string, string> stringDict = new Dictionary<string, string>();
    Dictionary<string, bool> boolDict = new Dictionary<string, bool>();

    private void OnEnable()
    {
        if (!SaveFile.file.HasData(typeof(DialogueVariableStorage)))
        {
            return;
        }

        Dictionary<string, object> dict = SaveFile.file.Load(typeof(DialogueVariableStorage));

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
            else if (kvp.Value is float)
            {
                floatDict.Add(kvp.Key, (float)kvp.Value);
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

    private void OnDisable()
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
    }
}
