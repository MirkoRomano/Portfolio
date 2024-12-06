using UnityEngine;

namespace Portfolio
{
    public static class MathUtility
    {
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