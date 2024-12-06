using UnityEditor;
using UnityEngine;

namespace Portfolio.UnityEditor
{
    [CustomEditor(typeof(Billboarder))]
    public class BillboarderEditor : Editor
    {
        /// <summary>
        /// Take automatically the reference of the camera if true
        /// </summary>
        private SerializedProperty useMainCamera;
        /// <summary>
        /// Main camera reference
        /// </summary>
        private SerializedProperty mainCamera;

        /// <summary>
        /// Lock x axis
        /// </summary>
        private SerializedProperty lockX;
        /// <summary>
        /// Lock y axis
        /// </summary>
        private SerializedProperty lockY;
        /// <summary>
        /// Lock z axis
        /// </summary>
        private SerializedProperty lockZ;
        /// <summary>
        /// Target script
        /// </summary>
        private Billboarder script;

        private void OnEnable()
        {
            useMainCamera = serializedObject.FindProperty("useMainCamera");
            mainCamera = serializedObject.FindProperty("mainCamera");
            lockX = serializedObject.FindProperty("lockX");
            lockY = serializedObject.FindProperty("lockY");
            lockZ = serializedObject.FindProperty("lockZ");
            
            script = target as Billboarder;

            if (useMainCamera.boolValue && mainCamera.objectReferenceValue == null)
            {
                mainCamera.objectReferenceValue = Camera.main;
                serializedObject.ApplyModifiedProperties();
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorUtility.ShowClicableTargetScript(target);
            
            EditorGUILayout.Space(15);
            EditorGUILayout.PropertyField(useMainCamera);

            if (!useMainCamera.boolValue)
            {
                EditorGUILayout.PropertyField(mainCamera);
            }

            EditorGUILayout.Space(15);

            EditorGUILayout.PropertyField(lockX);
            EditorGUILayout.PropertyField(lockY);
            EditorGUILayout.PropertyField(lockZ);

            serializedObject.ApplyModifiedProperties();
        }
    }
}