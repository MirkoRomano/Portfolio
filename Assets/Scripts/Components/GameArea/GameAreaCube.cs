using UnityEngine;

namespace Portfolio.Shared
{
    public class GameAreaCube : GameArea
    {
        /// <summary>
        /// Grid size
        /// </summary>
        [SerializeField]
        private float gridSize = 1f;

        [SerializeField]
        private Vector3 pivot;

        [SerializeField]
        private Vector3 size;

        [SerializeField]
        private bool drawGrid;

        /// <summary>
        /// Calculate if an object is inside the game area
        /// </summary>
        /// <param name="globalPosition">Position of the object to calculate</param>
        public override bool IsPointInsideGameArea(Vector3 globalPosition)
        {
            Vector3 cubeCenter = GetCenter();
            Vector3 min = cubeCenter - size / 2f;
            Vector3 max = cubeCenter + size / 2f;

            return globalPosition.x >= min.x && globalPosition.x <= max.x &&
                   globalPosition.y >= min.y && globalPosition.y <= max.y &&
                   globalPosition.z >= min.z && globalPosition.z <= max.z;
        }

        /// <summary>
        /// Calculate if an object is outside the game area
        /// </summary>
        /// <param name="globalPosition">Position of the object to calculate</param>
        public override bool IsPointOutsideGameArea(Vector3 globalPosition)
        {
            return !IsPointInsideGameArea(globalPosition);
        }

        /// <summary>
        /// Get the scale of the area
        /// </summary>
        public Vector3 GetSize()
        {
            return size;
        }

        /// <summary>
        /// Get the center position of the area
        /// </summary>
        public Vector3 GetCenter()
        {
            return transform.position + GetPivotOffset();
        }

        /// <summary>
        /// Get the pivot position of the area
        /// </summary>
        public Vector3 GetPivot() 
        {
            return new Vector3(pivot.x * size.x - size.x,
                               pivot.y * size.y - size.y,
                               pivot.z * size.z - size.z);
        }

        /// <summary>
        /// Get the grid size of the area
        /// </summary>
        public float GetGridSize()
        {
            return gridSize;
        }

        /// <summary>
        /// Display the game are gizmo
        /// </summary>
        protected override void DisplayGizmo()
        {
            Color gizmoColor = Gizmos.color;
            Gizmos.color = this.gizmoColor;

            if (drawGrid)
            {
                Vector3 cubeCenter = GetCenter();
                Vector3 min = cubeCenter - size / 2f;
                Vector3 max = cubeCenter + size / 2f;

                for (float y = min.y; y <= max.y; y += gridSize)
                {
                    for (float x = min.x; x <= max.x; x += gridSize)
                    {
                        Gizmos.color = GetColor(x, min.x, max.x);
                        Gizmos.DrawLine(new Vector3(x, y, min.z), new Vector3(x, y, max.z));
                    }

                    for (float z = min.z; z <= max.z; z += gridSize)
                    {
                        Gizmos.color = GetColor(z, min.z, max.z);
                        Gizmos.DrawLine(new Vector3(min.x, y, z), new Vector3(max.x, y, z));
                    }
                }

                for (float x = min.x; x <= max.x; x += gridSize)
                {
                    for (float z = min.z; z <= max.z; z += gridSize)
                    {
                        Gizmos.color = GetColorAB(x, min.x, max.x, z, min.z, max.z);
                        Gizmos.DrawLine(new Vector3(x, min.y, z), new Vector3(x, max.y, z));
                    }
                }

                Color GetColor(float value, float minValue, float maxValue) 
                {
                    if (value == minValue || value == maxValue)
                    {
                        return this.gizmoColor;
                    }
                    else
                    {
                        return new Color(1f, 1f, 1f, 0.2f);
                    }
                }

                Color GetColorAB(float a, float minA, float maxA, float b, float minB, float maxB)
                {
                    if ((a == minA || a == maxA) || (b == minB || b == maxB))
                    {
                        return this.gizmoColor;
                    }
                    else
                    {
                        return new Color(1f, 1f, 1f, 0.2f);
                    }
                }

            }
            else
            {
                Gizmos.DrawWireCube(GetCenter(), size);
            }

            Gizmos.color = gizmoColor;
        }

        /// <summary>
        /// Calculates the offset of the cube center based on the pivot.
        /// </summary>
        /// <returns>Vector representing the offset of the cube</returns>
        private Vector3 GetPivotOffset()
        {
            return new Vector3(pivot.x * size.x - (size.x / 2f),
                               pivot.y * size.y - (size.y / 2f),
                               pivot.z * size.z - (size.z / 2f));
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            DisplayGizmo();
        }
#endif
    }
}