using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventInteraction : Interaction
{
    [SerializeField]
    protected UnityEvent unityEvent;

    public override void Interact()
    {
        unityEvent.Invoke();
        OnEnd();
    }
}
