// Importing libraries
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine;

namespace NYX {
    public class KeyItem : MonoBehaviour {
        // Inspector refs
        [Header("[ References ]")]
        [SerializeField] Text keyNameText;
        [Space]
        [SerializeField] Text keyboardText;
        [SerializeField] Text gamepadText;

        // Hidden
        [HideInInspector] public int keyIndex;

        // Privates
        NYX_UIManager ui;
        InputManager inputs;
        bool gamepadConfirm, mouseConfirm;
        bool setKeyboard, setGamepad;
        
        float time; float timer = 0.85f;
        bool hold, released;

        // Start 1 sec after Start()
        IEnumerator Start()
        {
            yield return new WaitForSeconds(1f);

            // Get InputManager & UIManager
            inputs = GameObject.Find("InputManager").GetComponent<InputManager>();
            ui = GameObject.Find("NYX_GUI").GetComponent<NYX_UIManager>();

            UpdateValues();
        }

        // FUNCTIONS //
        public void Keyboard_SetKey() { setKeyboard = true; ui.AwaitInputPanel(true); }
        public void Gamepad_SetKey() { time = 0; setGamepad = true; ui.AwaitInputPanel(true); }

        #region Utils
        public void UpdateValues()
        {
            keyNameText.text = inputs.keys[keyIndex].keyName + " : ";
            keyboardText.text = inputs.keys[keyIndex].keyboardKey.ToString();
            gamepadText.text = inputs.keys[keyIndex].gamepadKey.ToString();
        }

        void Keyboard()
        {
            #region MouseHandler

            // Double "Mouse0" press security
            if (!mouseConfirm) { mouseConfirm = !Input.GetKey(KeyCode.Mouse0); }
            if (Input.GetKeyUp(KeyCode.JoystickButton0)) { mouseConfirm = true; }

            if (mouseConfirm)
            {
                // Iterate thrue all M_BTN
                for (int i = 0; i < 6; i++)
                {
                    // Build the keycode
                    KeyCode mouseKeycode = (KeyCode)Enum.Parse(typeof(KeyCode), "Mouse" + i.ToString(), true);
                    if (Input.GetKey(mouseKeycode))
                    {
                        // Set the key
                        inputs.keys[keyIndex].keyboardKey = mouseKeycode;

                        // Close panel & stop waiting
                        ui.AwaitInputPanel(false); UpdateValues();
                        setKeyboard = false; mouseConfirm = false;
                    }
                }
            }
            #endregion

            #region KeyboardHandler

            // Get keys by event
            Event keyboard = Event.current;
            if (keyboard.isKey)
            {
                // Set the key if the user do not cancel
                if (keyboard.keyCode != KeyCode.Escape)
                    inputs.keys[keyIndex].keyboardKey = keyboard.keyCode;

                // Close panel & stop waiting
                ui.AwaitInputPanel(false); UpdateValues();
                setKeyboard = false;
            }
            #endregion
        }

        void Gamepad()
        {
            #region ButtonsInputs

            // Double "JB0" press security
            if (!gamepadConfirm) { gamepadConfirm = !Input.GetKey(KeyCode.JoystickButton0); }
            if (Input.GetKeyUp(KeyCode.JoystickButton0)) { gamepadConfirm = true; }

            if (gamepadConfirm)
            {
                // Iterate thrue all JB's
                for (int i = 0; i < 9; i++)
                {
                    // Build the keycode
                    KeyCode gamepadKeycode = (KeyCode)Enum.Parse(typeof(KeyCode), "JoystickButton" + i.ToString(), true);

                    // Gamepad cancel check
                    if (hold && released)
                    {
                        // Set the key if the user do not cancel
                        if (time < timer) { inputs.keys[keyIndex].gamepadKey = KeyCode.JoystickButton1; }

                        // Close panel & stop waiting
                        ui.AwaitInputPanel(false); UpdateValues();
                        time = 0; hold = false;
                        setGamepad = false; gamepadConfirm = false;
                    }

                    if (Input.GetKey(gamepadKeycode))
                    {
                        // Check holding time
                        if (gamepadKeycode == KeyCode.JoystickButton1) { hold = true; time += Time.deltaTime; }
                        else
                        {
                            // Set the key if the user do not cancel
                            inputs.keys[keyIndex].gamepadKey = gamepadKeycode;

                            // Close panel & stop waiting
                            ui.AwaitInputPanel(false); UpdateValues();
                            setGamepad = false; gamepadConfirm = false;
                        }
                    }
                    else { if (Input.GetKey(KeyCode.Escape)) { ui.AwaitInputPanel(false); setGamepad = false; gamepadConfirm = false; } }
                }
            }
            #endregion
        }
        #endregion

        // Await Inputs
        void OnGUI()
        {
            if (setKeyboard) { Keyboard(); }
            if (setGamepad) { Gamepad(); }
        }

        void Update() { released = Input.GetKeyUp(KeyCode.JoystickButton1); }
    }
}