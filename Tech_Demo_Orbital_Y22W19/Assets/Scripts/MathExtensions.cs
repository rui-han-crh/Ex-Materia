using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExtensionMethods
{
    public static class MathExtensions
    {

        public static Vector3Int RotateVector(this Vector3Int vector, float radians)
        {
            float sin = Mathf.Sin(radians);
            float cos = Mathf.Cos(radians);

            return new Vector3Int(
                Mathf.RoundToInt(cos * vector.x - sin * vector.y),
                Mathf.RoundToInt(sin * vector.x + cos * vector.y),
                0);
        }

        public static bool Approximately(float a, float b, float threshold)
        {
            return ((a - b) < 0 ? ((a - b) * -1) : (a - b)) <= threshold;
        }
    }
}
