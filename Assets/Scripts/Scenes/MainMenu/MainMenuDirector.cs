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

        /// <summary>
        /// Item that's facing the camera
        /// </summary>
        IRoteable facingItem = null;

        private void Start()
        {
            mainCamera = Camera.main;

            if (spawner.GetFacingbject(mainCamera.transform).TryGetComponent<IRoteable>(out facingItem))
            {
                facingItem.CanRotate = true;
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
            facingItem.CanRotate = false;

            yield return spawner.RotateCoroutine(spawner.AngleStep * (int)direction, menuRotationAnimationInSeconds);

            //TOFIX: sometimes it took the most far item
            if (spawner.GetFacingbject(mainCamera.transform).TryGetComponent<IRoteable>(out facingItem))
            {
                facingItem.CanRotate = true;
            }

            animationCoroutine = null;
        }


    }
}