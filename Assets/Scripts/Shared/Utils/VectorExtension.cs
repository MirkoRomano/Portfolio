using UnityEngine;

namespace Portfolio
{
    public static class VectorExtension
    {
        /// <summary>
        /// Rotate a point around a pivot
        /// </summary>
        /// <param name="v">Vector3 to rotate</param>
        /// <param name="pivot">Rotation pivot</param>
        /// <param name="angle">Rotation angle</param>
        /// <param name="distance">Diistance from the pivot</param>
        /// <param name="axis">Rotation axis</param>
        /// <param name="rotateClockwise">Rotation direction</param>
        public static Vector3 RotateAround(this Vector3 v, Vector3 pivot, float angle, float distance, Vector3 axis, bool rotateClockwise = true)
        {
            Vector3 referenceVector = Vector3.right;
            if (Vector3.Dot(referenceVector, axis) > 0.99f)
            {
                referenceVector = Vector3.up;
            }

            Vector3 offset = Vector3.Cross(axis, referenceVector).normalized * distance;
            Vector3 translatedPoint = pivot + offset;
            Quaternion rotation = Quaternion.AngleAxis(rotateClockwise ? angle : -angle, axis);
            Vector3 result = rotation * (translatedPoint - pivot) + pivot;

            //Because the position of the point is rotated of 180 degrees, return the opposite vector
            return -result;
        }

    }
}