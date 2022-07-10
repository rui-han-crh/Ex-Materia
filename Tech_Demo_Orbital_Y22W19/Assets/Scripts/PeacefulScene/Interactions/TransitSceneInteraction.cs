using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitSceneInteraction : Interaction
{
    public string sceneName;

    public override void Interact()
    {
        SceneTransitionManager.Instance.ChangeScene(sceneName);
        OnEnd();
    }
}
