using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetComponentActiveInteraction : Interaction
{

    [SerializeField]
    private MonoBehaviour behaviour;

    [SerializeField]
    private bool active;

    public override void Interact()
    {
        behaviour.enabled = active;
        OnEnd();
    }
}
