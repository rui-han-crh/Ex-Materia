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

    public bool hasIcon = true;

    public bool interactOnceOnly = false;

    [SerializeField]
    private bool canInteract = true;

    public event Action EndedInteract = delegate { };

    [SerializeField]
    [RequireInterface(typeof(IInteraction))]
    private UnityEngine.Object[] interactions;

    private void OnEnable()
    {
        if (SaveFile.file.HasData(gameObject, typeof(Interactable), "hasIcon"))
        {
            hasIcon = SaveFile.file.Load<bool>(gameObject, typeof(Interactable), "hasIcon");
            Debug.Log($"Loaded {gameObject.name} hasIcon as {hasIcon}");
        }

        if (SaveFile.file.HasData(gameObject, typeof(Interactable), "canInteract")) 
        {
            canInteract = SaveFile.file.Load<bool>(gameObject, typeof(Interactable), "canInteract");
            Debug.Log($"Loaded {gameObject.name} canInteract as {canInteract}");
        }
    }

    private void OnDisable()
    {
        SaveFile.file.Save(gameObject, typeof(Interactable), "hasIcon", hasIcon);

        SaveFile.file.Save(gameObject, typeof(Interactable), "canInteract", canInteract);

        Debug.Log($"Saving {gameObject.name}, hasIcon {hasIcon} and canInteract {canInteract}");
    }

    private void Awake()
    {
        if (hasIcon)
        {
            icon = Instantiate(InteractableCollection.Instance.InteractIcon, InteractableCollection.Instance.Canvas.transform);
        }

        RepositionIcon();

        SetIconVisibility(false);
        InteractableCollection.Instance.Collection.Add(this);
    }

    public void SetInteractability(bool state)
    {
        canInteract = state;
    }

    public void RepositionIcon()
    {
        if (!hasIcon)
        {
            return;
        }

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

    public bool Interact(Interactable other)
    {
        if (!canInteract || !other.canInteract)
        {
            return false;
        }

        if (interactions.Length == 0)
        {
            Debug.LogError($"There were no interactions given to {this}. Either add at least 1 interaction, " +
                $"or remove this Monobehaviour from {name}");

            return false;
        }

        void OnFinishInteractions()
        {
            SetInteracting(false);
            other.SetInteracting(false);
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
                Debug.Log($"Beginning {interaction}");
                interaction.OnEnded += InvokeInteraction;
                interaction.Interact();
            }
        }

        InvokeInteraction();

        if (interactOnceOnly)
        {
            canInteract = false;
        }

        return true;
    }

    public void SetIconVisibility(bool state)
    {
        if (!hasIcon || !canInteract)
        {
            return;
        }

        icon.SetActive(state);
    }
}
