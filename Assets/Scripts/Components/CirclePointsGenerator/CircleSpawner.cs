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
        /// Current rotation degrees of the circle points
        /// </summary>
        private float currentRotationDegrees = 0;

        /// <summary>
        /// Rotation angle of a single point
        /// </summary>
        public float AngleStep => ANGLE_FULL_CIRCLE / numberOfObjects;

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

        private void Awake()
        {
            if(prefab == null)
            {
                Debug.LogWarning($"[{nameof(CircleSpawner)}]: circle spawner prefab null reference");
            }
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
                float pointAngle = Mathf.Repeat(currentRotationDegrees + (AngleStep * i), ANGLE_FULL_CIRCLE);
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

            float rotationDifference = Mathf.DeltaAngle(startRotation, targetRotation);

            while (elapsedTime < durationInSeconds)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / durationInSeconds);
                float interpolatedRotation = startRotation + rotationDifference * t;
                Rotate(interpolatedRotation - currentRotationDegrees);
                yield return null;
            }

            Rotate(targetRotation - currentRotationDegrees);
        }

        /// <summary>
        /// Get the object that's facing another object
        /// </summary>
        /// <param name="facingDirection">Facing direction normalizedVector</param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        public Transform GetFacingbject(Vector3 facingDirection)
        {
            float mostNegativeDot = 0f;
            int perpendicularObjectIndex = 0;
            
            for (int i = 0; i < objects.Length; i++) 
            {
                float prooduct = -Vector3.Dot(objects[i].forward, facingDirection);
                if(prooduct < mostNegativeDot) 
                {
                    mostNegativeDot = prooduct;
                    perpendicularObjectIndex = i;
                }
            }

            if(Mathf.Approximately(mostNegativeDot, -1f))
            {
                return objects[perpendicularObjectIndex];
            }

            throw new NotFoundException("Most opposte object not found");
        }

    }
}