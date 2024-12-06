// Importing libraries
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

namespace NYX {
    public class InputManager : MonoBehaviour {
        // Vars
        [Header("[ Inputs ]")]
        [Tooltip("List of all keys in scene.")] public Key[] keys;

        #region KeyClass
        // Backup Class
        [HideInInspector] public List<DefaultKey> defaultKeys;
        [Serializable]
        public class DefaultKey
        {
            public KeyCode keyboard_key;
            public KeyCode gamepad_key;
        }

        // Class
        [Serializable]
        public class Key
        {
            [Tooltip("Name of your key action.")] public string keyName = "myKey";
            [Tooltip("The keycode to use with keyboards.")] public KeyCode keyboardKey = KeyCode.Space;
            [Tooltip("The button to use on gamepads.")] public KeyCode gamepadKey = KeyCode.Joystick1Button0;
        }
        #endregion

        [Header("[ Axis ]")]
        [Tooltip("List of all axis in scene.")] public Axis[] axis;

        #region AxisClass
        // Backup Class
        [HideInInspector] public List<DefaultAxis> defaultAxis;
        [Serializable]
        public class DefaultAxis
        {
            public KeyCode keyboard_positive;
            public KeyCode keyboard_negative;

            public KeyCode gamepad_positive;
            public KeyCode gamepad_negative;

            public GamepadInputs gamepad_axis;
        }

        // Enum for controllers
        public enum GamepadInputs
        {
            None,
            LeftJoyX,
            LeftJoyY,
            Triggers,
            RightJoyX,
            RightJoyY,
            CrossX,
            CrossY
        }

        // Classes
        [Serializable]
        public class KeyboardAxis
        {
            [Tooltip("The key that will return '1' value.")] public KeyCode positiveKey = KeyCode.UpArrow;
            [Tooltip("The key that will return '-1' value.")] public KeyCode negativeKey = KeyCode.DownArrow;
        }

        [Serializable]
        public class GamepadAxis
        {
            [Tooltip("The key that will return '1' value.")] public KeyCode positiveKey = KeyCode.None;
            [Tooltip("The key that will return '-1' value.")] public KeyCode negativeKey = KeyCode.None;

            [Tooltip("The axis to use when a gamepad is connected.")]
            public GamepadInputs axis = GamepadInputs.LeftJoyY;
        }

        [Serializable]
        public class Axis
        {
            [Tooltip("Name of your axis.")] public string axisName = "myAxis";

            [Tooltip("Keyboard inputs config.")] public KeyboardAxis keyboard;
            [Tooltip("Gamepad inputs config.")] public GamepadAxis gamepad;
            
            [Tooltip("Should your axis be smoothed ?")] public bool useAxisSmoothing = false;
            [Tooltip("The amount of smoothing added to the axis.")] [Range(0, 1)] public float axisSmoothingAmount = 0.5f;
            [Tooltip("The realtime value returned by the axis.")] public float value = 0f;
            
            [HideInInspector] public float refValue = 0;
        }
        #endregion

        NYX_Settings project_settings;
        string path;

        void Awake()
        {
            // Get project settings
            project_settings = Resources.Load("Presets/ProjectConfiguration/NYX_settings") as NYX_Settings;
            path = Application.dataPath + "/" + project_settings.saveFilePath;

            // Backup the inputs
            PackingDefaultInputs();
            // Loads the saved inputs
            LoadInputs();
        }

        // FUNCTIONS //

        public void PackingDefaultInputs()
        {
            #region packing keys
            for (int i = 0; i < keys.Length; i++)
            {
                // Create new instance of a backup
                DefaultKey newItem = new DefaultKey();

                // Store correct data inside
                newItem.keyboard_key = keys[i].keyboardKey;
                newItem.gamepad_key = keys[i].gamepadKey;

                // Add the new instance in the list of backups keys
                defaultKeys.Add(newItem);
            }
            #endregion

            #region packing axis
            for (int i = 0; i < axis.Length; i++)
            {
                // Create new instance of a backup
                DefaultAxis newItem = new DefaultAxis();

                // Store correct data inside
                newItem.keyboard_positive = axis[i].keyboard.positiveKey;
                newItem.keyboard_negative = axis[i].keyboard.negativeKey;
                newItem.gamepad_negative = axis[i].gamepad.negativeKey;
                newItem.gamepad_negative = axis[i].gamepad.negativeKey;
                newItem.gamepad_axis = axis[i].gamepad.axis;

                // Add the new instance in the list of backups axis
                defaultAxis.Add(newItem);
            }
            #endregion
        }

        public void LoadInputs()
        {
            // Check if save already exist to avoid load-errors
            if (File.Exists(path))
            {
                // Get inputs by index from custom-save file

                #region LoadKeys
                for (int i = 0; i < keys.Length; i++)
                {
                    keys[i].keyboardKey = (KeyCode)Enum.Parse(typeof(KeyCode), ReadString(i.ToString() + "_key_keyboard"));
                    keys[i].gamepadKey = (KeyCode)Enum.Parse(typeof(KeyCode), ReadString(i.ToString() + "_key_gamepad"));
                }
                #endregion

                #region LoadAxis
                for (int i = 0; i < axis.Length; i++)
                {
                    axis[i].keyboard.positiveKey = (KeyCode)Enum.Parse(typeof(KeyCode), ReadString(i.ToString() + "_axis_keyboard_positive"));
                    axis[i].keyboard.negativeKey = (KeyCode)Enum.Parse(typeof(KeyCode), ReadString(i.ToString() + "_axis_keyboard_negative"));
                    axis[i].gamepad.positiveKey = (KeyCode)Enum.Parse(typeof(KeyCode), ReadString(i.ToString() + "_axis_gamepad_positive"));
                    axis[i].gamepad.negativeKey = (KeyCode)Enum.Parse(typeof(KeyCode), ReadString(i.ToString() + "_axis_gamepad_negative"));
                    axis[i].gamepad.axis = (GamepadInputs)Enum.Parse(typeof(GamepadInputs), ReadString(i.ToString() + "_axis_gamepad_axis"));
                }
                #endregion
            }

        }

