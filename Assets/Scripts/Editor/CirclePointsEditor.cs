using UnityEditor;
using UnityEngine;

namespace Portfolio
{
    [CustomEditor(typeof(CirclePoints))]
    public class CirclePointsGeneratorEditor : Editor
    {
        private const float MIN_CLAMP = -1f;
        private const float MAX_CLAMP = 1f;

        private float sliderX = 0f;
        private float sliderY = 0f;
        private float sliderZ = 0f;

        private SerializedProperty radius;
        private SerializedProperty numberOfPoints;
        private SerializedProperty rotationDirection;
        private SerializedProperty points;

        private void OnEnable()
        {
            radius = serializedObject.FindProperty("radius");
            numberOfPoints = serializedObject.FindProperty("numberOfPoints");
            rotationDirection = serializedObject.FindProperty("rotationDirection");
            points = serializedObject.FindProperty("points");

            sliderX = rotationDirection.vector3Value.x;
            sliderY = rotationDirection.vector3Value.y;
            sliderZ = rotationDirection.vector3Value.z;
        }

        public override void OnInspectorGUI()
        {
            float oldRadius = radius.floatValue;
            int oldNumberOfPoints = numberOfPoints.intValue;
            Vector3 oldRotationDirection = rotationDirection.vector3Value; 

            serializedObject.Update();
            EditorGUILayout.Space(15);

            EditorGUILayout.PropertyField(radius);
            EditorGUILayout.PropertyField(numberOfPoints);

            EditorGUILayout.Space(15);
            sliderX = EditorGUILayout.Slider("Rotation X", sliderX, MIN_CLAMP, MAX_CLAMP);
            sliderY = EditorGUILayout.Slider("Rotation Y", sliderY, MIN_CLAMP, MAX_CLAMP);
            sliderZ = EditorGUILayout.Slider("Rotation Z", sliderZ, MIN_CLAMP, MAX_CLAMP);

            rotationDirection.vector3Value = new Vector3(sliderX, sliderY, sliderZ);
            if (IsEditorChanged(oldRadius, oldNumberOfPoints, oldRotationDirection))
            {
                CirclePoints generator = (CirclePoints)target;
                float stepAngle = 360f / numberOfPoints.intValue;
                points.arraySize = numberOfPoints.intValue;
                for (int i = 0; i < numberOfPoints.intValue; i++) 
                {
                    serializedObject.FindProperty(string.Format("points.Array.data[{0}]", i)).vector3Value = generator.transform.position.RotateAround(generator.transform.position, stepAngle * i, radius.floatValue, rotationDirection.vector3Value);
                }
            }

            EditorGUILayout.Space(15);

            GUI.enabled = false;
            EditorGUILayout.PropertyField(points);
            GUI.enabled = true;
            
            serializedObject.ApplyModifiedProperties();
        }

        private void OnSceneGUI()
        {
            int? size = serializedObject.FindProperty("points.Array.size")?.intValue;
            if (size != null && size > 0)
            {
                Handles.color = Color.green;
                for (int i = 0; i < points.arraySize; ++i)
                {
                    if (i == 0)
                    {
                        Handles.color = Color.yellow;
                    }
                    else
                    {
                        Handles.color = Color.green;
                    }
                    SerializedProperty element = serializedObject.FindProperty(string.Format("points.Array.data[{0}]",  i));
                    Handles.DrawWireCube(element.vector3Value, Vector3.one);
                }
            }
        }

        private bool IsEditorChanged(float oldRadius, int oldNumberOfPoints, Vector3 oldRotationDirection)
        {
            return !Mathf.Approximately(oldRadius, radius.floatValue) ||
                   !oldNumberOfPoints.Equals(numberOfPoints.intValue) ||
                   !oldRotationDirection.Equals(rotationDirection.vector3Value);
        }

        private void ClampDirection()
        {
            Vector3 direction = rotationDirection.vector3Value;
            rotationDirection.vector3Value = new Vector3(Mathf.Clamp01(direction.x),
                                                         Mathf.Clamp01(direction.y),
                                                         Mathf.Clamp01(direction.z));
        }
    }
 }