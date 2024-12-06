using UnityEditor;
using UnityEngine;

namespace Portfolio
{
    public class Billboarder : MonoBehaviour
    {
        /// <summary>
        /// Automatically use the main camera
        /// </summary>
        [SerializeField]
        private bool useMainCamera;

        /// <summary>
        /// Target camera
        /// </summary>
        [SerializeField]
        private Camera mainCamera;

        /// <summary>
        /// Lock x axis rotation
        /// </summary>
        [SerializeField] 
        private bool lockX = false;

        /// <summary>
        /// Lock y axis rotation
        /// </summary>
        [SerializeField] 
        private bool lockY = false;

        /// <summary>
        /// Lock z axis rotation
        /// </summary>
        [SerializeField] 
        private bool lockZ = false;

        void Awake()
        {
            if (useMainCamera && mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            if(!useMainCamera && mainCamera == null)
            {
                throw new System.Exception($"[{nameof(Billboarder)}]: Camera null reference");
            }
        }

        private void LateUpdate()
        {
            if (mainCamera == null)
            {
                return;
            }

            LookTarget();
        }

        /// <summary>
        /// Make the object look in camera
        /// </summary>
        public void LookTarget()
        {
            Vector3 direction = mainCamera.transform.position - transform.position;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Vector3 euler = targetRotation.eulerAngles;

            if (lockX)
            {
                euler.x = transform.rotation.eulerAngles.x;
            }

            if (lockY)
            {
                euler.y = transform.rotation.eulerAngles.y;
            }

            if (lockZ)
            {
                euler.z = transform.rotation.eulerAngles.z;
            }

            transform.rotation = Quaternion.Euler(euler);
        }
    }
}