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
        YarnManager.Instance.StartConvoAuto(yarnScriptName);
        YarnManager.Instance.OnEnded += InvokeDelegate;
    }

    private void InvokeDelegate()
    {
        CanvasTransitions.Fade(InteractableCollection.Instance.DialogueSystemCanvasGroup, 1, 0, time: 0.3f);
        OnEnded();
        FlushEventHandlers();
    }

    public void FlushEventHandlers()
    {
        OnEnded = delegate { };
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
