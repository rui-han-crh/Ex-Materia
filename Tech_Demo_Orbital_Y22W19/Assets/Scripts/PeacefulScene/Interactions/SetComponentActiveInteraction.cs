using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetComponentActiveInteraction : MonoBehaviour, IInteraction
{
    public event Action OnEnded = delegate { };

    [SerializeField]
    private MonoBehaviour behaviour;

    [SerializeField]
    private bool active;

    public void Interact()
    {
        behaviour.enabled = active;
        OnEnded();
    }

    public void FlushEventHandlers()
    {
        OnEnded = delegate { };
    }
}
