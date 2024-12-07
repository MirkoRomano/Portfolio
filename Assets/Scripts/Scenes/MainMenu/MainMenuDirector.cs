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
            CounterClockwise = -1
        }

        /// <summary>
        /// Menu games spawner
        /// </summary>
        [SerializeField]
        private CircleSpawner spawner;

        /// <summary>
        /// Menu rotation animation in secoonds
        /// </summary>
        [SerializeField, Space(15)]
        private float menuRotationAnimationInSeconds;

        /// <summary>
        /// Facing object slide animation vector
        /// </summary>
        [SerializeField]
        private Vector3 facingObjectSlideVector;

        /// <summary>
        /// Facind object slide animation duration in seconds
        /// </summary>
        [SerializeField]
        private float facingObjectRotationAnimationInSeconds;

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

            Transform facingObject = spawner.GetFacingbject(mainCamera.transform).GetChild(0);
            if (facingObject.TryGetComponent<IRoteable>(out var facingItem))
            {
                facingItem.CanRotate = true;
            }

            if (facingObject.TryGetComponent<ISlidable>(out var slidable))
            {
                Vector3 startPoint = facingObject.transform.position;
                Vector3 endPoint = facingObject.transform.position + facingObjectSlideVector;
                StartCoroutine(slidable.Slide(startPoint, endPoint, facingObjectRotationAnimationInSeconds));
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
                animationCoroutine = StartCoroutine(RotateMenu(Direction.CounterClockwise));
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
            Transform facingObject = spawner.GetFacingbject(mainCamera.transform).GetChild(0);
            if (facingObject.TryGetComponent<IRoteable>(out var facingItem))
            {
                facingItem.CanRotate = false;
            }

            if (facingObject.TryGetComponent<ISlidable>(out var slidable))
            {
                Vector3 startPoint = facingObject.transform.position;
                Vector3 endPoint = facingObject.transform.position + -facingObjectSlideVector;
                yield return slidable.Slide(startPoint, endPoint, facingObjectRotationAnimationInSeconds);
            }

            yield return spawner.RotateCoroutine(spawner.AngleStep * (int)direction, menuRotationAnimationInSeconds);

            facingObject = spawner.GetFacingbject(mainCamera.transform).GetChild(0);
            if (facingObject.TryGetComponent<ISlidable>(out  slidable))
            {
                Vector3 startPoint = facingObject.transform.position;
                Vector3 endPoint = facingObject.transform.position + facingObjectSlideVector;
                yield return slidable.Slide(startPoint, endPoint, facingObjectRotationAnimationInSeconds);
            }

            if (facingObject.TryGetComponent<IRoteable>(out facingItem))
            {
                facingItem.CanRotate = true;
            }

            animationCoroutine = null;
        }


    }
}