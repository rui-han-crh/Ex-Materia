using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSceneTransition : MonoBehaviour, IInteraction
{
    public event Action OnEnded = delegate { };

    [SerializeField]
    private string sceneName;

    public void FlushEventHandlers()
    {
        OnEnded = delegate { };
    }

    public void Interact()
    {
        SceneTransitionManager.Instance.PrepareScene(sceneName);
        OnEnded();
        FlushEventHandlers();
    }
}
