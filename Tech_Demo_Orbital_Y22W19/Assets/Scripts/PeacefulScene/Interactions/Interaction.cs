using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interaction : MonoBehaviour, IInteraction
{
    public event Action OnEnded = delegate { };

    public void FlushEventHandlers()
    {
        OnEnded = delegate { };
    }

    public abstract void Interact();

    public void OnEnd()
    {
        OnEnded();
        FlushEventHandlers();
    }
}
