using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitionInteraction : MonoBehaviour, IInteraction
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
        SceneTransitionManager.Instance.SceneBuildIndex = sceneIndex;
        OnEnded();
        FlushEventHandlers();
    }
}
