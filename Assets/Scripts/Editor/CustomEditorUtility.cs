using UnityEditor;
using UnityEngine;

namespace Portfolio
{
    public static class CustomEditorUtility
    {
        /// <summary>
        /// Shw in the inspector the target script
        /// </summary>
        /// <param name="target">Editr target script</param>
        public static void ShowClicableTargetScript(UnityEngine.Object target, float space = 15f)
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script:", MonoScript.FromMonoBehaviour(target as MonoBehaviour), typeof(MonoScript), false);
            GUI.enabled = true;

            GUILayout.Space(space);
        }
    }
}
