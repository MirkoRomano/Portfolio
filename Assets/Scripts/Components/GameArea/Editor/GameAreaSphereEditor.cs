using Portfolio.Shared;
using UnityEditor;
using UnityEngine;

namespace Kula.EditorScripts
{
    [CustomEditor(typeof(GameAreaSphere))]
    public class GameAreaSphereEditor : Editor
    {
        SerializedProperty radius;
        SerializedProperty color;

        private void OnEnable()
        {
            radius = serializedObject.FindProperty("radius");
            color = serializedObject.FindProperty("gizmoColor");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            //Prevent negative number in the radius
            radius.floatValue = Mathf.Max(radius.floatValue, 0f);

            //Draw fields
            EditorGUILayout.PropertyField(radius);
            EditorGUILayout.PropertyField(color);

            //Apply
            serializedObject.ApplyModifiedProperties();
        }
    }
}