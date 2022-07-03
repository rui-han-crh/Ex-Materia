using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventTriggerInteraction : Interaction
{
    public UnityEvent unityEvent;

    public override void Interact()
    {
        unityEvent.Invoke();
    }
}
