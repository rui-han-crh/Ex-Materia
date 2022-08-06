using System.Collections;
using System.Collections.Generic;
using Transitions;
using UnityEngine;
using UnityEngine.Extensions;
using UnityEngine.UI;

public class ScrollingText : MonoBehaviour
{
    [SerializeField]
    private bool horizontalScroll;

    [SerializeField]
    private bool oppositeDirection = false;

    [SerializeField]
    private float duration = 1f;

    [SerializeField]
    private float expandBoundsRatio = 1f;

    private LerpAnimation lerpAnimation;

    private bool wasDisabled;

    private Task scrolling;

    public void OnDisable()
    {
        lerpAnimation?.Stop();
        scrolling?.Stop();
        wasDisabled = true;
    }

    public void OnEnable()
    {
        if (wasDisabled)
            scrolling = new Task(Scroll());

        wasDisabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject parent = new GameObject($"{name}_mask", typeof(RectTransform), typeof(RectMask2D));

        parent.transform.SetParent(transform.parent);
        parent.transform.SetSiblingIndex(transform.GetSiblingIndex());

        RectTransform rect = GetComponent<RectTransform>();
        RectTransform parentRect = parent.GetComponent<RectTransform>();

        parentRect.anchorMax = rect.anchorMax;
        parentRect.anchorMin = rect.anchorMin;
        parentRect.anchoredPosition = rect.anchoredPosition;

        lerpAnimation = gameObject.AddComponent<LerpAnimation>();

        transform.SetParent(parentRect);
        rect.anchorMax = Vector2.one;
        rect.anchorMin = Vector2.zero;

        IEnumerator LateStart()
        {
            yield return new WaitForEndOfFrame();
            parentRect.offsetMin = Vector2.zero;
            parentRect.offsetMax = Vector2.zero;
            parentRect.localScale = Vector3.one;

            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.localScale = Vector3.one;

            yield return new WaitForEndOfFrame();

            Vector2 center = rect.CenterAnchor();

            Vector2 anchorBegin = horizontalScroll ?
                new Vector2(1 + center.x * expandBoundsRatio, center.y) :
                new Vector2(center.x, 1 + center.y * expandBoundsRatio);

            Vector2 anchorEnd = horizontalScroll ?
                new Vector2(-center.x * expandBoundsRatio, center.y) :
                new Vector2(center.x, -center.y * expandBoundsRatio);

            if (oppositeDirection)
            {
                lerpAnimation.SetActiveAnchor(anchorEnd);
                lerpAnimation.SetInactiveAnchor(anchorBegin);
            }
            else
            {
                lerpAnimation.SetActiveAnchor(anchorBegin);
                lerpAnimation.SetInactiveAnchor(anchorEnd);
            }

            lerpAnimation.StartAsActive = false;
            scrolling = new Task(Scroll());
        }

        StartCoroutine(LateStart());
    }

    private IEnumerator Scroll()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();

            lerpAnimation.Duration = 0f;
            lerpAnimation.SetAnimationState(true);

            yield return new WaitForEndOfFrame();

            lerpAnimation.Duration = duration;
            lerpAnimation.SetAnimationState(false);

            yield return new WaitForSeconds(lerpAnimation.Duration);
        }
    }
}
