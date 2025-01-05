using UnityEngine;

namespace Portfolio
{
    public static class MathUtility
    {
        /// <summary>
        /// irrational number (1 + Square root of√5)/2
        /// </summary>
        public const float GOLDEN_RATIO = 0.618033f;

        /// <summary>
        /// Clamp a value between -1 and 1
        /// </summary>
        /// <param name="value">Value to clamp</param>
        public static float Clamp11(float value)
        {
            return Mathf.Clamp(value, -1.0f, 1.0f);
        }
    }
}