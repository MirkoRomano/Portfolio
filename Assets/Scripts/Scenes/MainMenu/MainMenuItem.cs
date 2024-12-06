using System;
using System.Collections;
using UnityEngine;

namespace Portfolio
{
    public interface ISceneInfo
    {
        string GetSceneName();
    }

    public interface IRoteable
    {
        bool CanRotate { set; get; }
    }

    public interface IItemInfo
    {
        string Name { get; }
        string Description { get; }
        int PlayerCount { get; }
    }

    public class MainMenuItem : MonoBehaviour, ISceneInfo, IItemInfo, IRoteable
    {
        /// <summary>
        /// Menu info
        /// </summary>
        [SerializeField]
        private ScriptableMenuItem menuItemInfo;


        [SerializeField]
        private float delayTimeInSeconds = 1f;
       
        /// <summary>
        /// Object rotation speed
        /// </summary>
        [SerializeField]
        private float rotationSpeed = 1f;
        
        /// <summary>
        /// Object return rotation speed
        /// </summary>
        [SerializeField]
        private float returnRotationTimeInSeconds = 1f;

        /// <summary>
        /// Allow the object to be roteable
        /// </summary>
        private bool canRotate = false;

        /// <summary>
        /// Rotation coroutine
        /// </summary>
        private Coroutine coroutine = null;

        /// <summary>
        /// Allow the object to be roteable
        /// </summary>
        public bool CanRotate
        {
            set
            {
                canRotate = value;

                if (canRotate)
                {
                    DisableBillboarding();
                    coroutine = StartCoroutine(ApplyRotation());
                }
                else
                {
                    if(coroutine != null)
                    {
                        StopCoroutine(coroutine);
                    }

                    StartCoroutine(ReturnToPosition(EnableBillboarding));
                }
            }

            get
            {
                return canRotate;
            }
        }

        /// <summary>
        /// Item name
        /// </summary>
        string IItemInfo.Name => menuItemInfo.Name;
        
        /// <summary>
        /// Item description
        /// </summary>
        string IItemInfo.Description => menuItemInfo.Description;

        /// <summary>
        /// Player count
        /// </summary>
        int IItemInfo.PlayerCount => menuItemInfo.PlayersCount;

        /// <summary>
        /// Scene name
        /// </summary>
        /// <returns></returns>
        string ISceneInfo.GetSceneName()
        {
            return menuItemInfo.Scene.SceneName;
        }

        /// <summary>
        /// object rotation
        /// </summary>
        /// <returns></returns>
        private IEnumerator ApplyRotation()
        {
            yield return new WaitForSeconds(delayTimeInSeconds);

            while (true)
            {
                float rotationAmount = rotationSpeed * Time.deltaTime;
                transform.Rotate(0, rotationAmount, 0, Space.World);
                yield return null;
            }
        }

        /// <summary>
        /// Return to the original rotation
        /// </summary>
        /// <param name="callback">Coroutine finished callback</param>
        private IEnumerator ReturnToPosition(Action callback)
        {
            Camera camera = Camera.main;

            Quaternion startingRotation = transform.rotation;
            Quaternion targetRotation = Quaternion.LookRotation(camera.transform.position - transform.position);

            float elapsedTime = 0f;

            while (elapsedTime < returnRotationTimeInSeconds)
            {
                targetRotation = Quaternion.LookRotation(camera.transform.position - transform.position);
                transform.rotation = Quaternion.Lerp(startingRotation, targetRotation, elapsedTime / returnRotationTimeInSeconds);
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            transform.rotation = Quaternion.LookRotation(camera.transform.position - transform.position);
            callback?.Invoke();
        }

        /// <summary>
        /// Enable billboarding component
        /// </summary>
        private void EnableBillboarding() => ChangeBillboardingEnabling(true);

        /// <summary>
        /// Dsable billboarding component
        /// </summary>
        private void DisableBillboarding() => ChangeBillboardingEnabling(false);

        /// <summary>
        /// Change billboarding component enable state
        /// </summary>
        private void ChangeBillboardingEnabling(bool enable)
        {
            if (TryGetComponent<Billboarder>(out var billboarder))
            {
                billboarder.enabled = enable;
            }
        }
    }
}