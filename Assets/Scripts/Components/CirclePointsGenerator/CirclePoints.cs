using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Portfolio
{
    public class CirclePoints : MonoBehaviour, IEnumerable<Vector3>
    {
        /// <summary>
        /// Entire angle in degrees
        /// </summary>
        private const float ANGLE_FULL_CIRCLE = 360.0f;

        /// <summary>
        /// Radius of the circle of points
        /// </summary>
        [SerializeField]
        private float radius;

        /// <summary>
        /// Number of points to create and rotate
        /// </summary>
        [SerializeField]
        private int numberOfPoints;

        /// <summary>
        /// Rotation directin of the vector
        /// </summary>
        [SerializeField]
        private Vector3 rotationDirection;

        /// <summary>
        /// Generated points
        /// </summary>
        [SerializeField]
        private Vector3[] points;

        /// <summary>
        /// Rotation angle of a single point
        /// </summary>
        private float angleStep => ANGLE_FULL_CIRCLE / numberOfPoints;

        /// <summary>
        /// Current rotation degrees of the circle points
        /// </summary>
        private float currentRotationDegrees = 0;

        /// <summary>
        /// Points count
        /// </summary>
        public int Count => numberOfPoints;
        
        /// <summary>
        /// Is readonly
        /// </summary>
        public bool IsReadOnly => true;

        /// <summary>
        /// Array index
        /// </summary>
        /// <param name="index">Index of the point</param>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        public Vector3 this[int index]
        {
            get
            {
                if (index < 0 || index >= points.Length)
                {
                    throw new System.IndexOutOfRangeException($"Index {index} is out of range.");
                }

                return points[index];
            }
        }

        /// <summary>
        /// Enumerator for points
        /// </summary>
        public IEnumerator<Vector3> GetEnumerator()
        {
            return ((IEnumerable<Vector3>)points).GetEnumerator();
        }

        /// <summary>
        /// Enumerator for points (non-generic version)
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return points.GetEnumerator();
        }


        /// <summary>
        /// Rotate every points instantaneously by an angle
        /// </summary>
        /// <param name="angle">Rotation angle</param>
        public void Rotate(float angle)
        {
            currentRotationDegrees = Mathf.Repeat(currentRotationDegrees + angle, ANGLE_FULL_CIRCLE);
            
            for (int i = 0; i < numberOfPoints; i++)
            {
                float pointAngle = Mathf.Repeat(currentRotationDegrees + (angleStep * i), ANGLE_FULL_CIRCLE);
                points[i] = points[i].RotateAround(transform.position, pointAngle, radius, rotationDirection);
            }
        }

        /// <summary>
        /// Rotate every point around the pivot for n seconds
        /// </summary>
        /// <param name="angle">Rotation angle</param>
        /// <param name="durationInSeconds">Rotation time in seconds</param>
        public IEnumerator RotateCoroutine(float angle, float durationInSeconds)
        {
            float startRotation = currentRotationDegrees;
            float targetRotation = Mathf.Repeat(startRotation + angle, ANGLE_FULL_CIRCLE);
            float elapsedTime = 0;

            //Prevent wrong rotation direction
            float rotationDifference = Mathf.Repeat(targetRotation - startRotation, ANGLE_FULL_CIRCLE);
            if (rotationDifference > ANGLE_FULL_CIRCLE / 2) 
            {
                rotationDifference -= ANGLE_FULL_CIRCLE;
            } 

            while (elapsedTime < durationInSeconds)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / durationInSeconds);
                float smoothAngle = startRotation + rotationDifference * t;
                Rotate(smoothAngle - currentRotationDegrees);
                yield return null;
            }

            //Rotate remaining angle
            Rotate(targetRotation - currentRotationDegrees);
        }
    }
}