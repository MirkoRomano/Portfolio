using Portfolio.Shared;
using UnityEditor;
using UnityEngine;

namespace Portfolio.UnityEditor
{
    [CustomPropertyDrawer(typeof(MinMaxAttribute))]
    public class MinMaxDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.Vector2)
            {
                float textFieldSize = 50f;
                float labelWidth = EditorGUIUtility.labelWidth;
                float spacing = 5f;

                MinMaxAttribute minMax = (MinMaxAttribute)attribute;

                Vector2 vector = property.vector2Value;
                EditorGUI.BeginProperty(position, label, property);

                //Label
                Rect labelBoxRect = new Rect(position.x,
                                             position.y,
                                             labelWidth,
                                             position.height);

                EditorGUI.LabelField(labelBoxRect, label);

                //Min
                Rect leftBoxRect = new Rect(position.x + labelWidth,
                                            position.y,
                                            textFieldSize,
                                            position.height);

                vector.x = Mathf.Clamp(EditorGUI.FloatField(leftBoxRect, vector.x), minMax.Min, minMax.Max);

                //Slider
                Rect sliderRect = new Rect(position.x + labelWidth + textFieldSize + spacing,
                                           position.y,
                                           position.width - (textFieldSize * 2) - labelWidth - (spacing * 2),
                                           position.height);

                EditorGUI.MinMaxSlider(sliderRect, ref vector.x, ref vector.y, minMax.Min, minMax.Max);

                //Max
                Rect rightBoxRect = new Rect(position.x + position.width - textFieldSize,
                                             position.y,
                                             textFieldSize,
                                             position.height);

                vector.y = Mathf.Clamp(EditorGUI.FloatField(rightBoxRect, vector.y), minMax.Min, minMax.Max);

                property.vector2Value = vector;

                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }
}