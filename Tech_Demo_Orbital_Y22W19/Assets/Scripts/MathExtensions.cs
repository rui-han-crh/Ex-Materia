using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ExtensionMethods
{
    public static class MathExtensions
    {
        public static readonly float EULER_CONSTANT = 2.71828182845904523536f;

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

        /// <summary>
        /// Smooths an curve within domain [-alpha, alpha] by an average gradient of beta. 
        /// The provided argument x must lie between -alpha and alpha inclusive.
        /// Values beyond the domain will be clamped into the domain interval.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <returns></returns>
        public static float InverseLogit(float x, float alpha, float beta)
        {
            x = Mathf.Clamp(x, -alpha, alpha);

            float sign = Mathf.Sign(x);
            float adjustedX = Mathf.Abs(x) / alpha;
            return 1.0f / (1.0f + Mathf.Pow((adjustedX / (1 - adjustedX)), -beta)) * sign;
        }
    }
}
