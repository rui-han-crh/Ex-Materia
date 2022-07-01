using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSceneTransition : Interaction
{

    [SerializeField]
    private string sceneName;

    public override void Interact()
    {
        SceneTransitionManager.Instance.PrepareScene(sceneName);
        OnEnd();
        FlushEventHandlers();
    }
}
