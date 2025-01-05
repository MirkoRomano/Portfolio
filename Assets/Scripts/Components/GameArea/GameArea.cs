using UnityEngine;

namespace Portfolio.Shared
{
    [DisallowMultipleComponent]
    public abstract class GameArea : MonoBehaviour
    {
        /// <summary>
        /// Color of the gizmo
        /// </summary>
        [SerializeField]
        protected Color gizmoColor;

        public abstract bool IsPointInsideGameArea(Vector3 globalPosition);
        public abstract bool IsPointOutsideGameArea(Vector3 globalPosition);
        protected abstract void DisplayGizmo();
    }
}
