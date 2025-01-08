using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Portfolio
{
    [DefaultExecutionOrder(-100)]
    public class InputManager : SingletonMonobehaviour<InputManager>
    {
        public static Vector2 CurrentMousePostion => InputManager.Instance.currentMousePosition;
        public static Vector2 PreviousMousePositon => InputManager.Instance.previousMousePosition;
        public static Vector2 MouseDelta => InputManager.Instance.mouseDelta;
        public static Action<char> KeyboardInput;

        private Vector2 currentMousePosition => Input.mousePosition;
        private Vector2 previousMousePosition;
        private Vector2 mouseDelta;

        private void OnEnable()
        {
            Keyboard.current.onTextInput += BroadcastKeyboardInput;
        }

        private void OnDisable()
        {
            Keyboard.current.onTextInput -= BroadcastKeyboardInput;
        }

        private void Start()
        {
            previousMousePosition = Vector2.zero;
            mouseDelta = Vector2.zero;
        }

        private void Update()
        {
            mouseDelta = currentMousePosition - previousMousePosition;
            previousMousePosition = currentMousePosition;
        }

        /// <summary>
        /// Send the keyboard input through the input manager
        /// </summary>
        /// <param name="input">Keyboard char</param>
        private void BroadcastKeyboardInput(char input)
        {
            KeyboardInput?.Invoke(input);
        }

    }
}