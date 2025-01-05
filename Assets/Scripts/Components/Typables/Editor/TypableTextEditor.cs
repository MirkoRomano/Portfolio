using Portfolio.Shared;
using System;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Portfolio.UnityEditor
{
    [CustomEditor(typeof(TypableText))]
    public class TypableTextEditor : Editor
    {
        /// <summary>
        /// Default editor space
        /// </summary>
        private const float DEFAULT_SPACE = 15f;
        /// <summary>
        /// Text area min height
        /// </summary>
        private const float TEXTAREA_MIN_HEIGHT = 80f;

        /// <summary>
        /// Color tag (start)
        /// </summary>
        private const string COLOR_START_TAG = "<color={0}>";
        /// <summary>
        /// Color tag format
        /// </summary>
        private const string COLOR_START_TAG_FORMAT = "<color=#ffffff>";
        /// <summary>
        /// Color tag (end)
        /// </summary>
        private const string COLOR_END_TAG = "</color>";

        /// <summary>
        /// Text area field
        /// </summary>
        private SerializedProperty text;
        /// <summary>
        /// Text char array
        /// </summary>
        private SerializedProperty textArray;

        /// <summary>
        /// Color to highlight the typed letters
        /// </summary>
        private SerializedProperty highlitedColor;
        /// <summary>
        /// Color of the remaining text
        /// </summary>
        private SerializedProperty textColor;

        /// <summary>
        /// Reset the highlited text when there's an error
        /// </summary>
        private SerializedProperty resetWhenError;
        /// <summary>
        /// Error treshold for text resetting
        /// </summary>
        private SerializedProperty errorTreshold;

        /// <summary>
        /// Error committed event
        /// </summary>
        private SerializedProperty onErrorCommittedEvent;
        /// <summary>
        /// Treshold exceeded event
        /// </summary>
        private SerializedProperty onErrorTresholdExceedEvent;

        /// <summary>
        /// Target script text child
        /// </summary>
        private SerializedProperty tmpTextProperty;

        /// <summary>
        /// Target script
        /// </summary>
        private TypableText script;
        /// <summary>
        /// Target script text child
        /// </summary>
        private TMP_Text tmpText;
        /// <summary>
        /// Target script text child transform
        /// </summary>
        private RectTransform tmpTextTransform;



        private void OnEnable()
        {
            text = serializedObject.FindProperty("text");
            textArray = serializedObject.FindProperty("textArray");
            highlitedColor = serializedObject.FindProperty("highlitedColor");
            textColor = serializedObject.FindProperty("textColor");
            resetWhenError = serializedObject.FindProperty("resetWhenError");
            errorTreshold = serializedObject.FindProperty("errorTreshold");
            onErrorCommittedEvent = serializedObject.FindProperty("onErrorCommittedEvent");
            onErrorTresholdExceedEvent = serializedObject.FindProperty("onErrorTresholdExceedEvent");
            tmpTextProperty = serializedObject.FindProperty("tmpText");

            script = (TypableText)target;
            RetrieveChildTmpText();

            tmpTextProperty.objectReferenceValue = tmpText;
        }

        public override void OnInspectorGUI()
        {
            string oldTextValue = text.stringValue;

            if (tmpText == null || tmpTextTransform == null)
            {
                RetrieveChildTmpText();
            }

            serializedObject.Update();

            CustomEditorUtility.ShowClicableTargetScript(target);
            DrawTextArea(text);
            DrawColors();
            DrawSettings();
            DrawEvents();

            if (!Application.isPlaying)
            {
                DrawText(oldTextValue);
            }

            MakeTextFillParent();

            serializedObject.ApplyModifiedProperties();
        }

        private void RetrieveChildTmpText()
        {
            tmpText = script.GetComponentInChildren<TMP_Text>();
            tmpTextTransform = tmpText.GetComponent<RectTransform>();
        }

        /// <summary>
        /// Draw color pickers
        /// </summary>
        private void DrawColors()
        {
            GUILayout.Space(DEFAULT_SPACE);


            GUILayout.BeginVertical();
            GUILayout.Label("Colors");
            DrawColorPicker(textColor);
            DrawColorPicker(highlitedColor);
            GUILayout.EndVertical();
        }


        /// <summary>
        /// Draw Settings
        /// </summary>
        private void DrawSettings()
        {
            GUILayout.Space(DEFAULT_SPACE);

            GUILayout.BeginVertical();

            GUILayout.Label("Settings");
            EditorGUILayout.PropertyField(resetWhenError);
            if (resetWhenError.boolValue)
            {
                errorTreshold.intValue = Mathf.Max(errorTreshold.intValue, 0);
                EditorGUILayout.PropertyField(errorTreshold);
            }

            GUILayout.EndVertical();
        }

        /// <summary>
        /// Draw event block
        /// </summary>
        private void DrawEvents()
        {
            GUILayout.Space(DEFAULT_SPACE);

            GUILayout.BeginVertical();
            GUILayout.Label("Events");
            EditorGUILayout.PropertyField(onErrorCommittedEvent);
            EditorGUILayout.PropertyField(onErrorTresholdExceedEvent);
            GUILayout.EndVertical();
        }


        /// <summary>
        /// Draw a text area
        /// </summary>
        /// <param name="property">String serialized property</param>
        private void DrawTextArea(SerializedProperty property)
        {
            GUILayout.BeginVertical();
            GUILayout.Label("Text");
            property.stringValue = EditorGUILayout.TextArea(property.stringValue,
                                                            EditorStyles.textArea,
                                                            GUILayout.MinHeight(TEXTAREA_MIN_HEIGHT));
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Draw html color picker
        /// </summary>
        /// <param name="property">Color serialized property</param>
        private void DrawColorPicker(SerializedProperty property)
        {
            Rect position = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);

            float colorPickerWidth = position.width / 3f;

            Rect htmlField = new Rect(position.x, position.y, position.width - colorPickerWidth, position.height);
            Rect colorField = new Rect(position.x + htmlField.width, position.y, position.width - htmlField.width, position.height);

            string htmlValue = EditorGUI.TextField(htmlField, property.displayName, "#" + ColorUtility.ToHtmlStringRGB(property.colorValue).ToLower());
            if (ColorUtility.TryParseHtmlString(htmlValue, out Color color))
            {
                property.colorValue = color;
            }

            property.colorValue = EditorGUI.ColorField(colorField, property.colorValue);
        }

        /// <summary>
        /// Update text 
        /// </summary>
        /// <param name="oldText">old text value</param>
        private void DrawText(string oldText)
        {
            try
            {
                ReadOnlySpan<char> textValue = text.stringValue;
                ReadOnlySpan<char> tmpTextValue = tmpText.text.AsSpan().Slice(COLOR_START_TAG_FORMAT.Length + COLOR_END_TAG.Length);
                ColorUtility.TryParseHtmlString(tmpText.text.AsSpan().Slice(7, 7).ToString(), out Color tmpTextColor);

                bool isTextCorrect = textValue.SequenceCompareTo(oldText) == 0 &&
                                     textValue.SequenceCompareTo(tmpTextValue) == 0 &&
                                     highlitedColor.colorValue.Equals(tmpTextColor);
                if (!isTextCorrect)
                {
                    UpdateTmpText();
                }

                bool isColorCorrect = textColor.colorValue.Equals(tmpText.color);
                if (!isColorCorrect)
                {
                    tmpText.color = textColor.colorValue;
                }
            }
            catch (Exception)
            {
                UpdateTmpText();
            }
        }

        /// <summary>
        /// Update the text and the array from the tmpText script text with the rich text tags
        /// </summary>
        private void UpdateTmpText()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(string.Format(COLOR_START_TAG, "#" + ColorUtility.ToHtmlStringRGB(highlitedColor.colorValue).ToLower()));
            builder.Append(COLOR_END_TAG);
            builder.Append(text.stringValue);
            
            string textToUpdate = builder.ToString();
            tmpText.text = textToUpdate;

            serializedObject.FindProperty($"{textArray.name}.Array.size").intValue = textToUpdate.Length;
            for (int i = 0; i < textToUpdate.Length; i++)
            {
                SerializedProperty element = serializedObject.FindProperty(string.Format("{0}.Array.data[{1}]", textArray.name, i));
                char character = tmpText.text[i];
                element.intValue = (int)character;
            }
        }

        /// <summary>
        /// Make the child Tmp_Text fill the parent Typable Text
        /// </summary>
        private void MakeTextFillParent()
        {
            tmpTextTransform.anchorMin = Vector2.zero;
            tmpTextTransform.anchorMax = Vector2.one;
            tmpTextTransform.offsetMin = Vector2.zero;
            tmpTextTransform.offsetMax = Vector2.zero;
        }
    }
}