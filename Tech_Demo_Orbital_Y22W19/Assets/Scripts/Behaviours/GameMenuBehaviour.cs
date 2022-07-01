using System.Collections;
using System.Collections.Generic;
using Transitions;
using UnityEngine;

public class GameMenuBehaviour : MonoBehaviour
{
    private KeyboardControls keyboardControls;

    private bool menuOn;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        keyboardControls = new KeyboardControls();

        keyboardControls.Mouse.Tab.performed += _ => MenuScreenSetActive();

        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnEnable()
    {
        keyboardControls?.Enable();
    }

    public void OnDisable()
    {
        keyboardControls?.Disable();
    }

    public void MenuScreenSetActive()
    {
        menuOn = !menuOn;

        foreach (Transform child in transform)
        {
            child.GetComponent<LerpAnimation>().SetAnimationState(menuOn);
        }

        if (menuOn)
        {
            canvasGroup.interactable = true;
            CanvasTransitions.Fade(canvasGroup, 0, 1, 0.5f);
        } 
        else
        {
            canvasGroup.interactable = false;
            CanvasTransitions.Fade(canvasGroup, 1, 0, 0.5f);
        }
    }

}
