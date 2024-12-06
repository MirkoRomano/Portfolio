// Importing libraries
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace NYX {
    public class OnlineDocumentation {
        // Declaring a new void in editor-menu
        [MenuItem("Window/NYX Documentations/NYX Online Documentation")]
        static void OpenMyWebsite() { Application.OpenURL("https://www.fichier-pdf.fr/2023/03/12/nyxdocumentation/"); }
    }

    public class EditorDocumentation : EditorWindow {
        // Resources vars
        List<Texture2D> images = new List<Texture2D>();
        int currentImageIndex = 0;

        // Declaring a new EditorWindow in editor-menu
        [MenuItem("Window/NYX Documentations/NYX Editor Documentation")]
        public static void ShowWindow()
        {
            EditorDocumentation window = GetWindow<EditorDocumentation>("Editor Documentation");
            window.minSize = new Vector2(177, 250);
            window.maxSize = new Vector2(1414, 2000);
        }

        // Draw the window content
        private void OnGUI()
        {
            // Check if images where found
            if (images.Count == 0) { GUILayout.Label("No documentation found.", EditorStyles.centeredGreyMiniLabel); return; }
            else { GUILayout.Label("Page" + currentImageIndex + "/" + (images.Count - 1), EditorStyles.centeredGreyMiniLabel); }

            ///////////////////////////////////////

            /// BUTTONS ///
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("<", GUILayout.Width(30)))
            {
                currentImageIndex -= 1;
                if (currentImageIndex < 0) { currentImageIndex = images.Count - 1; }
            }

            if (GUILayout.Button(">", GUILayout.Width(30)))
            {
                currentImageIndex += 1;
                if (currentImageIndex >= images.Count) { currentImageIndex = 0; }
            }
            GUILayout.EndHorizontal();

            ///////////////////////////////////////

            /// RESPONSIVE IMAGE DRAW ///
            Rect windowRect = position;
            GUILayout.Label(images[currentImageIndex], GUILayout.Width(windowRect.width), GUILayout.Height(windowRect.height));
        }

        // On Window opening
        private void OnEnable()
        {
            // Clear images list
            images.Clear();

            // finding and asigning Files names
            string path = Directory.GetDirectories(Application.dataPath, "DOC_Images", SearchOption.AllDirectories)[0];
            string relativePath = path.Replace(Application.dataPath, "Assets");
            string[] files = Directory.GetFiles(relativePath, "*.png");

            // Create array of 2D textures
            Texture2D[] textures = new Texture2D[files.Length];

            // Asigning Textures to array
            for (int i = 0; i < files.Length; i++)
            {
                byte[] data = File.ReadAllBytes(files[i]);
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(data);
                textures[i] = texture;
            }

            // Transfering Local array to Global array
            foreach (Object texture in textures)
            {
                Texture2D tex = texture as Texture2D;
                if (tex != null)
                {
                    images.Add(tex);
                }
            }
        }
    }
}