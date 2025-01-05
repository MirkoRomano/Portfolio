using System;
using UnityEngine;

namespace Portfolio.Shared
{
    [AttributeUsage(System.AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class RequireComponentInParentAttribute : Attribute
    {
        public System.Type RequiredType { get; }

        public RequireComponentInParentAttribute(System.Type requiredType)
        {
            if (!typeof(Component).IsAssignableFrom(requiredType))
            {
                throw new System.ArgumentException("RequireComponentInParent can only be used with types derived from UnityEngine.Component");
            }

            RequiredType = requiredType;
        }
    }
}
