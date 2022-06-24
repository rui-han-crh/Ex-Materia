using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSceneTransition : MonoBehaviour, IInteraction
{
    public event Action OnEnded = delegate { };

    [SerializeField]
    private int sceneIndex;

    public void FlushEventHandlers()
    {
        OnEnded = delegate { };
    }

    public void Interact()
    {
        SceneTransitionManager.Instance.PrepareScene(sceneIndex);
        OnEnded();
        FlushEventHandlers();
    }
}
