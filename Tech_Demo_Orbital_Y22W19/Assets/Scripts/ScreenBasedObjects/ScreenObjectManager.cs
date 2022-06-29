using System.Collections;
using System.Collections.Generic;
using Transitions;
using UnityEngine;
using Yarn.Unity;

public class ScreenObjectManager : MonoBehaviour
{
    private static ScreenObjectManager instance;

    private string currentActiveObject;

    public static ScreenObjectManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ScreenObjectManager>();
                Debug.Assert(instance != null, $"There was no ScreenObjectManager in this scene, consider adding one");
            }
            return instance;
        }
    }


    private CanvasGroup[] screenObjects;

    private Dictionary<string, CanvasGroup> screenObjectsMapping = new Dictionary<string,CanvasGroup> ();

    private void Awake()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }

        screenObjects = GetComponentsInChildren<CanvasGroup>();

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        foreach (CanvasGroup screenObject in screenObjects)
        {
            screenObjectsMapping[screenObject.name] = screenObject;
        }
    }

    [YarnCommand("ShowScreenObject")]
    public void ShowObject(string name)
    {
        if (!screenObjectsMapping.ContainsKey(name))
        {
            Debug.LogError($"There was no object given with the name {name}");
        }

        currentActiveObject = name;
        screenObjectsMapping[name].gameObject.SetActive(true);
        CanvasTransitions.Fade(screenObjectsMapping[name], 0, 1, 0.5f);
    }

    public void HideObject(string name)
    {
        if (!screenObjectsMapping.ContainsKey(name))
        {
            Debug.LogError($"There was no object given with the name {name}");
        }

        screenObjectsMapping[name].gameObject.SetActive(false);
    }

    public void HideCurrentActiveObject()
    {
        HideObject(currentActiveObject);
    }
}
