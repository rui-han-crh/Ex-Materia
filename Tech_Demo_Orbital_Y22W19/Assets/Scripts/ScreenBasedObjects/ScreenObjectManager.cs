using System.Collections;
using System.Collections.Generic;
using Transitions;
using UnityEngine;
using Yarn.Unity;

public class ScreenObjectManager : MonoBehaviour
{
    private static ScreenObjectManager instance;

    private CanvasGroup currentActiveCanvasGroup;

    public event System.Action OnEnded = delegate { };

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

        foreach (CanvasGroup screenObject in screenObjects)
        {
            screenObjectsMapping[screenObject.name] = screenObject;
        }

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    [YarnCommand("ShowScreenObject")]
    public void ShowObject(string name)
    {
        if (!screenObjectsMapping.ContainsKey(name))
        {
            Debug.LogError($"There was no object given with the name {name}. Did you add a CanvasGroup component to {name}?");
        }

        currentActiveCanvasGroup = screenObjectsMapping[name];
        currentActiveCanvasGroup.gameObject.SetActive(true);
        CanvasTransitions.Fade(currentActiveCanvasGroup, 0, 1, 0.5f);
    }

    public void ShowObject(CanvasGroup canvasGroup)
    {
        if (!screenObjectsMapping.ContainsKey(canvasGroup.name))
        {
            screenObjectsMapping[canvasGroup.name] = Instantiate(canvasGroup, transform);
        }

        currentActiveCanvasGroup = screenObjectsMapping[canvasGroup.name];
        currentActiveCanvasGroup.gameObject.SetActive(true);
        CanvasTransitions.Fade(currentActiveCanvasGroup, 0, 1, 0.5f);
    }

    public void HideObject(string name)
    {
        if (!screenObjectsMapping.ContainsKey(name))
        {
            Debug.LogError($"There was no object given with the name {name}");
        }

        screenObjectsMapping[name].gameObject.SetActive(false);
    }

    public void HideObject(CanvasGroup canvasGroup)
    {
        canvasGroup.gameObject.SetActive(false);
    }

    public void HideCurrentActiveObject()
    {
        HideObject(currentActiveCanvasGroup);
        OnEnded();
        OnEnded = delegate { };
    }
}
