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
        Trigger(collision);
    }

    private void Trigger(Collider2D collision)
    {
        Interactable collisionInteractable = collision.GetComponent<Interactable>();
        if (collisionInteractable == null
            || (triggerOnceOnly && numberOfTimesTriggered > 0))
        {
            return;
        }

        interactableToTrigger.Interact(collisionInteractable);
        numberOfTimesTriggered++;
    }
}
