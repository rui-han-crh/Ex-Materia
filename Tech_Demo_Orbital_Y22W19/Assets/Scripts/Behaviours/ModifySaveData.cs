using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifySaveData : MonoBehaviour
{
    [SerializeField]
    private string componentName;

    [SerializeField]
    private string variableName;

    [SerializeField]
    private float floatValue;

    [SerializeField]
    private string stringValue;

    [SerializeField]
    private int intValue;

    [SerializeField]
    private bool boolValue;

    [SerializeField]
    public ValueTypes valueType;

    public enum ValueTypes
    {
        String = 0,
        Float = 1,
        Integer = 2,
        Bool = 3
    }

    public void OnModify()
    {
        Dictionary<string, Dictionary<string, object>> universalData = SaveFile.file.universalData;
        if (!universalData.ContainsKey(componentName))
        {
            universalData.Add(componentName, new Dictionary<string, object>());
        }

        switch (valueType)
        {
            case ValueTypes.String:
                universalData[componentName][variableName] = stringValue;
                break;

            case ValueTypes.Float:
                universalData[componentName][variableName] = floatValue;
                break;

            case ValueTypes.Integer:
                universalData[componentName][variableName] = intValue;
                break;

            case ValueTypes.Bool:
                universalData[componentName][variableName] = boolValue;
                break;
        }

        try
        {
            SmoothCameraFollow.Instance?.LoadData(); // temporary measure
        } 
        catch (Exception ex)
        {
            Debug.Log($"Skipped load of SmoothCameraFollow \n {ex.Message}");
        }
    }

    public void SetStringValue(string value)
    {
        stringValue = value;
        OnModify();
    }

    public void SetFloatValue(float value)
    {
        floatValue = value;
        OnModify();
    }

    public void SetIntegerValue(int value)
    {
        intValue = value;
        OnModify();
    }

    public void SetBoolValue(bool value)
    {
        boolValue = value;
        OnModify();
    }

    public bool HasValue()
    {
        return SaveFile.file.universalData.ContainsKey(componentName) 
            && SaveFile.file.universalData[componentName].ContainsKey(variableName);
    }

    public T FetchValue<T>()
    {
        if (!HasValue())
        {
            return default(T);
        }

        return (T)SaveFile.file.universalData[componentName][variableName];
    }
}
