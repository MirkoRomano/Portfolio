// Importing libraries
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine.SceneManagement;

namespace NYX {
    public class SceneManagementWindow : EditorWindow {
        // Resources vars
        Rect iconRect;
        Texture2D iconImage;

        // Declaring a new EditorWindow in editor-menu
        [MenuItem("NYX/Scene Management")]
        public static void ShowWindow()
        {
            SceneManagementWindow window = GetWindow<SceneManagementWindow>(SceneManager.GetActiveScene().name + " Management");
            window.minSize = new Vector2(250, 180);
            window.maxSize = new Vector2(500, 180);
        }

        // Load & show graphicals resources
        void LoadResources()
        {
            #region LoadIconResource
            // Load Icon
            iconImage = Resources.Load<Texture2D>("GUI/Logos/LogoWhite");
            // Define Icon Rect
            iconRect.x = (Screen.width / 2) - 100;
            iconRect.y = -25;
            iconRect.width = 200;
            iconRect.height = 150;
            // Draw Icon
            GUI.DrawTexture(iconRect, iconImage);
            #endregion
        }

        // Draw the window content
        private void OnGUI()
        {
            LoadResources();
            GUILayout.Space(100);

            // Instanciate NYX prefabs in the scene
            if (GUILayout.Button("Setup Scene"))
            {
                GameObject Systems = Instantiate(Resources.Load("Core/Prefabs/NYX_Systems")) as GameObject;
                Systems.name = "NYX_Systems";
            }

            // Instanciate KeyConfigMenu prefab to the scene
            if (GUILayout.Button("Add KeyConfig Menu"))
            {
                GameObject GUI = Instantiate(Resources.Load("GUI/Prefabs/NYX_GUI")) as GameObject;
                GameObject Event = Instantiate(Resources.Load("GUI/Prefabs/NYX_EventSystem")) as GameObject;
                GUI.name = "NYX_GUI";
                Event.name = "NYX_EventSystem";
            }

            // Create a UnityPresetFile for the inputs
            if (GUILayout.Button("Create NYX Config"))
            {
                // Check if a InputManager is selected and act in consequence
                if (Selection.activeGameObject != null)
                {
                    InputManager inputs = Selection.activeGameObject.GetComponent<InputManager>();

                    if (inputs != null)
                    {
                        Preset preset = new Preset(inputs);
                        string path = Directory.GetDirectories(Application.dataPath, "Presets", SearchOption.AllDirectories)[0];
                        string relativePath = path.Replace(Application.dataPath, "Assets");
                        AssetDatabase.CreateAsset(preset, relativePath + "/" + "myNewConfig" + ".preset");
                        Object newConfig = AssetDatabase.LoadAssetAtPath(relativePath + "/" + "myNewConfig.preset", typeof(Preset));
                        Selection.activeObject = newConfig;
                        EditorGUIUtility.PingObject(newConfig);
                    }
                    else
                    { Debug.LogWarning("Unable to find an 'InputManager' class under the current selected GameObject, please verify your selection."); }
                }
                else
                { Debug.LogWarning("Please select a GameObject with the 'InputManager' component on it."); }
            }
            GUILayout.BeginHorizontal();

            // Select the config file in the assets
            if (GUILayout.Button("Edit Config"))
            {
                // Load object
                string path = Directory.GetDirectories(Application.dataPath, "Presets", SearchOption.AllDirectories)[0];
                string relativePath = path.Replace(Application.dataPath, "Assets");
                Object config = AssetDatabase.LoadAssetAtPath(relativePath + "/" + "DefaultConfig.preset", typeof(Preset));
                Selection.activeObject = config;
                EditorGUIUtility.PingObject(config);
            }

            // Remove all NYX prefabs from the scene
            if (GUILayout.Button("Remove All"))
            {
                DestroyImmediate(GameObject.Find("NYX_Systems"));
                DestroyImmediate(GameObject.Find("NYX_GUI"));
                DestroyImmediate(GameObject.Find("NYX_EventSystem"));
            }
            GUILayout.EndHorizontal();
        }
    }
}