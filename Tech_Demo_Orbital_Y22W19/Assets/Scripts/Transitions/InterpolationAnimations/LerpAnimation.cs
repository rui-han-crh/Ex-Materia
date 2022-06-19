using System.Collections;
using System.Collections.Generic;
using Transitions;
using UnityEngine;
using UnityEngine.Extensions;


namespace Transitions
{
    public class LerpAnimation : MonoBehaviour, ITransition
    {
        [SerializeField]
        private Vector2 activeAnchor;

        [SerializeField]
        private Vector2 inactiveAnchor;

        public Vector2 ActiveAnchor => activeAnchor;

        public Vector2 InactiveAnchor => inactiveAnchor;


        [SerializeField]
        private float duration = 0.5f;

        [SerializeField]
        private bool startAsActive = false;


        private Task playingAnimation;

        private bool isActive = false;

        private RectTransform rect;

        [HideInInspector]
        public bool IsActive => isActive;

        public void SetActiveAnchor(Vector2 anchor)
        {
            activeAnchor = anchor;
        }

        public void SetInactiveAnchor(Vector2 anchor)
        {
            inactiveAnchor = anchor;
        }

        public void Awake()
        {
            isActive = startAsActive;
            rect = GetComponent<RectTransform>();
            rect.SetCenterAnchor(isActive ? activeAnchor : inactiveAnchor);
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

            Vector2 startAnchor = (rect.anchorMin + rect.anchorMax) / 2;
            playingAnimation = CanvasTransitions.InterpolateLinearly(rect, startAnchor, state ? activeAnchor : inactiveAnchor, duration);
            isActive = state;
        }
    }
}
