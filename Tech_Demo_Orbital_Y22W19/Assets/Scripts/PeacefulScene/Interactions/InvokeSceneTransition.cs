using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvokeSceneTransition : Interaction
{
    public override void Interact()
    {
        SceneTransitionManager.Instance.TransitionToScene();
        OnEnd();
    }
}
