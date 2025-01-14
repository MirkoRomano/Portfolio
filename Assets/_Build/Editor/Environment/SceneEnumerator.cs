using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Portfolio.UnityEditor
{
    public sealed class SceneEnumerator
    {
        /// <summary>
        /// Scene folder name
        /// </summary>
        private const string SCENE_FOLDER_NAME = "Scenes";
        /// <summary>
        /// Root folder name
        /// </summary>
        private const string ROOT_FOLDER_NAME = "Assets/";
        /// <summary>
        /// Scene name to place at the top of the list
        /// </summary>
        private const string FIRST_SCENE_NAME = "Scn_MainMenu";
        /// <summary>
        /// Unity scene exstension name
        /// </summary>
        private const string SCENE_EXTENSION_NAME = ".unity";
        /// <summary>
        /// Enum script save path
        /// </summary>
        private const string ENUM_SAVE_PATH = "Scripts/Autogenerated/SceneName.cs";
        /// <summary>
        /// Enum padding space count
        /// </summary>
        private const int ENUM_PADDING_COUNT = 8;

        [MenuItem("Environment/Enumerate Scenes")]
        public static void EnumerateScenes()
        {
            EditorUtility.DisplayProgressBar("Scene Enumeration", "Enumerating the scenes", 0.5f);

            try
            {
                List<string> scenesList = new List<string>();
                RetrieveScenePaths(ref scenesList, string.Format("{0}/{1}", Application.dataPath, SCENE_FOLDER_NAME));

                if (scenesList.Count == 0)
                {
                    EditorUtility.DisplayDialog("Scene Enumeration", "No scene were found in Assets/Scene folder. Please check your scene position inside the project folder", "OK");
                    return;
                }

                Sort(ref scenesList);
                UpdateUnitySceneEnumeration(scenesList);
                GenerateUnitySceneEnumFile(scenesList);

                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        /// <summary>
        /// Retrieve all the scene contained in the path Assets/Scenes
        /// </summary>
        /// <param name="scenes">List of scenes</param>
        /// <param name="directoryPath">Directory path to check</param>
        private static void RetrieveScenePaths(ref List<string> scenes, string directoryPath) 
        {
            IEnumerable<string> filePaths = Directory.GetFiles(directoryPath)
                                                     .Concat(Directory.GetDirectories(directoryPath));

            foreach (var path in filePaths)
            {
                FileAttributes attr = File.GetAttributes(path);

                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    RetrieveScenePaths(ref scenes, path);
                    continue;
                }

                if (!string.Equals(Path.GetExtension(path), SCENE_EXTENSION_NAME, System.StringComparison.InvariantCulture))
                {
                    continue;
                }

                int scenesIndex = path.IndexOf(ROOT_FOLDER_NAME, StringComparison.InvariantCultureIgnoreCase);
                if (scenesIndex == -1)
                {
                    Debug.LogWarning($"Il file '{path}' non contiene la directory '{SCENE_FOLDER_NAME}'. Ignorato.");
                    continue;
                }

                scenes.Add(path.Substring(scenesIndex).Replace("\\", "/"));
            }
        }

        /// <summary>
        /// Sort the list of scenes and place the right scene in the first place
        /// </summary>
        /// <param name="scenes">Scene path list</param>
        private static void Sort(ref List<string> scenes)
        {
            scenes.Sort((x, y) =>
                string.Compare(Path.GetFileNameWithoutExtension(x), 
                               Path.GetFileNameWithoutExtension(y), 
                               StringComparison.OrdinalIgnoreCase));

            int index = scenes.FindIndex(scene => Path.GetFileNameWithoutExtension(scene)
                                                      .Contains(FIRST_SCENE_NAME, StringComparison.OrdinalIgnoreCase));

            if (index != -1 && index != 0)
            {
                string element = scenes[index];
                scenes.RemoveAt(index);
                scenes.Insert(0, element);
            }
        }

        /// <summary>
        /// Add to the unity enumerator every scene contained in the path Assets/Scenes
        /// </summary>
        /// <param name="scenePaths">Scene path list</param>
        private static void UpdateUnitySceneEnumeration(List<string> scenePaths)
        {
            EditorBuildSettingsScene[] newScenes = new EditorBuildSettingsScene[scenePaths.Count];
            
            for (int i = 0; i < scenePaths.Count; i++) 
            {
                newScenes[i] = new EditorBuildSettingsScene(scenePaths[i], true);
            }

            EditorBuildSettings.scenes = newScenes.ToArray();
        }

        /// <summary>
        /// Generate a c# script that contains only the enumerator of the scenes
        /// </summary>
        private static void GenerateUnitySceneEnumFile(List<string> scenePaths)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("namespace Portfolio.Shared");
            builder.AppendLine("{");
            builder.AppendLine("    /*");
            builder.AppendLine("     * THIS SCRIPT IS AUTMATICALLY GENERATED");
            builder.AppendLine("     * BY THE EDITOR SCRIPT: SceneEnumerator.cs");
            builder.AppendLine("     * ");
            builder.AppendLine("     * FOR UPDATE THIS ENUM, PLEASE CLICK THE FOLLOWING");
            builder.AppendLine("     * TAB IN THE EDITOR TOOLBAR:");
            builder.AppendLine("     * ");
            builder.AppendLine("     * Environment/Enumerate Scenes");
            builder.AppendLine("     */");
            builder.AppendLine("    public enum SceneName");
            builder.AppendLine("    {");

            for (int i = 0; i < scenePaths.Count; i++)
            {
                string name = Path.GetFileNameWithoutExtension(scenePaths[i]);
                builder.AppendLine(name.PadLeft(name.Length + ENUM_PADDING_COUNT, ' ') + $" = {i},");
            }

            builder.AppendLine("    }");
            builder.AppendLine("}");

            File.WriteAllText(string.Format("{0}/{1}", Application.dataPath, ENUM_SAVE_PATH), builder.ToString());
        }
    }
}
