using System;
using System.Collections;
using System.Collections.Generic;
using Transitions;
using UnityEngine;

public class ShowScreenObjectInteraction : MonoBehaviour, IInteraction
{
    public event Action OnEnded = delegate { };

    [SerializeField]
    private GameObject screenObjectToShow;

    public void Interact()
    {
        ScreenObjectManager.Instance.ShowObject(screenObjectToShow.name);
        ScreenObjectManager.Instance.OnEnded += () => OnEnded();
    }

    public void FlushEventHandlers()
    {
        OnEnded = delegate { };
    }
}
