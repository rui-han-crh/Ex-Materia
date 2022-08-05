using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Linq;
using System.Linq;

[Serializable]
public class SaveFile
{
    public static SaveFile file = new SaveFile();

    public Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, object>>>> data
        = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, object>>>>();

    public Dictionary<string, Dictionary<string, object>> universalData
        = new Dictionary<string, Dictionary<string, object>>();

    private SaveFile()
    {

    }

    public void Save(GameObject gameObject, Type type, string variableName, object value)
    {
        Debug.Log($"The scene name is {gameObject.scene.name}");
        if (!data.ContainsKey(gameObject.scene.name))
        {
            data.Add(gameObject.scene.name, new Dictionary<string, Dictionary<string, Dictionary<string, object>>>());
        }

        if (!data[gameObject.scene.name].ContainsKey(gameObject.name))
        {
            data[gameObject.scene.name].Add(gameObject.name, new Dictionary<string, Dictionary<string, object>>());
        }

        if (!data[gameObject.scene.name][gameObject.name].ContainsKey(type.Name))
        {
            data[gameObject.scene.name][gameObject.name].Add(type.Name, new Dictionary<string, object>());
        }

        data[gameObject.scene.name][gameObject.name][type.Name][variableName] = value;
    }

    public void Save(Type type, string variableName, object value)
    {
        if (!universalData.ContainsKey(type.Name))
        {
            universalData.Add(type.Name, new Dictionary<string, object>());
        }

        universalData[type.Name][variableName] = value;
    }

    public T Load<T>(GameObject gameObject, Type type, string variableName)
    {
        object target = data[gameObject.scene.name][gameObject.name][type.Name][variableName];
        if (target is JArray)
        {
            data[gameObject.scene.name][gameObject.name][type.Name][variableName] = ((JArray)target).ToObject<T>();
        }

        return (T)data[gameObject.scene.name][gameObject.name][type.Name][variableName];
    }

    public object Load(GameObject gameObject, Type type, string variableName)
    {
        return Load<object>(gameObject, type, variableName);
    }

    public Dictionary<string, object> Load(GameObject gameObject, Type type)
    {
        return data[gameObject.scene.name][gameObject.name][type.Name];
    }

    public T Load<T>(Type type, string variableName)
    {
        object target = universalData[type.Name][variableName];

        if (target is JArray)
        {
            universalData[type.Name][variableName] = ((JArray)target).ToObject<T>();
        }

        if (target is double && typeof(T).IsAssignableFrom(typeof(float)))
        {
            universalData[type.Name][variableName] = Convert.ToSingle(target);
        }

        try
        {
            return (T)universalData[type.Name][variableName];
        } 
        catch (InvalidCastException ex)
        {
            throw new InvalidCastException($"Could not convert {universalData[type.Name][variableName].GetType()} to {typeof(T)} \n" + ex.Message);
        }
    }

    public object Load(Type type, string variableName)
    {
        return Load<object>(type, variableName);
    }

    public Dictionary<string, object> Load(Type type)
    {
        return new Dictionary<string, object>(universalData[type.Name]);
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

    public bool HasData(GameObject gameObject, Type type)
    {
        try
        {
            Load(gameObject, type);
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    public bool HasData(Type type, string variableName)
    {
        try
        {
            Load(type, variableName);
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    public bool HasData(Type type)
    {
        try
        {
            Load(type);
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    public static void Save()
    {
        IEnumerable<ISaveable> saveableObjects = UnityEngine.Object.FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();

        foreach (ISaveable saveableObject in saveableObjects)
        {
            saveableObject.SaveData();
            Debug.Log($"Saved {(MonoBehaviour)saveableObject}");
        }
    }

    public static void Load()
    {
        IEnumerable<ISaveable> saveableObjects = UnityEngine.Object.FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();

        foreach (ISaveable saveableObject in saveableObjects)
        {
            Debug.Log($"Loaded {saveableObject}");
            saveableObject.LoadData();
        }
    }
}
