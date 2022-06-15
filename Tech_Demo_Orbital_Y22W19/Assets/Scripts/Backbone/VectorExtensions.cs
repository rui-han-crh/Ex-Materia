using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.Extensions
{
    public static class VectorExtensions
    {
        public static Vector3Int Rotate(this Vector3Int source, float radians)
        {
            float sin = Mathf.Sin(radians);
            float cos = Mathf.Cos(radians);

            return new Vector3Int(
                Mathf.RoundToInt(cos * source.x - sin * source.y),
                Mathf.RoundToInt(sin * source.x + cos * source.y),
                0);
        }
    }
}
