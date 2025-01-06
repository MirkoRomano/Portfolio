using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Portfolio.Shared
{
    [RequireComponentInChildren(typeof(TextMeshProUGUI))]
    public class TypableText : MonoBehaviour, ITypable, ITextInputHandler
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
        /// Color tag (default)
        /// </summary>
        private const string COLOR_START_DEFAULT = "<color=#FFFFFF>";

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

        [SerializeField]
        private TypableGroup group = null;

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
        /// Text completely typed event
        /// </summary>
        [SerializeField]
        private UnityEvent onTextTypedEvent;

        /// <summary>
        /// Text completely resetted event
        /// </summary>
        [SerializeField]
        private UnityEvent onTextResettedEvent;

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
        /// Set the typable text
        /// </summary>
        public string Text
        {
            set
            {
                if(string.Equals(value, text))
                {
                    return;
                }

                text = value;
                ResetTypableText(value);
            }
            get
            {
                return text;
            }
        }

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

                return userInput.Count;
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
                    return text.AsSpan().Slice(0, userInput.Count);
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Check if the user finished to type the entire text
        /// </summary>
        public bool IsTextCompletelyTyped
        {
            get
            {
                return userInput.Count == text.Length;
            }
        }

        /// <summary>
        /// Last input
        /// </summary>
        public char PreviousTypedChar
        {
            get
            {
                if (userInput == null || userInput.Count == 0)
                {
                    throw new InvalidOperationException("There's no char typed previously");
                }

                return userInput.Peek();
            }
        }

        /// <summary>
        /// The next char to type of the typable text
        /// </summary>
        public char CharToType
        {
            get
            {
                if (userInput == null || IsTextCompletelyTyped)
                {
                    throw new InvalidOperationException("There's no char to type");
                }

                return text[userInput.Count];
            }
        }

        /// <summary>
        /// Check if the typable is enabled
        /// </summary>
        public bool Enabled
        {
            get
            {
                return enabled;
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

            ReadOnlySpan<char> originalValue = text.AsSpan();
            ReadOnlySpan<char> richTextValue = tmpText.text.AsSpan().Slice(COLOR_START_DEFAULT.Length, tmpText.text.Length - COLOR_START_DEFAULT.Length);
            bool areTextTheSame = originalValue.SequenceCompareTo(richTextValue) == 0;

            if (!areTextTheSame)
            {
                ResetTypableText(text);
            }
        }

        private void OnEnable()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            SetGroup(group);
            RegisterKeyboardInput();
        }

        private void OnDisable()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            SetGroup(null);
            UnregisterKeyboardInput();
        }

        /// <summary>
        /// Catch a typed input from the keyboard
        /// </summary>
        void ITextInputHandler.CatchInput(char inputChar)
        {
            CatchInput(inputChar);
        }

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

            if (IsTextCompletelyTyped)
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
                    ResetText();
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

                if (IsTextCompletelyTyped)
                {
                    onTextTypedEvent?.Invoke();
                }
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

        /// <summary>
        /// Reset the text to it's original version
        /// </summary>
        public void ResetText()
        {
            ResetTypableText(text);
            onTextResettedEvent?.Invoke();
        }

        /// <summary>
        /// Register / Unregister the typable to the group if any
        /// </summary>
        /// <param name="typableGroup">Typable group</param>
        /// <param name="setMemberValue">Reset the gropu if true</param>
        private void SetGroup(TypableGroup typableGroup, bool setMemberValue = false)
        {
            if(group != null)
            {
                group.UnregisterToggle(this);
            }

            if (setMemberValue)
            {
                group = typableGroup;
            }

            if(typableGroup != null && enabled)
            {
                group.RegisterToggle(this);
            }
        }

        /// <summary>
        /// Register the typable to the keyboard
        /// </summary>
        private void RegisterKeyboardInput()
        {
            //Register only if the group is not setted
            if(group != null)
            {
                return;
            }

            Keyboard.current.onTextInput += CatchInput;
        }

        /// <summary>
        /// Unregister the typable from the keyboard
        /// </summary>
        private void UnregisterKeyboardInput() 
        {
            Keyboard.current.onTextInput -= CatchInput;
        }
    }
}