using UnityEngine;

namespace Portfolio.Shared
{
    public class GameAreaSphere : GameArea
    {
        /// <summary>
        /// Radius of the sphere game area
        /// </summary>
        [SerializeField]
        private float radius;

        /// <summary>
        /// Calculate if an object is inside the game area
        /// </summary>
        /// <param name="position">Position of the object to calculate</param>
        public override bool IsPointInsideGameArea(Vector3 position)
        {
            float distanceFromCenter = Vector3.Distance(position, transform.position);
            return distanceFromCenter < radius;
        }

        /// <summary>
        /// Calculate if an object is outside the game area
        /// </summary>
        /// <param name="position">Position of the object to calculate</param>
        public override bool IsPointOutsideGameArea(Vector3 position)
        {
            return !IsPointInsideGameArea(position);
        }

        /// <summary>
        /// Get the center of the game area
        /// </summary>
        public Vector3 GetCenter()
        {
            return transform.position;
        }

        /// <summary>
        /// Display the game are gizmo
        /// </summary>
        protected override void DisplayGizmo()
        {
            Color gizmoColor = Gizmos.color;
            Gizmos.color = this.gizmoColor;
            Gizmos.DrawWireSphere(transform.position, radius);
            Gizmos.color = gizmoColor;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            DisplayGizmo();
        }
#endif
    }
}