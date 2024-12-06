using UnityEngine;

namespace NYX
{
    public class NYX_Settings : ScriptableObject
    {
        [Header("Project Settings")]

        [Range(0, 100)]
        public float axisSmoothingFactor;
        public string saveFilePath;
    }
}