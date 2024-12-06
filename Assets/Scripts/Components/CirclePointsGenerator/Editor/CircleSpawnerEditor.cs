using UnityEditor;
using UnityEngine;

namespace Portfolio.UnityEditor
{
    [CustomEditor(typeof(CircleSpawner))]
    public class CircleSpawnerEditor : Editor
    {
        /// <summary>
        /// Min axis directin clamp
        /// </summary>
        private const float MIN_CLAMP = -1f;

        /// <summary>
        /// Max axis direction clamp
        /// </summary>
        private const float MAX_CLAMP = 1f;

        /// <summary>
        /// Slider x value
        /// </summary>
        private float sliderX = 0f;

        /// <summary>
        /// Slider y value
        /// </summary>
        private float sliderY = 0f;

        /// <summary>
        /// Slider z value
        /// </summary>
        private float sliderZ = 0f;

        /// <summary>
        /// Prefab to spawn
        /// </summary>
        private SerializedProperty prefab;

        /// <summary>
        /// Distance of the prefab from the pivot
        /// </summary>
        private SerializedProperty distance;

        /// <summary>
        /// Number of the objects to spawn
        /// </summary>
        private SerializedProperty numberOfObjects;

        /// <summary>
        /// Axis rotation Direction of the objects
        /// </summary>
        private SerializedProperty axisRotationDirection;

        /// <summary>
        /// Spawned object array
        /// </summary>
        private SerializedProperty objects;

        private void OnEnable()
        {
            prefab = serializedObject.FindProperty("prefab");
            distance = serializedObject.FindProperty("distance");
            numberOfObjects = serializedObject.FindProperty("numberOfObjects");
            axisRotationDirection = serializedObject.FindProperty("axisRotationDirection");
            objects = serializedObject.FindProperty("objects");

            sliderX = axisRotationDirection.vector3Value.x;
            sliderY = axisRotationDirection.vector3Value.y;
            sliderZ = axisRotationDirection.vector3Value.z;
        }

        public override void OnInspectorGUI()
        {
            float oldRadius = distance.floatValue;
            int oldNumberOfPoints = numberOfObjects.intValue;
            Vector3 oldRotationDirection = axisRotationDirection.vector3Value;

            serializedObject.Update();

            EditorUtility.ShowClicableTargetScript(target);

            EditorGUILayout.Space(15);
            EditorGUILayout.PropertyField(prefab);

            EditorGUILayout.Space(15);
            EditorGUILayout.PropertyField(distance);
            EditorGUILayout.PropertyField(numberOfObjects);

            EditorGUILayout.Space(15);
            sliderX = EditorGUILayout.Slider("Rotation X", sliderX, MIN_CLAMP, MAX_CLAMP);
            sliderY = EditorGUILayout.Slider("Rotation Y", sliderY, MIN_CLAMP, MAX_CLAMP);
            sliderZ = EditorGUILayout.Slider("Rotation Z", sliderZ, MIN_CLAMP, MAX_CLAMP);
            axisRotationDirection.vector3Value = new Vector3(sliderX, sliderY, sliderZ);

//this logic is for test only
#if false
            GUI.enabled = false;
            EditorGUILayout.PropertyField(objects);
            GUI.enabled = true;
#endif

            if (prefab.objectReferenceValue != null)
            {
                CircleSpawner script = (CircleSpawner)target;

                if (AreObjectsChanged(oldNumberOfPoints, out int count))
                {
                    if (numberOfObjects.intValue == 0)
                    {
                        DestroySurplusObjects(script, 0);
                    }
                    else if (count > 0 || script.transform.childCount <= 0)
                    {
                        SpawnSurplusObjects(script, oldNumberOfPoints, numberOfObjects.intValue);
                    }
                    else
                    {
                        DestroySurplusObjects(script, numberOfObjects.intValue);
                    }

                    UpdateObjectsPosition(script);
                }

                if (IsObjectsPositionChanged(oldNumberOfPoints, oldRotationDirection))
                {
                    UpdateObjectsPosition(script);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Update the object position in according to the inspector values
        /// </summary>
        /// <param name="script">target</param>
        private void UpdateObjectsPosition(CircleSpawner script)
        {
            float stepAngle = 360f / numberOfObjects.intValue;
            for (int i = 0; i < numberOfObjects.intValue; i++)
            {
                SerializedProperty element = serializedObject.FindProperty(string.Format("objects.Array.data[{0}]", i));
                if (element.objectReferenceValue == null)
                {
                    continue;
                }

                Transform transform = element.objectReferenceValue as Transform;
                transform.position = script.transform.position.RotateAround(script.transform.position, stepAngle * i, distance.floatValue, axisRotationDirection.vector3Value);

            }
        }


        /// <summary>
        /// Spawn object in according to the inspector value
        /// </summary>
        /// <param name="script">target</param>
        /// <param name="oldCount">Old objects count</param>
        /// <param name="newCount">New objects count</param>
        private void SpawnSurplusObjects(CircleSpawner script, int oldCount, int newCount)
        {
            Transform transform = script.transform;

            serializedObject.FindProperty("objects.Array.size").intValue = newCount;
            for (int i = 0; i < newCount; i++)
            {
                SerializedProperty element = serializedObject.FindProperty(string.Format("objects.Array.data[{0}]", i));
                if (element.objectReferenceValue != null && script.transform.childCount > i)
                {
                    Transform child = script.transform.GetChild(i);
                    child.name = $"Object_{i}";
                    element.objectReferenceValue = child;
                    continue;
                }

                GameObject instantiatedObject = PrefabUtility.InstantiatePrefab(prefab.objectReferenceValue, transform) as GameObject;
                instantiatedObject.name = $"Object_{i}";
                element.objectReferenceValue = instantiatedObject.transform;
            }
        }

        /// <summary>
        /// Destroy useless ojects
        /// </summary>
        /// <param name="script">target</param>
        /// <param name="newCount">New count of objects</param>
        private void DestroySurplusObjects(CircleSpawner script, int newCount)
        {
            for (int i = script.transform.childCount - 1; i >= newCount; i--)
            {
                if (i > script.transform.childCount)
                {
                    continue;
                }

                DestroyImmediate(script.transform.GetChild(i).gameObject);
            }

            serializedObject.FindProperty("objects.Array.size").intValue = newCount;
        }

        /// <summary>
        /// Are object inspector count changed
        /// </summary>
        /// <param name="oldNumberOfPoints">old objects value</param>
        /// <param name="difference">Difference</param>
        /// <returns></returns>
        private bool AreObjectsChanged(int oldNumberOfPoints, out int difference)
        {
            difference = numberOfObjects.intValue - oldNumberOfPoints;
            return !oldNumberOfPoints.Equals(numberOfObjects.intValue);
        }

        /// <summary>
        /// Is object position changed in the inspector
        /// </summary>
        /// <param name="oldDistance">old distance value</param>
        /// <param name="oldAxisRotationDirection">Old axis rotation direction value</param>
        /// <returns></returns>
        private bool IsObjectsPositionChanged(float oldDistance, Vector3 oldAxisRotationDirection)
        {
            return !Mathf.Approximately(oldDistance, distance.floatValue) ||
                   !oldAxisRotationDirection.Equals(axisRotationDirection.vector3Value);
        }
    }
}