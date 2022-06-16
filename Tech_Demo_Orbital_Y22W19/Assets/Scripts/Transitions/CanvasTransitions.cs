using Math.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Transitions
{
    public static class CanvasTransitions
    {
        public static void Fade(CanvasGroup canvasGroup, float start, float end, float time = 1)
        {
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
            }
            new Task(FadeEnumerator());
        }
    }
}