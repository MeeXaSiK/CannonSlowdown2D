using UnityEngine;

namespace Scripts.Core.Extensions
{
    public static class MathExtensions
    {
        public static float GetSquaredNumber(this float toSquare)
        {
            return Mathf.Pow(toSquare, 2);
        }
    }
}