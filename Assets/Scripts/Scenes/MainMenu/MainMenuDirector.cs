using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Portfolio
{
    public class MainMenuDirector : MonoBehaviour
    {
        /// <summary>
        /// Rotation direction
        /// </summary>
        private enum Direction
        {
            Clockwise = 1,
            ClockwiseClockwise = -1
        }

        /// <summary>
        /// Menu games spawner
        /// </summary>
        [SerializeField]
        private CircleSpawner spawner;

        /// <summary>
        /// Menu rotation animation in secoonds
        /// </summary>
        [SerializeField]
        private float menuRotationAnimationInSeconds;

        /// <summary>
        /// Menu rotation coroutine
        /// </summary>
        private Coroutine animationCoroutine = null;

        /// <summary>
        /// Main camera
        /// </summary>
        private Camera mainCamera;

        private void Start()
        {
            mainCamera = Camera.main;

            if (spawner.GetFacingbject(mainCamera.transform).TryGetComponent<IRoteable>(out var rotatable))
            {
                rotatable.CanRotate = true;
            }
        }

        private void Update()
        {
            if (animationCoroutine != null)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                animationCoroutine = StartCoroutine(RotateMenu(Direction.ClockwiseClockwise));
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                animationCoroutine = StartCoroutine(RotateMenu(Direction.Clockwise));
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Transform objectTransform = spawner.GetFacingbject(mainCamera.transform);

                if (objectTransform.TryGetComponent<ISceneInfo>(out ISceneInfo info))
                {
                    SceneManager.LoadScene(info.GetSceneName());
                }
            }
        }


        /// <summary>
        /// Rotatiion animation
        /// </summary>
        /// <param name="direction">Rotation direction</param>
        private IEnumerator RotateMenu(Direction direction)
        {
            //TOFIX: The item is rotating, and because of that the dot prooduct doesn't return the right facing item
            if (spawner.GetFacingbject(mainCamera.transform).TryGetComponent<IRoteable>(out var rotatable))
            {
                rotatable.CanRotate = false;
            }

            yield return spawner.RotateCoroutine(spawner.AngleStep * (int)direction, menuRotationAnimationInSeconds);

            if (spawner.GetFacingbject(mainCamera.transform).TryGetComponent<IRoteable>(out var nextRotatable))
            {
                nextRotatable.CanRotate = true;
            }

            animationCoroutine = null;
        }


    }
}