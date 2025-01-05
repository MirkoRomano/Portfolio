using System;
using UnityEngine;

namespace Portfolio.Shared
{
    public static class MonoBehaviourExtension
    {
        public static T GetComponentInParentRecursive<T>(this Transform obj) where T : Component
        {
            Transform parent = obj.transform.parent;

            if (parent == null)
            {
                return null;
            }

            if (!parent.TryGetComponent<T>(out T component))
            {
                return parent.GetComponentInParentRecursive<T>();
            }

            return component;
        }

        public static bool TryGetComponentInChildren<T>(this GameObject obj, out T component) where T : Component
        {
            component = null;

            try
            {
                component = obj.GetComponentInChildren<T>();
                return component != null;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
