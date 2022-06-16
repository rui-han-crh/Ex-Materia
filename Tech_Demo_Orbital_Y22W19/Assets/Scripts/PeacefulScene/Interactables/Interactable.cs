using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Transitions;

public class Interactable : MonoBehaviour
{
    private GameObject icon;

    private void Awake()
    {
        icon = Instantiate(InteractableCollection.Instance.InteractIcon, InteractableCollection.Instance.Canvas.transform);
        Reposition();
        SetIconVisibility(false);
        InteractableCollection.Instance.Collection.Add(this);
    }

    public void Reposition()
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.position + InteractableCollection.HEAD_OFFSET);
        icon.GetComponent<RectTransform>().position = screenPoint;
    }

    public void Interact(Interactable other)
    {
        Debug.Log($"{this} interacting with {other}");
        CanvasTransitions.Fade(InteractableCollection.Instance.DialogueSystemCanvasGroup, 0, 1);

        YarnManager.Instance.StartConvoAuto("StartEvelynAndOlivia");
    }

    public void SetIconVisibility(bool state)
    {
        icon.SetActive(state);
    }
}
