using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TriggerInteractable : MonoBehaviour
{
    [SerializeField]
    private Interactable interactableToTrigger;
    [SerializeField]
    private bool triggerOnceOnly;
    private int numberOfTimesTriggered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("trigger!");
        Trigger(collision);
    }

    private void Trigger(Collider2D collision)
    {
        InteractionController interactionController = collision.GetComponent<InteractionController>();
        if (interactionController == null
            || (triggerOnceOnly && numberOfTimesTriggered > 0))
        {
            return;
        }

        interactionController.InteractUsing(interactableToTrigger);
        numberOfTimesTriggered++;
    }
}
