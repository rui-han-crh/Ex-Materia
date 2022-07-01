using System;
using System.Collections;
using System.Collections.Generic;
using Transitions;
using UnityEngine;

public class ShowScreenObjectInteraction : Interaction
{
    [SerializeField]
    private GameObject screenObjectToShow;

    public override void Interact()
    {
        ScreenObjectManager.Instance.ShowObject(screenObjectToShow.name);
        ScreenObjectManager.Instance.OnEnded += () => OnEnd();
    }
}
