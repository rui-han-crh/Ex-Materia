using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class SaveFile
{
    public static SaveFile file = new SaveFile();

    public Dictionary<int, Dictionary<string, Dictionary<Type, Dictionary<string, object>>>> data
        = new Dictionary<int, Dictionary<string, Dictionary<Type, Dictionary<string, object>>>>();

    private SaveFile()
    {

    }

    public void Save(GameObject gameObject, Type type, string variableName, object value)
    {
        if (!data.ContainsKey(gameObject.scene.buildIndex))
        {
            data.Add(gameObject.scene.buildIndex, new Dictionary<string, Dictionary<Type, Dictionary<string, object>>>());
        }

        if (!data[gameObject.scene.buildIndex].ContainsKey(gameObject.name))
        {
            data[gameObject.scene.buildIndex].Add(gameObject.name, new Dictionary<Type, Dictionary<string, object>>());
        }

        if (!data[gameObject.scene.buildIndex][gameObject.name].ContainsKey(type))
        {
            data[gameObject.scene.buildIndex][gameObject.name].Add(type, new Dictionary<string, object>());
        }

        data[gameObject.scene.buildIndex][gameObject.name][type][variableName] = value;
    }

    public T Load<T>(GameObject gameObject, Type type, string variableName)
    {
        return (T)data[gameObject.scene.buildIndex][gameObject.name][type][variableName];
    }

    public bool HasData(GameObject gameObject, Type type, string variableName)
    {
        try
        {
            Load<object>(gameObject, type, variableName);
        } 
        catch (Exception)
        {
            return false;
        } 

        return true;
    }
}
