using UnityEngine;

namespace Portfolio
{
    public abstract class SingletonMonobehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        /// <summary>
        /// Thread safe lock
        /// </summary>
        private static readonly object padlock = new object();
        
        /// <summary>
        /// Instance
        /// </summary>
        private static T instance = null;

        /// <summary>
        /// Does the instance exist
        /// </summary>
        public static bool HasInstance => instance != null;

        /// <summary>
        /// Application quitting check
        /// </summary>
        private static bool isApplicationQuitting = false;

        /// <summary>
        /// Instance
        /// </summary>
        public static T Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        if (isApplicationQuitting)
                        {
                            throw new System.InvalidOperationException($"{nameof(T)} Cannot create a new instance during application quitting process");
                        }

                        GameObject obj = new GameObject($"{typeof(T)} Singleton");
                        instance = obj.AddComponent<T>();
                        DontDestroyOnLoad(instance);
                    }
                }

                return instance;
            }
        }


        protected virtual void Awake()
        {
            if (HasInstance && !instance.Equals(this))
            {
                Debug.LogError($"Theres already a singleton instance of {this.name}");
                Destroy(this);
                return;
            }

            if (!HasInstance && TryGetComponent<T>(out T target))
            {
                instance = target;
                DontDestroyOnLoad(instance);
                return;
            }
        }

        protected virtual void OnApplicationQuit()
        {
            isApplicationQuitting = true;
        }
    }
}
