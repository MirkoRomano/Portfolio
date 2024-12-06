// Importing libraries
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine;

namespace NYX {
    public class AxisItem : MonoBehaviour {
        
        // Inspector refs
        [Header("[ References ]")]
        [SerializeField] Text keyNameText;

        #region KeyboardRefs
        [SerializeField] KeyboardUI keyboard;
        [Serializable]
        public class KeyboardUI
        {
            public Text negativeKeycodeText;
            public Text positiveKeycodeText;
        }
        #endregion

        #region GamepadRefs
        [SerializeField] GamepadUI gamepad;
        [Serializable]
        public class GamepadUI
        {
            public Text negativeKeycodeText;
            public Text positiveKeycodeText;

            public GameObject axisUI;
            public GameObject keysUI;

            public Text axisText;
        }

        #endregion

        // Hidden
        [HideInInspector] public int axisIndex;

        // Privates
        NYX_UIManager ui;
        InputManager inputs;
        bool keyboardPositive, keyboardNegative, fireGamepad;
        bool gamepadConfirm, mouseConfirm;
        string currentAction;

        float time; float timer = 0.85f;
        bool hold, released;

        // Start 1 sec after Start()
        IEnumerator Start()
        {
            yield return new WaitForSeconds(1f);

            // Get InputManager & UIManager
            inputs = GameObject.Find("InputManager").GetComponent<InputManager>();
            ui = GameObject.Find("NYX_GUI").GetComponent<NYX_UIManager>();

            // Draw keys for gamepad inputs if no gamepad axis exist
            if(inputs.axis[axisIndex].gamepad.axis == InputManager.GamepadInputs.None)
            { gamepad.axisUI.SetActive(false); gamepad.keysUI.SetActive(true); }

            UpdateValues();
        }

        // FUNCTIONS //
        public void Keyboard_SetPositive() { keyboardPositive = true; ui.AwaitInputPanel(true); }
        public void Keyboard_SetNegative() { keyboardNegative = true; ui.AwaitInputPanel(true); }

        public void Fire_Gamepad(string action) { currentAction = action; fireGamepad = true; ui.AwaitInputPanel(true); }

        #region Utils
        public void UpdateValues()
        {
            keyNameText.text = inputs.axis[axisIndex].axisName + " : ";
            keyboard.positiveKeycodeText.text = inputs.axis[axisIndex].keyboard.positiveKey.ToString();
            keyboard.negativeKeycodeText.text = inputs.axis[axisIndex].keyboard.negativeKey.ToString();
            gamepad.positiveKeycodeText.text = inputs.axis[axisIndex].gamepad.positiveKey.ToString();
            gamepad.negativeKeycodeText.text = inputs.axis[axisIndex].gamepad.negativeKey.ToString();
            gamepad.axisText.text = inputs.axis[axisIndex].gamepad.axis.ToString();
        }

        void KeyboardPositive()
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
                        inputs.axis[axisIndex].keyboard.positiveKey = mouseKeycode;

                        // Close panel & stop waiting
                        ui.AwaitInputPanel(false); UpdateValues();
                        keyboardPositive = false; mouseConfirm = false;
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
                    inputs.axis[axisIndex].keyboard.positiveKey = keyboard.keyCode;

                // Close panel & stop waiting
                ui.AwaitInputPanel(false); UpdateValues();
                keyboardPositive = false;
            }
            #endregion
        }

        void KeyboardNegative()
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
                        inputs.axis[axisIndex].keyboard.negativeKey = mouseKeycode;

                        // Close panel & stop waiting
                        ui.AwaitInputPanel(false); UpdateValues();
                        keyboardNegative = false; mouseConfirm = false;
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
                if(keyboard.keyCode != KeyCode.Escape)
                    inputs.axis[axisIndex].keyboard.negativeKey = keyboard.keyCode;

                // Close panel & stop waiting
                ui.AwaitInputPanel(false); UpdateValues();
                keyboardNegative = false;
            }
            #endregion
        }

        void Gamepad()
        {
            #region AxisInputs

            // Define list of all axis
            string[] axis = { "LeftJoyX", "LeftJoyY", "Triggers", "RightJoyX", "RightJoyY", "CrossX", "CrossY" };

            // Check if one of the axis is used
            for (int i = 0; i < axis.Length; i++)
            {
                if (Input.GetAxis(axis[i]) != 0)
                {
                    // Set correct axis & empty keys
                    inputs.axis[axisIndex].gamepad.axis = (InputManager.GamepadInputs)Enum.Parse(typeof(InputManager.GamepadInputs), axis[i]);
                    inputs.axis[axisIndex].gamepad.positiveKey = KeyCode.None; inputs.axis[axisIndex].gamepad.negativeKey = KeyCode.None;
                    gamepad.axisUI.SetActive(true); gamepad.keysUI.SetActive(false);
                    
                    // Close panel & stop waiting
                    ui.AwaitInputPanel(false); UpdateValues();
                    fireGamepad = false;
                }
            }
            #endregion

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
                        if (time < timer) 
                        {
                            if (currentAction == "positive") { inputs.axis[axisIndex].gamepad.positiveKey = KeyCode.JoystickButton1; }
                            else if (currentAction == "negative") { inputs.axis[axisIndex].gamepad.negativeKey = KeyCode.JoystickButton1; }

                            // Empty axis
                            inputs.axis[axisIndex].gamepad.axis = InputManager.GamepadInputs.None;
                            gamepad.axisUI.SetActive(false); gamepad.keysUI.SetActive(true);
                        }

                        // Close panel & stop waiting
                        ui.AwaitInputPanel(false); UpdateValues();
                        time = 0; hold = false;
                        fireGamepad = false; gamepadConfirm = false;
                    }

                    if (Input.GetKey(gamepadKeycode))
                    {
                        // Check holding time
                        if (gamepadKeycode == KeyCode.JoystickButton1) { hold = true; time += Time.deltaTime; }
                        else
                        {
                            // Set the right keys
                            if (currentAction == "positive") { inputs.axis[axisIndex].gamepad.positiveKey = gamepadKeycode; }
                            else if (currentAction == "negative") { inputs.axis[axisIndex].gamepad.negativeKey = gamepadKeycode; }

                            // Empty axis
                            inputs.axis[axisIndex].gamepad.axis = InputManager.GamepadInputs.None;
                            gamepad.axisUI.SetActive(false); gamepad.keysUI.SetActive(true);

                            // Close panel & stop waiting
                            ui.AwaitInputPanel(false); UpdateValues();
                            fireGamepad = false; gamepadConfirm = false;
                        }
                    }
                    else { if (Input.GetKey(KeyCode.Escape)) { ui.AwaitInputPanel(false); fireGamepad = false; gamepadConfirm = false; } }
                }
            }
            #endregion
        }
        #endregion

        // Await Inputs
        void OnGUI()
        {
            if (fireGamepad) { Gamepad(); }
            if (keyboardPositive){ KeyboardPositive(); }
            if (keyboardNegative) { KeyboardNegative(); }
        }

        void Update() { released = Input.GetKeyUp(KeyCode.JoystickButton1); }
    }
}