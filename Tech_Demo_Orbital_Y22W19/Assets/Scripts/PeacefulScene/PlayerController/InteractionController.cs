using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Interactable), typeof(MovementController))]
public class InteractionController : MonoBehaviour
{
    private KeyboardControls keyboardControls;
    private Interactable thisInteractable;
    private MovementController movementController;

    [SerializeField]
    private float permittedRadius = 1;

    private List<Interactable> drawnInteractables = new List<Interactable>();

    private void OnEnable()
    {
        keyboardControls?.Enable();
    }

    private void OnDisable()
    {
        keyboardControls?.Disable();
    }

    private void Awake()
    {
        keyboardControls = new KeyboardControls();
        keyboardControls.Mouse.Interact.performed += _ => Interact();
        thisInteractable = GetComponent<Interactable>();
        movementController = GetComponent<MovementController>();
    }

    private void Update()
    {
        CheckForInteractables();
    }

    private void CheckForInteractables()
    {
        IEnumerable<Interactable> allInteractables = GetAllInteractables(permittedRadius);

        IEnumerable<Interactable> noLongerInRange = drawnInteractables.Where(x => !allInteractables.Contains(x)).ToList();
        IEnumerable<Interactable> newInRange = allInteractables.Where(x => !drawnInteractables.Contains(x)).ToList();

        foreach (Interactable interactable in noLongerInRange)
        {
            interactable.SetIconVisibility(false);
            drawnInteractables.Remove(interactable);
        }

        foreach (Interactable interactable in newInRange)
        {
            if (thisInteractable.Equals(interactable))
            {
                continue;
            }

            interactable.SetIconVisibility(true);
            drawnInteractables.Add(interactable);
        }
        
        foreach (Interactable interactable in allInteractables)
        {
            interactable.RepositionIcon();
        }

    }

    private void Interact()
    {
        if (thisInteractable.IsInteracting)
        {
            return;
        }

        IEnumerable<Interactable> allInteractables = GetAllInteractables(permittedRadius);
        if (allInteractables.Count() == 0)
        {
            return;
        }

        if (!GetAllInteractables(permittedRadius).Any(x => x.Interact(thisInteractable)))
        {
            return;
        }

        AfterInteraction();
    }

    public void InteractUsing(Interactable interactable)
    {
        if (!interactable.Interact(thisInteractable))
        {
            return;
        }

        AfterInteraction();
    }

    private void AfterInteraction()
    {
        float savedRadius = permittedRadius;
        permittedRadius = 0;
        CheckForInteractables();

        thisInteractable.EndedInteract += () =>
        {
            permittedRadius = savedRadius;
            movementController.OnEnable();
        };

        movementController.OnDisable();
    }


    private IEnumerable<Interactable> GetAllInteractables(float radius)
    {
        ICollection<Interactable> interactables = new List<Interactable>();

        foreach (Interactable interactable in InteractableCollection.Instance.Collection)
        {
            if (interactable.Equals(thisInteractable))
            {
                continue;
            }

            if (Vector2.Distance(transform.position, interactable.transform.position) < radius && !interactable.IsInteracting)
            {
                interactables.Add(interactable);
            }
        }

        return interactables;
    }
}
