using Portfolio.Shared;
using UnityEditor;
using UnityEngine;

namespace Kula.EditorScripts
{
    [CustomEditor(typeof(GameAreaCube))]
    public class GameAreaCubeEditor : Editor
    {
        SerializedProperty pivot;
        SerializedProperty size;
        SerializedProperty color;
        SerializedProperty drawGrid;

        private void OnEnable()
        {
            pivot = serializedObject.FindProperty("pivot");
            size = serializedObject.FindProperty("size");
            color = serializedObject.FindProperty("gizmoColor");
            drawGrid = serializedObject.FindProperty("drawGrid");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            //Prevent negative numbers
            pivot.vector3Value = Clamp01(pivot.vector3Value);
            size.vector3Value = PreventNegativeNumbers(size.vector3Value);
            
            //Draw fields
            EditorGUILayout.PropertyField(pivot);
            EditorGUILayout.PropertyField(size);
            EditorGUILayout.PropertyField(color);
            EditorGUILayout.PropertyField(drawGrid);

            //Apply
            serializedObject.ApplyModifiedProperties();
        }

        private Vector3 PreventNegativeNumbers(Vector3 value)
        {
            return new Vector3(Mathf.Max(value.x, 0f),
                               Mathf.Max(value.y, 0f),
                               Mathf.Max(value.z, 0f));
        }

        private Vector3 Clamp01(Vector3 value) 
        {
            return new Vector3(Mathf.Clamp01(value.x), 
                               Mathf.Clamp01(value.y), 
                               Mathf.Clamp01(value.z));
        }
    } 
}
