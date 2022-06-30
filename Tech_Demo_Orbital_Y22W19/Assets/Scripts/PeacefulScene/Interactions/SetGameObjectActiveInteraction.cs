using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetGameObjectActiveInteraction : MonoBehaviour, IInteraction
{
    public event Action OnEnded = delegate { };

    [SerializeField]
    private GameObject gameObject;

    [SerializeField]
    private bool active;

    public void Interact()
    {
        gameObject.SetActive(active);
        OnEnded();
    }

    public void FlushEventHandlers()
    {
        OnEnded = delegate { };
    }
}
