using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class VariableStorage : InMemoryVariableStorage
{
    Dictionary<string, float> floatDict = new Dictionary<string, float>();
    Dictionary<string, string> stringDict = new Dictionary<string, string>();
    Dictionary<string, bool> boolDict = new Dictionary<string, bool>();
    private void OnEnable()
    {
        if (SaveFile.file.HasData(typeof(VariableStorage), "floatDict"))
        {
            floatDict = SaveFile.file.Load<Dictionary<string, float>>(typeof(VariableStorage), "floatDict");
        }

        if (SaveFile.file.HasData(typeof(VariableStorage), "stringDict"))
        {
            stringDict = SaveFile.file.Load<Dictionary<string, string>>(typeof(VariableStorage), "stringDict");
        }

        if (SaveFile.file.HasData(typeof(VariableStorage), "boolDict"))
        {
            boolDict = SaveFile.file.Load<Dictionary<string, bool>>(typeof(VariableStorage), "boolDict");
        }

        SetAllVariables(floatDict, stringDict, boolDict);
    }

    private void OnDisable()
    {
        (floatDict, stringDict, boolDict) = GetAllVariables();

        SaveFile.file.Save(typeof(VariableStorage), "floatDict", floatDict);
        SaveFile.file.Save(typeof(VariableStorage), "stringDict", stringDict);
        SaveFile.file.Save(typeof(VariableStorage), "boolDict", boolDict);
    }
}
