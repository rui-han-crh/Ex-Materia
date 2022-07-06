using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueDisplayBehaviour : MonoBehaviour
{
    private static DialogueDisplayBehaviour instance;

    private CanvasGroup canvasGroup;

    public CanvasGroup CanvasGroup => canvasGroup;
    public static DialogueDisplayBehaviour Instance
    {
        get
        {
            instance = FindObjectOfType<DialogueDisplayBehaviour>();
            Debug.Assert(instance != null, "Did you add the dialogue display into the scene? " +
                "If so, is DialogueDisplayBehaviour an added component?");
            return instance;
        }
    }


    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
    }
}
