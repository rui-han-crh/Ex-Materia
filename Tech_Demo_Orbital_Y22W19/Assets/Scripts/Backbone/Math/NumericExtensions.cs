using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Math.Extensions
{
    public static class NumericExtensions
    {
        public static bool Approximately(float a, float b, float threshold)
        {
            return ((a - b) < 0 ? ((a - b) * -1) : (a - b)) <= threshold;
        }
    }
}
