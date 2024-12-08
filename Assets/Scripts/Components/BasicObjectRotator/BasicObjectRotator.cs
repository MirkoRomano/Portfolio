using System;
using System.Collections;
using UnityEngine;

namespace Portfolio
{
    public class BasicObjectRotator : MonoBehaviour
    {
        /// <summary>
        /// Rotation direction
        /// </summary>
        public enum Direction
        {
            Clockwise = 1,
            CounterClockwise = -1
        }

        /// <summary>
        /// Rotatiion diirection
        /// </summary>
        [SerializeField]
        private Direction direction;

        /// <summary>
        /// Start delay rotation when the script enables
        /// </summary>
        [SerializeField]
        private float startDelayTimeInSeconds;

        /// <summary>
        /// rotation speed
        /// </summary>
        [SerializeField]
        private float rotationAnglePerSecond;

        /// <summary>
        /// Rotation time in seconds
        /// </summary>
        [SerializeField]
        private float lookAtSmoothTimeInSeconds;

        /// <summary>
        /// Rotation coroutine
        /// </summary>
        private Coroutine coroutine;

        private void OnEnable()
        {
            if (coroutine != null) 
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }

            coroutine = StartCoroutine(ApplyRotation());
        }


        private void OnDisable() 
        {
            if(coroutine == null)
            {
                return;
            }

            StopCoroutine(coroutine);
            coroutine = null;
        }


        /// <summary>
        /// Apply rotation to the object
        /// </summary>
        private IEnumerator ApplyRotation()
        {
            yield return new WaitForSeconds(startDelayTimeInSeconds);

            while (true)
            {
                float rotationAmount = (rotationAnglePerSecond * (int)direction) * Time.deltaTime;
                transform.Rotate(0, rotationAmount, 0, Space.World);
                yield return null;
            }
        }

        /// <summary>
        /// Return to the original rotation
        /// </summary>
        /// <param name="objectToLook">Transform of the object to look</param>
        /// <param name="callback">Coroutine finished callback</param>
        public IEnumerator LookAtSmoothly(Transform objectToLook, Action callback)
        {
            Vector3 targetDirection = objectToLook.position - transform.position;
            targetDirection.y = 0;

            float currentAngle = Mathf.Atan2(transform.forward.x, transform.forward.z) * Mathf.Rad2Deg;
            float targetAngle = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;
            float angleDifference = Mathf.DeltaAngle(currentAngle, targetAngle);

            float elapsedTime = 0f;
            while (elapsedTime < lookAtSmoothTimeInSeconds)
            {
                float interpolatedAngle = Mathf.LerpAngle(currentAngle, currentAngle + angleDifference, elapsedTime / lookAtSmoothTimeInSeconds);
                transform.rotation = Quaternion.Euler(0, interpolatedAngle, 0);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Vector3 finalDirection = objectToLook.position - transform.position;
            finalDirection.y = 0;
            float finalAngle = Mathf.Atan2(finalDirection.x, finalDirection.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, finalAngle, 0);

            callback?.Invoke();
        }

        /// <summary>
        /// Set the start delay time
        /// </summary>
        /// <param name="delaySeconds">Delay time in seconds</param>
        public void SetStartDelayTime(float delaySeconds)
        {
            startDelayTimeInSeconds = delaySeconds;
        }

        /// <summary>
        /// Set the rotation angle per second
        /// </summary>
        /// <param name="rotationAngle">Rotation angle per second</param>
        public void SetRotationAngle(float rotationAngle)
        {
            rotationAnglePerSecond = rotationAngle;
        }

        /// <summary>
        /// Set the new rotation direction of the object
        /// </summary>
        /// <param name="rotationDirection">Object rotation direction</param>
        public void ChangeDirection(Direction rotationDirection) 
        {
            direction = rotationDirection;
        }
    }
}