using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Portfolio.Shared
{
    [RequireComponentInChildren(typeof(TextMeshProUGUI))]
    public class TypableText : MonoBehaviour
    {
        /// <summary>
        /// Color tag (start)
        /// </summary>
        private const string COLOR_START_TAG = "<color=#{0}>";

        /// <summary>
        /// Color tag (start)
        /// </summary>
        private const string COLOR_END_TAG = "</color>";

        /// <summary>
        /// Color tag hexadecimal value
        /// </summary>
        private const string COLOR_HEX_FORMAT = "FFFFFF";

        /// <summary>
        /// Text to type
        /// </summary>
        [SerializeField]
        private string text;

        /// <summary>
        /// Text to type char array
        /// </summary>
        [SerializeField, HideInInspector]
        private char[] textArray = new char[0];

        /// <summary>
        /// Color to highlight the typed letters
        /// </summary>
        [SerializeField, ColorHtmlProperty]
        private Color highlitedColor;

        /// <summary>
        /// Color of the remaining text
        /// </summary>
        [SerializeField, ColorHtmlProperty]
        private Color textColor;

        /// <summary>
        /// Reset the highlited text when there's an error
        /// </summary>
        [SerializeField]
        private bool resetWhenError;

        /// <summary>
        /// Error treshold for text resetting
        /// </summary>
        [SerializeField]
        private int errorTreshold = 1;

        /// <summary>
        /// Error committed event
        /// </summary>
        [SerializeField]
        private UnityEvent onErrorCommittedEvent;

        /// <summary>
        /// Treshold exceeded event
        /// </summary>
        [SerializeField]
        private UnityEvent onErrorTresholdExceedEvent;

        /// <summary>
        /// Target script text child
        /// </summary>
        [SerializeField, HideInInspector]
        private TMP_Text tmpText;

        /// <summary>
        /// User input queue
        /// </summary>
        private readonly Queue<char> userInput = new Queue<char>();

        /// <summary>
        /// Current error count
        /// </summary>
        private int currentErrorCount;

        /// <summary>
        /// Total error count
        /// </summary>
        private int totalErrorCount;

        /// <summary>
        /// Total error count
        /// </summary>
        public int TypedErrorCount
        {
            get
            {
                return currentErrorCount;
            }
        }

        /// <summary>
        /// Typed length
        /// </summary>
        public int TypedLength
        {
            get
            {
                if (userInput == null)
                {
                    return 0;
                }

                return text.Length - userInput.Count;
            }
        }

        /// <summary>
        /// Typed text
        /// </summary>
        public ReadOnlySpan<char> TypedText
        {
            get
            {
                try
                {
                    return text.AsSpan().Slice(0, TypedLength);
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Last input
        /// </summary>
        public char CurrentChar
        {
            get
            {
                if (userInput == null || userInput.Count == 0)
                {
                    return ' ';
                }

                return userInput.Peek();
            }
        }

        /// <summary>
        /// Highlited color
        /// </summary>
        private string htmlHighlitedColor
        {
            get
            {
                return ColorUtility.ToHtmlStringRGBA(highlitedColor);
            }
        }

        private void Awake()
        {
            if (tmpText == null)
            {
                if (!gameObject.TryGetComponentInChildren<TMP_Text>(out tmpText))
                {
                    throw new NullReferenceException($"[{nameof(TypableText)}]: Null text component");
                }
            }
        }

        private void OnEnable()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            Keyboard.current.onTextInput += CatchInput;
        }

        private void OnDisable()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            Keyboard.current.onTextInput -= CatchInput;
        }

        //    /// <summary>
        //    /// Set the original text (without rich text)
        //    /// </summary>
        //    /// <param name="value">Text to set</param>
        //    public void SetupText(string value)
        //    {
        //        originalText = value;
        //        text = value;

        //        if(textArray != null)
        //        {
        //            Array.Clear(textArray, 0, textArray.Length);
        //        }
        //    }


        /// <summary>
        /// Catch a typed input from the keyboard
        /// </summary>
        /// <param name="input">Typed input</param>
        /// <exception cref="NullReferenceException"></exception>
        private void CatchInput(char input)
        {
            if (tmpText == null || text == null || tmpText.text == null || text.Length <= 0 || tmpText.text.Length <= 0)
            {
                throw new NullReferenceException($"[{nameof(TypableText)}]: {gameObject.name} No text has been found");
            }

            if (userInput.Count == text.Length)
            {
                Debug.Log($"[{nameof(TypableText)}]: Text typable lenght reached");
                return;
            }

            int currentIndex = userInput.Count;
            char currentInput = input;

            if (!currentInput.Equals(text[currentIndex]))
            {
                if (resetWhenError && currentErrorCount >= errorTreshold - 1)
                {
                    ResetTypableText(text);
                    currentErrorCount = 0;
                    onErrorTresholdExceedEvent?.Invoke();
                    return;
                }

                currentErrorCount++;
                totalErrorCount++;
                onErrorCommittedEvent?.Invoke();

                Debug.Log($"[{nameof(TypableText)}]: Incorrect input");
            }
            else
            {
                userInput.Enqueue(input);
                MoveCharRight(ref textArray);
                tmpText.SetCharArray(textArray);
            }
        }

        /// <summary>
        /// Reset typable input
        /// </summary>
        private void ResetTypableText(string value)
        {
            userInput.Clear();

            StringBuilder builder = new StringBuilder();
            builder.Append(string.Format(COLOR_START_TAG, htmlHighlitedColor));
            builder.Append(COLOR_END_TAG);
            builder.Append(value);

            string textToReset = builder.ToString();
            tmpText.text = textToReset;
            textArray = textToReset.ToCharArray();
        }

        /// <summary>
        /// Move message char right
        /// </summary>
        /// <param name="chars">Char of array to cange</param>
        private void MoveCharRight(ref char[] chars)
        {
            int endTagLength = COLOR_END_TAG.Length;
            int moveIndex = chars.GetIndexOf(COLOR_END_TAG);

            if (moveIndex == -1 || moveIndex + endTagLength > chars.Length - 1)
            {
                return;
            }

            chars.MoveCharRight(moveIndex, endTagLength);
        }
    }
}