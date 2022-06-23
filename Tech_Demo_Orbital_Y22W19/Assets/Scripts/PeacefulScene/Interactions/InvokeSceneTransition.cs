using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvokeSceneTransition : MonoBehaviour, IInteraction
{
    public event Action OnEnded = delegate { };

    public void Interact()
    {
        SceneTransitionManager.Instance.TransitionToScene();
    }

    public void FlushEventHandlers()
    {
        OnEnded = delegate { };
    }
}
