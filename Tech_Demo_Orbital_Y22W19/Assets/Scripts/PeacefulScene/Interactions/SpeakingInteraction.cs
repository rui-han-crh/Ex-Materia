using System;
using System.Collections;
using System.Collections.Generic;
using Transitions;
using UnityEngine;

public class SpeakingInteraction : MonoBehaviour, IInteraction
{
    public event Action OnEnded = delegate { };

    [SerializeField]
    private string yarnScriptName;

    public void Interact()
    {
        CanvasTransitions.Fade(InteractableCollection.Instance.DialogueSystemCanvasGroup, 0, 1, time: 0.3f);
        InteractableCollection.Instance.DialogueSystemCanvasGroup.interactable = true;
        YarnManager.Instance.StartConvoAuto(yarnScriptName);
        YarnManager.Instance.OnEnded += InvokeDelegate;
    }

    private void InvokeDelegate()
    {
        CanvasTransitions.Fade(InteractableCollection.Instance.DialogueSystemCanvasGroup, 1, 0, time: 0.3f);
        InteractableCollection.Instance.DialogueSystemCanvasGroup.interactable = false;
        OnEnded();
        FlushEventHandlers();
    }

    public void FlushEventHandlers()
    {
        OnEnded = delegate { };
    }
}