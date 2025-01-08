using Portfolio.Shared;
using System.Collections;
using UnityEngine;

namespace Portfolio.MainMenu
{
    public class MainMenuItem : MonoBehaviour, ISceneInfo, IRoteable, ISlidable
    {
        /// <summary>
        /// Menu info
        /// </summary>
        [SerializeField]
        private ScriptableMenuItem menuItemInfo;

        /// <summary>
        /// Allow the object to be roteable
        /// </summary>
        private bool canRotate = false;

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
                    EnableObjectRotation();
                    DisableBillboarding();
                    return;
                }

                if (TryGetComponent<BasicObjectRotator>(out var objectRotator))
                {
                    StartCoroutine(objectRotator.LookAtSmoothly(Camera.main.transform, () =>
                    {
                        DisableObjectRotation();
                        EnableBillboarding();
                    }));
                }
            }

            get
            {
                return canRotate;
            }
        }

        /// <summary>
        /// Scene name
        /// </summary>
        /// <returns></returns>
        string ISceneInfo.GetSceneName()
        {
            return menuItemInfo.Scene.ToString();
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

        /// <summary>
        /// Enable ObjectRotator component
        /// </summary>
        private void EnableObjectRotation() => ChangeObjectRotatorEnabling(true);

        /// <summary>
        /// Dsable ObjectRotator component
        /// </summary>
        private void DisableObjectRotation() => ChangeObjectRotatorEnabling(false);

        /// <summary>
        /// Change ObjectRotator component enable state
        /// </summary>
        private void ChangeObjectRotatorEnabling(bool enable)
        {
            if (TryGetComponent<BasicObjectRotator>(out var objectRotator))
            {
                objectRotator.enabled = enable;
            }
        }

        /// <summary>
        /// Slide the object towards a direction
        /// </summary>
        /// <param name="startPoint">Start point</param>
        /// <param name="endPoint">End point</param>
        /// <param name="duration">Animation duration</param>
        public IEnumerator Slide(Vector3 startPoint, Vector3 endPoint, float duration)
        {
            float time = 0;
            while(time < duration)
            {
                time += Time.deltaTime;
                transform.position = Vector3.Lerp(startPoint, endPoint, time/ duration);
                yield return null;
            }

            transform.position = endPoint;
        }
    }
}