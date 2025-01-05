using UnityEngine;
using UnityEditor;

namespace Portfolio.Shared
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class RequireComponentInParentEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            MonoBehaviour monoBehaviour = (MonoBehaviour)target;
            var requireAttribute = (RequireComponentInParentAttribute)System.Attribute.GetCustomAttribute(
                monoBehaviour.GetType(),
                typeof(RequireComponentInParentAttribute)
            );

            if (requireAttribute != null)
            {
                var requiredType = requireAttribute.RequiredType;
                bool componentFound = monoBehaviour.GetComponentInParent(requiredType) != null;

                if (!componentFound)
                {
                    // Show the popup
                    EditorUtility.DisplayDialog(
                        "Invalid Component Addition",
                        $"The component {monoBehaviour.GetType().Name} requires a {requiredType.Name} in a parent GameObject. The component has been removed.",
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