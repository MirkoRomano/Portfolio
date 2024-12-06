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
            if (instance != null && !instance.Equals(this))
            {
                Debug.LogError($"Theres already a singleton instance of {this.name}");
                Destroy(this);
                return;
            }

            if (instance == null && TryGetComponent<T>(out T target))
            {
                instance = target;
                DontDestroyOnLoad(instance);
                return;
            }
        }
    }
}