        public void SaveInputs()
        {
            // Save data by index in custom-save file

            #region SaveKeys
            for (int i = 0; i < keys.Length; i++)
            {
                SaveString(i.ToString() + "_key_keyboard", keys[i].keyboardKey.ToString());
                SaveString(i.ToString() + "_key_gamepad", keys[i].gamepadKey.ToString());
            }
            #endregion

            #region SaveAxis
            for (int i = 0; i < axis.Length; i++)
            {
                SaveString(i.ToString() + "_axis_keyboard_positive", axis[i].keyboard.positiveKey.ToString());
                SaveString(i.ToString() + "_axis_keyboard_negative", axis[i].keyboard.negativeKey.ToString());
                SaveString(i.ToString() + "_axis_gamepad_positive", axis[i].gamepad.positiveKey.ToString());
                SaveString(i.ToString() + "_axis_gamepad_negative", axis[i].gamepad.negativeKey.ToString());
                SaveString(i.ToString() + "_axis_gamepad_axis", axis[i].gamepad.axis.ToString());
            }
            #endregion
        }

        public void ResetInputs()
        {
            // Delete the saved file
            File.Delete(path);

            // Reset values based on backups classes
            #region ResetKeys
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i].keyboardKey = defaultKeys[i].keyboard_key;
                keys[i].gamepadKey = defaultKeys[i].gamepad_key;
            }
            #endregion

            #region ResetAxis
            for (int i = 0; i < axis.Length; i++)
            {
                axis[i].keyboard.positiveKey = defaultAxis[i].keyboard_positive; axis[i].keyboard.negativeKey = defaultAxis[i].keyboard_negative;
                axis[i].gamepad.positiveKey = defaultAxis[i].gamepad_positive; axis[i].gamepad.negativeKey = defaultAxis[i].gamepad_negative;
                axis[i].gamepad.axis = defaultAxis[i].gamepad_axis;
            }
            #endregion
        }

        // SAVE SYSTEM //

        #region SaveUtils
        void SaveString(string key, string value)
        {
            // If file exist, read to avoid key-duplication
            if (File.Exists(path))
            {
                // Get all the lines in the file
                string[] lines = File.ReadAllLines(path);

                // Get all lines different from the key
                string[] filteredLines = lines.Where(line => !line.Contains("%" + key + "%")).ToArray();

                // re-write file with only thoses to avoid duplication
                File.WriteAllLines(path, filteredLines);
            }

            // Save key to file
            File.AppendAllText(path, "%" + key + "%" + ":" + value + "\n");
        }

        string ReadString(string key)
        {
            // Get value by key
            string data = File.ReadLines(path).SkipWhile(line => !line.Contains("%" + key + "%")).First();
            string value = data.Split(":")[1];
            
            // Return value if finded
            return value;
        }
        #endregion

        // Update keys & axis
        void Update()
        {
            for (int i = 0; i < axis.Length; i++)
            {
                // Do we use gamepad ?
                float gamepadValue = 1;
                if (Input.GetAxis(axis[i].gamepad.axis.ToString()) !=0)
                { gamepadValue = Mathf.Abs(Input.GetAxis(axis[i].gamepad.axis.ToString())); }

                // Positive
                if (Input.GetKey(axis[i].keyboard.positiveKey) || Input.GetKey(axis[i].gamepad.positiveKey) || Input.GetAxis(axis[i].gamepad.axis.ToString()) > 0)
                {
                    if (axis[i].useAxisSmoothing)
                    { axis[i].value = Mathf.SmoothDamp(axis[i].value, gamepadValue, ref axis[i].refValue, axis[i].axisSmoothingAmount * (project_settings.axisSmoothingFactor / 64) ); }
                    else {  axis[i].value = 1; }
                }

                // Negative
                else if (Input.GetKey(axis[i].keyboard.negativeKey) || Input.GetKey(axis[i].gamepad.negativeKey) || Input.GetAxis(axis[i].gamepad.axis.ToString()) < 0)
                {
                    if (axis[i].useAxisSmoothing)
                    { axis[i].value = Mathf.SmoothDamp(axis[i].value, -gamepadValue, ref axis[i].refValue, axis[i].axisSmoothingAmount * (project_settings.axisSmoothingFactor / 64) ); }
                    else { axis[i].value = -1; }
                }

                // Zero Return
                else
                {
                    if (axis[i].useAxisSmoothing)
                    { axis[i].value = Mathf.SmoothDamp(axis[i].value, 0, ref axis[i].refValue, axis[i].axisSmoothingAmount * (project_settings.axisSmoothingFactor / 64) ); }
                    else { axis[i].value = 0; }
                }
            }
        }
    }
}