using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetGameObjectActiveInteraction : Interaction
{
    [SerializeField]
    private GameObject gameObjectToSet;

    [SerializeField]
    private bool active;

    public override void Interact()
    {
        gameObjectToSet.SetActive(active);
        OnEnd();
    }
}
