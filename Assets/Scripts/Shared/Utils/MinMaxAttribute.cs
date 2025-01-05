using UnityEngine;

namespace Portfolio.Shared
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class MinMaxAttribute : PropertyAttribute
    {
        public float Min;
        public float Max;

        public MinMaxAttribute(float min, float max)
        {
            this.Min = min;
            this.Max = max;
        }
    }
}