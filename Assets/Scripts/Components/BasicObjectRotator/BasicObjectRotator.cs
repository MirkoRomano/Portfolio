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
        /// <param name="callback">Coroutine finished callback</param>
        public IEnumerator LookAtSmoothly(Action callback)
        {
            Camera camera = Camera.main;

            Quaternion startingRotation = transform.rotation;
            Quaternion targetRotation = Quaternion.LookRotation(camera.transform.position - transform.position);

            float elapsedTime = 0f;

            while (elapsedTime < lookAtSmoothTimeInSeconds)
            {
                targetRotation = Quaternion.LookRotation(camera.transform.position - transform.position);
                transform.rotation = Quaternion.Lerp(startingRotation, targetRotation, elapsedTime / lookAtSmoothTimeInSeconds);
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            transform.rotation = Quaternion.LookRotation(camera.transform.position - transform.position);
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