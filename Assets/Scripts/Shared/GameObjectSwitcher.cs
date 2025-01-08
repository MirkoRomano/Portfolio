using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Portfolio.Shared
{
    public class GameObjectSwitcher : MonoBehaviour
    {
        [Serializable]
        public class SwitchObject
        {
            /// <summary>
            /// GameObjects to switch
            /// </summary>
            public GameObject Object;
            /// <summary>
            /// Seconds between every switch
            /// </summary>
            public float DelayInSeconds;
        }

        /// <summary>
        /// List of object to switch
        /// </summary>
        [SerializeField]
        private SwitchObject[] gameObjects;

        /// <summary>
        /// Number of the switching loop
        /// </summary>
        [SerializeField, Space(15)]
        private uint switchLoopCount;

        /// <summary>
        /// Event that fire when the object switch loop finished executing
        /// </summary>
        [SerializeField, Space(15)]
        private UnityEvent OnSwitchFinishedEvent;

        void OnEnable()
        {
            if (gameObjects == null || gameObjects.Length == 0)
            {
                return;
            }

            StartCoroutine(SwitchGameObjects());
        }

        private void OnDisable()
        {
            StopCoroutine(SwitchGameObjects());
        }

        /// <summary>
        /// Switch objects
        /// </summary>
        /// <returns></returns>
        private IEnumerator SwitchGameObjects()
        {
            int count = 0;
            int currentObject = 0;
            while(switchLoopCount == 0 || count < switchLoopCount)
            {
                for(int i =  0; i < gameObjects.Length; i++)
                {
                    if (gameObjects[i] == null || gameObjects[i].Object == null)
                    {
                        continue;
                    }

                    gameObjects[i].Object.SetActive(i == currentObject);
                }

                yield return new WaitForSeconds(gameObjects[currentObject].DelayInSeconds);

                currentObject++;

                if (currentObject >= gameObjects.Length)
                {
                    count++;
                    currentObject %= gameObjects.Length;
                }
            }
        }
    }
}