using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetInteractabilityInteraction : Interaction
{
    [SerializeField]
    private Interactable interactable;

    [SerializeField]
    private bool interactability;

    public override void Interact()
    {
        interactable.SetInteractability(interactability);
    }
}
