using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Extensions
{
    public static class RectTransformExtensions
    {
        public static Vector2 CenterAnchor(this RectTransform rect)
        {
            return (rect.anchorMin + rect.anchorMax) / 2;
        }

        public static void SetCenterAnchor(this RectTransform rect, Vector2 centerAnchor)
        {
            Vector2 halfDifference = (rect.anchorMax - rect.anchorMin) / 2;
            rect.anchorMin = centerAnchor - halfDifference;
            rect.anchorMax = centerAnchor + halfDifference;
        }

        public static void SetAnchors(this RectTransform rect, Vector2 minAnchor, Vector2 maxAnchor)
        {
            rect.anchorMin = minAnchor;
            rect.anchorMax = maxAnchor;
        }
    }
}
