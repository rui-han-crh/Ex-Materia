using System.Collections;
using System.Collections.Generic;
using Transitions;
using UnityEngine;
using UnityEngine.Extensions;

[RequireComponent(typeof(CanvasGroup))]
public class FadeAnimation : MonoBehaviour, ITransition
{
    [SerializeField]
    private float activeAlpha;

    [SerializeField]
    private float inactiveAlpha;

    [SerializeField]
    private float activationDuration = 0.5f;

    [SerializeField]
    private float deactivationDuration = 0.5f;

    [SerializeField]
    private bool startAsActive = false;

    private Task playingAnimation;

    private bool isActive = false;

    private CanvasGroup canvasGroup;

    [HideInInspector]
    public bool IsActive => isActive;

    private void Awake()
    {
        isActive = startAsActive;
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = startAsActive ? activeAlpha : inactiveAlpha;
    }

    public void ToggleAnimation()
    {
        SetAnimationState(!isActive);
    }

    public void SetAnimationState(bool state)
    {
        if (playingAnimation != null && playingAnimation.Running)
        {
            playingAnimation.Stop();
        }

        if (state)
        {
            playingAnimation = CanvasTransitions.Fade(canvasGroup, canvasGroup.alpha, activeAlpha, activationDuration);
        } 
        else
        {
            playingAnimation = CanvasTransitions.Fade(canvasGroup, canvasGroup.alpha, inactiveAlpha, deactivationDuration);
        }

        isActive = state;
    }
}
