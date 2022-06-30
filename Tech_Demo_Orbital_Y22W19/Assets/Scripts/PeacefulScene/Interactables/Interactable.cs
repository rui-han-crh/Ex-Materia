using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Transitions;
using System;
using System.Linq;

public class Interactable : MonoBehaviour
{
    private GameObject icon;
    private bool isInteracting;

    public bool IsInteracting => isInteracting;

    private int numberOfInteractionsInProgress;

    public event Action EndedInteract = delegate { };

    [SerializeField]
    [RequireInterface(typeof(IInteraction))]
    private UnityEngine.Object[] interactions;

    private void Awake()
    {
        icon = Instantiate(InteractableCollection.Instance.InteractIcon, InteractableCollection.Instance.Canvas.transform);

        RepositionIcon();

        SetIconVisibility(false);
        InteractableCollection.Instance.Collection.Add(this);

        Debug.Log($"Created {this.name} at {transform.position}");
    }

    public void RepositionIcon()
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.position + InteractableCollection.HEAD_OFFSET);
        icon.GetComponent<RectTransform>().position = screenPoint;
    }

    public void FlushEventHandlers()
    {
        EndedInteract = delegate { };
    }

    public void SetInteracting(bool state)
    {
        bool previous = isInteracting;
        isInteracting = state;

        if (!state && previous)
        {
            EndedInteract();
            FlushEventHandlers();
        }
    }

    public void Interact(Interactable other)
    {
        if (interactions.Length == 0)
        {
            Debug.LogError($"There were no interactions given to {this}. Either add at least 1 interaction, " +
                $"or remove this Monobehaviour from {name}");

            return;
        }

        void OnFinishInteractions()
        {
            SetInteracting(false);
            other.SetInteracting(false);

            EndedInteract();
            FlushEventHandlers();
        }

        SetInteracting(true);
        other.SetInteracting(true);

        IEnumerator enumerator = interactions.GetEnumerator();

        void InvokeInteraction()
        {
            if (!enumerator.MoveNext())
            {
                OnFinishInteractions();
            } 
            else
            {
                IInteraction interaction = (IInteraction)enumerator.Current;
                interaction.OnEnded += InvokeInteraction;
                interaction.Interact();
            }
        }
        InvokeInteraction();
    }

    public void SetIconVisibility(bool state)
    {
        icon.SetActive(state);
    }
}
