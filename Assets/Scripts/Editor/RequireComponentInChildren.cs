using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Portfolio.Shared
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class RequireComponentInChildren : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            MonoBehaviour monoBehaviour = (MonoBehaviour)target;
            var requireAttribute = (RequireComponentInChildrenAttribute)System.Attribute.GetCustomAttribute(
                monoBehaviour.GetType(),
                typeof(RequireComponentInChildrenAttribute)
            );

            if (requireAttribute != null)
            {
                var requiredType = requireAttribute.RequiredType;
                bool componentFound = monoBehaviour.GetComponentInChildren(requiredType) != null;

                if (!componentFound)
                {
                    // Show the popup
                    EditorUtility.DisplayDialog(
                        "Invalid Component Addition",
                        $"The component {monoBehaviour.GetType().Name} requires a {requiredType.Name} in a child GameObject. The component has been removed.",
                        "OK"
                    );

                    // Remove the invalid component
                    Undo.DestroyObjectImmediate(monoBehaviour);

                    // Disable inspector if the requirement is not met
                    GUI.enabled = false;
                }
            }
        }
    }
}

