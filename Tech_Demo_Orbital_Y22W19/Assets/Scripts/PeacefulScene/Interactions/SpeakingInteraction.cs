using System;
using System.Collections;
using System.Collections.Generic;
using Transitions;
using UnityEngine;

public class SpeakingInteraction : Interaction
{
    [SerializeField]
    private string yarnScriptName;

    public override void Interact()
    {
        CanvasTransitions.Fade(DialogueDisplayBehaviour.Instance.CanvasGroup, 0, 1, time: 0.3f);
        DialogueDisplayBehaviour.Instance.CanvasGroup.interactable = true;
        DialogueDisplayBehaviour.Instance.CanvasGroup.blocksRaycasts = true;
        YarnManager.Instance.StartConvoAuto(yarnScriptName);
        YarnManager.Instance.OnEnded += InvokeDelegate;
    }

    private void InvokeDelegate()
    {
        CanvasTransitions.Fade(DialogueDisplayBehaviour.Instance.CanvasGroup, 1, 0, time: 0.3f);
        DialogueDisplayBehaviour.Instance.CanvasGroup.interactable = false;
        DialogueDisplayBehaviour.Instance.CanvasGroup.blocksRaycasts = false;
        OnEnd();
    }
}
