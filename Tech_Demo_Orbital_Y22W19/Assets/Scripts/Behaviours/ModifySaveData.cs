using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private Type fetchedType;

    public void Awake()
    {
        fetchedType = Type.GetType(componentName);
        bool isMonoBehaviour = typeof(MonoBehaviour).IsAssignableFrom(fetchedType);

        if (!isMonoBehaviour)
        {
            Debug.LogWarning($"{componentName} does not extend from MonoBehaviour, please check that the name you provided is correct.");
        }
    }

    public void OnModify()
    {
        Dictionary<string, Dictionary<string, object>> universalData = SaveFile.file.universalData;
        

        switch (valueType)
        {
            case ValueTypes.String:
                SaveFile.file.Save(fetchedType, variableName, stringValue);
                break;

            case ValueTypes.Float:
                SaveFile.file.Save(fetchedType, variableName, floatValue);
                break;

            case ValueTypes.Integer:
                SaveFile.file.Save(fetchedType, variableName, intValue);
                break;

            case ValueTypes.Bool:
                SaveFile.file.Save(fetchedType, variableName, boolValue);
                break;
        }

        Debug.Log($"Saved {fetchedType} : {variableName}");


        IEnumerable<ISaveable> saveables = FindObjectsOfType(fetchedType).OfType<ISaveable>();

        foreach (ISaveable saveable in saveables)
        {
            saveable.LoadData();
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
        return SaveFile.file.HasData(fetchedType, variableName);
    }

    public T FetchValue<T>()
    {
        if (!HasValue())
        {
            return default(T);
        }

        return SaveFile.file.Load<T>(fetchedType, variableName);
    }
}
