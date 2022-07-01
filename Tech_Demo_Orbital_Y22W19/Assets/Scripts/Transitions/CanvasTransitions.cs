using Math.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Extensions;

namespace Transitions
{
    public static class CanvasTransitions
    {
        public static Task Fade(CanvasGroup canvasGroup, float start, float end, float time = 1)
        {
            canvasGroup.alpha = start;
            IEnumerator FadeEnumerator()
            {
                canvasGroup.alpha = start;
                float journeyLength = end - start;
                float startTime = Time.time;
                while (!Mathf.Approximately(canvasGroup.alpha, end))
                {
                    float currentTime = Time.time;
                    canvasGroup.alpha = Mathf.Lerp(start, end, (currentTime - startTime) / time);
                    yield return null;
                }
                canvasGroup.alpha = end;

                canvasGroup.blocksRaycasts = canvasGroup.alpha > 0;
            }
            return new Task(FadeEnumerator());
        }

        public static Task InterpolateLinearly(RectTransform rect, Vector2 startAnchor, Vector2 endAnchor, float time = 1)
        {
            IEnumerator LerpEnumerator()
            {
                rect.SetCenterAnchor(startAnchor);

                float startTime = Time.time;
                Vector2 currentAnchor = startAnchor;
                while (Vector2.Distance(currentAnchor, endAnchor) > Mathf.Epsilon)
                {
                    float currentTime = Time.time;
                    currentAnchor = Vector2.Lerp(startAnchor, endAnchor, (currentTime - startTime) / time);
                    rect.SetCenterAnchor(currentAnchor);
                    yield return null;
                }

                rect.SetCenterAnchor(endAnchor);
            }

            return new Task(LerpEnumerator());
        }

        public static Task ScaleByRatio(RectTransform rect, Vector2 halfAnchorDifference, float startRatio, float endRatio, float time = 1)
        {
            IEnumerator LerpEnumerator()
            {
                Vector2 centreAnchor = rect.CenterAnchor();

                rect.anchorMin = centreAnchor - halfAnchorDifference * startRatio;
                rect.anchorMax = centreAnchor + halfAnchorDifference * startRatio;

                float startTime = Time.time;
                float currentRatio = startRatio;



                while (!Mathf.Approximately(currentRatio, endRatio))
                {
                    float currentTime = Time.time;
                    currentRatio = Mathf.Lerp(startRatio, endRatio, (currentTime - startTime) / time);

                    rect.anchorMin = centreAnchor - halfAnchorDifference * currentRatio;
                    rect.anchorMax = centreAnchor + halfAnchorDifference * currentRatio;

                    yield return null;
                }
                
                rect.anchorMin = centreAnchor - halfAnchorDifference * endRatio;
                rect.anchorMax = centreAnchor + halfAnchorDifference * endRatio;
            }

            return new Task(LerpEnumerator());
        }
    }
}