using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Portfolio
{
    public sealed class CircleSpawner : MonoBehaviour, IEnumerable<Transform>
    {
        /// <summary>
        /// Entire angle in degrees
        /// </summary>
        private const float ANGLE_FULL_CIRCLE = 360.0f;

        /// <summary>
        /// Prefab of objects to spawn
        /// </summary>
        [SerializeField]
        private GameObject prefab = null;

        /// <summary>
        /// Radius of the circle of points
        /// </summary>
        [SerializeField]
        private float distance = 0f;

        /// <summary>
        /// Number of points to create and rotate
        /// </summary>
        [SerializeField]
        private int numberOfObjects = 0;

        /// <summary>
        /// Rotation directin of the vector
        /// </summary>
        [SerializeField]
        private Vector3 axisRotationDirection = Vector3.zero;

        /// <summary>
        /// Generated points
        /// </summary>
        [SerializeField]
        private Transform[] objects = new Transform[0];

        /// <summary>
        /// Rotation angle of a single point
        /// </summary>
        private float angleStep => ANGLE_FULL_CIRCLE / numberOfObjects;

        /// <summary>
        /// Current rotation degrees of the circle points
        /// </summary>
        private float currentRotationDegrees = 0;

        /// <summary>
        /// Points count
        /// </summary>
        public int Count => numberOfObjects;

        /// <summary>
        /// Is readonly
        /// </summary>
        public bool IsReadOnly => true;

        /// <summary>
        /// Array index
        /// </summary>
        /// <param name="index">Index of the point</param>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        public Transform this[int index]
        {
            get
            {
                if (index < 0 || index >= objects.Length)
                {
                    throw new System.IndexOutOfRangeException($"Index {index} is out of range.");
                }

                return objects[index];
            }
        }

        /// <summary>
        /// Enumerator for points
        /// </summary>
        public IEnumerator<Transform> GetEnumerator()
        {
            return ((IEnumerable<Transform>)objects).GetEnumerator();
        }

        /// <summary>
        /// Enumerator for points (non-generic version)
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return objects.GetEnumerator();
        }


        /// <summary>
        /// Rotate every points instantaneously by an angle
        /// </summary>
        /// <param name="angle">Rotation angle</param>
        public void Rotate(float angle)
        {
            currentRotationDegrees = Mathf.Repeat(currentRotationDegrees + angle, ANGLE_FULL_CIRCLE);

            for (int i = 0; i < numberOfObjects; i++)
            {
                float pointAngle = Mathf.Repeat(currentRotationDegrees + (angleStep * i), ANGLE_FULL_CIRCLE);
                objects[i].position = objects[i].position.RotateAround(transform.position, pointAngle, distance, axisRotationDirection);
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