// Importing libraries
using System.IO;
using UnityEngine;
using UnityEditor;

namespace NYX {
    public class SettingsWindow : EditorWindow {
        // Resources
        Rect iconRect;
        Texture2D iconImage;

        // Settings
        NYX_Settings project_settings;

        // Declaring a new EditorWindow in editor-menu
        [MenuItem("NYX/Settings")]
        public static void ShowWindow()
        {

            SettingsWindow window = GetWindow<SettingsWindow>("NYX Settings");
            window.minSize = new Vector2(300, 218);
            window.maxSize = new Vector2(600, 218);

            window.LoadSettings();
        }

        // FUNCTIONS //

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

        void LoadSettings()
        {
            string[] assets = AssetDatabase.FindAssets("t:NYX_Settings");
            if (assets.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(assets[0]);
                project_settings = AssetDatabase.LoadAssetAtPath<NYX_Settings>(path);
            }
        }

        // Draw the window content
        private void OnGUI()
        {
            LoadResources();

            /// TITLE ///
            GUILayout.Space(100);
            GUILayout.Button("NYX PREFERENCES :");

            ///////////////////////////////////////

            // Axis Smoothing
            GUILayout.BeginHorizontal();
            GUILayout.Label("Axis Smoothing Multiplicator :", GUILayout.MaxWidth(200));
            project_settings.axisSmoothingFactor = GUILayout.HorizontalSlider(project_settings.axisSmoothingFactor, 0, 100);
            GUILayout.Label(Mathf.Round(project_settings.axisSmoothingFactor).ToString(), GUILayout.MaxWidth(25));
            GUILayout.EndHorizontal();

            // Save Path
            GUILayout.BeginHorizontal();
            GUILayout.Label("Save File Path :", GUILayout.MaxWidth(200));
            project_settings.saveFilePath = GUILayout.TextField(project_settings.saveFilePath);
            GUILayout.EndHorizontal();
            
            GUILayout.Space(20);

            ///////////////////////////////////////

            // Remove nyx
            if (GUILayout.Button("Remove NYX from project"))
            {
                string path = Directory.GetDirectories(Application.dataPath, "NYX", SearchOption.AllDirectories)[0];
                string relativePath = path.Replace(Application.dataPath, "Assets");

                if (Directory.Exists(relativePath)) {  Directory.Delete(relativePath, true); }
            }

            // Reset prefs
            if (GUILayout.Button("RESET"))
            {
                project_settings.axisSmoothingFactor = 50;
                project_settings.saveFilePath = "inputs.nyx";
            }
        }
    }
}