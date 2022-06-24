using System.Collections;
using System.Collections.Generic;
using Transitions;
using UnityEngine;
using UnityEngine.Extensions;

public class ScaleFixedAspectAnimation : MonoBehaviour, ITransition
{
    [SerializeField]
    private float activeRatio;

    [SerializeField]
    private float inactiveRatio;

    [SerializeField]
    private float duration = 0.5f;

    [SerializeField]
    private bool startAsActive = false;

    private Task playingAnimation;

    [SerializeField]
    private bool isActive = false;

    private RectTransform rect;

    private float currentRatio;

    [HideInInspector]
    public bool IsActive => isActive;

    private Vector2 initialAnchorMin;
    private Vector2 initialAnchorMax;
    private Vector2 anchorDifference;

    private void Awake()
    {
        isActive = startAsActive;

        rect = GetComponent<RectTransform>();

        initialAnchorMin = rect.anchorMin;
        initialAnchorMax = rect.anchorMax;

        anchorDifference = initialAnchorMax - initialAnchorMin;

        if (!isActive) 
        {
            Vector2 centre = rect.CenterAnchor();
            rect.SetAnchors(centre, centre);
        }
    }

    public void ToggleAnimation()
    {
        SetAnimationState(!isActive);
    }

    private float GetCurrentRatio()
    {
        Vector2 halfAnchorDifference = (rect.anchorMax - rect.anchorMin) / 2;
        return halfAnchorDifference.magnitude;
    }

    public void SetAnimationState(bool state)
    {
        if (playingAnimation != null && playingAnimation.Running)
        {
            playingAnimation.Stop();
        }

        playingAnimation = CanvasTransitions.ScaleByRatio(
            rect,
            anchorDifference / 2,
            GetCurrentRatio(),
            state ? activeRatio : inactiveRatio,
            duration);

        isActive = state;
    }
}
